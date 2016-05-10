using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Qvizzen.Controller;
using Qvizzen.Model;

namespace Qvizzen.Networking
{
    public class Server
    {
        List<SocketHelper> Clients;
        Thread TCPThread;
        Thread UDPThread;
        TcpListener TCPListener = null;

        /// <summary>
        /// Starts the server. The server then starts two threads to listen for connections.
        /// One for TCP and sending game information, and one for UDP for discovery.
        /// </summary>
        public void StartServer(int tcpPort, int udpPort)
        {
            //Creates a client list.
            Clients = new List<SocketHelper>();
            
            //Starts a listen thread to listen for connections.
            TCPThread = new Thread(new ThreadStart(delegate
            {
                Listen(tcpPort);
            }));
            TCPThread.Start();

            //Starts an UDP listen port to listen for clients.
            UDPThread = new Thread(new ThreadStart(delegate
            {
                UDPListen(udpPort);
            }));
            UDPThread.Start();
        }

        /// <summary>
        /// Stops the server from running.
        /// </summary>
        public void StopServer()
        {
            TCPThread.Abort();
            UDPThread.Abort();
            TCPThread = null;
            UDPThread = null;

            foreach (SocketHelper client in Clients)
            {
                client.ReadThread.Abort();
                client.WriteThread.Abort();
                client.ReadThread = null;
                client.WriteThread = null;
            }

            TCPListener.Stop();
            Clients = null;
        }

        /// <summary>
        /// Sends a message out to all connected clients.
        /// </summary>
        public void SendMessageToClients(string message)
        {
            foreach (SocketHelper client in Clients)
            {
                client.SendMessage(message);
            }
        }

        private void UDPListen(int port)
        {
            UdpClient UDPListener = new UdpClient(port);

            while (true)
            {
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] clientRequestData = UDPListener.Receive(ref clientEndpoint);
                string clientRequest = Encoding.ASCII.GetString(clientRequestData);

                if (clientRequest == "Qviz")
                {
                    IPAddress ipAddress = Dns.GetHostEntry(String.Empty).AddressList[0];
                    Lobby lobby = new Lobby(ipAddress.ToString(), ContentController.GetInstance().Name, Clients.Count);
                    string json = JsonConvert.SerializeObject(lobby);
                    byte[] response = Encoding.ASCII.GetBytes(json);
                    UDPListener.Send(response, response.Length, clientEndpoint);
                }
            }
        }

        /// <summary>
        /// Listens for clients and starts threads to handle them.
        /// </summary>
        private void Listen(int port)
        {
            IPAddress ipAddress = Dns.GetHostEntry(String.Empty).AddressList[0];
            TCPListener = new TcpListener(ipAddress, port);
            TCPListener.Start();

            while (true)
            {
                Thread.Sleep(10);
                TcpClient tcpClient = TCPListener.AcceptTcpClient();
                SocketHelper helper = new SocketHelper();
                helper.StartClient(tcpClient);
                Clients.Add(helper);
            }
        }

        private class SocketHelper
        {
            MultiplayerController MultiplayerCtr = MultiplayerController.GetInstance();
            Queue<string> WriteQueue = new Queue<string>();
            public Thread ReadThread;
            public Thread WriteThread;
            TcpClient mscClient;
            string mstrMessage;
            string mstrResponse;

            /// <summary>
            /// Starts the given client in two ned threads for reading and writing.
            /// </summary>
            public void StartClient(TcpClient client)
            {
                //Starts a read thread.
                ReadThread = new Thread(new ThreadStart(delegate
                {
                    Read(client);
                }));
                ReadThread.Start();


                //Starts a write thread.
                WriteThread = new Thread(new ThreadStart(delegate
                {
                    Write(client);
                }));
                WriteThread.Start();
            }

            /// <summary>
            /// Sends a string message to the client. This message is added to the write queue and send
            /// once it is it's turn.
            /// </summary>
            public void SendMessage(String message)
            {
                WriteQueue.Enqueue(message);
            }


            /// <summary>
            /// Writes data to the client in sequence on the server.
            /// </summary>
            public void Write(TcpClient client)
            {
                while (true)
                {
                    Thread.Sleep(10);
                    if (WriteQueue.Count != 0)
                    {
                        String message = WriteQueue.Dequeue();
                        byte[] bytesSent = new byte[256];
                        NetworkStream stream = client.GetStream();
                        bytesSent = Encoding.ASCII.GetBytes(message);
                        stream.Write(bytesSent, 0, bytesSent.Length);
                    }
                }
            }

            /// <summary>
            /// Reads data from the client and sends back a response.
            /// </summary>
            public void Read(TcpClient client)
            {
                while (true) 
                {
                    Thread.Sleep(10);
                    byte[] bytesReceived = new byte[256]; 
                    NetworkStream stream = client.GetStream();
                    stream.Read(bytesReceived, 0, bytesReceived.Length);
                    mstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
                    mscClient = client;
                    
                    string command = mstrMessage.Substring(0, 4);

                    switch (command)
                    {
                        //Player connects/joins lobby.
                        case "CMD1":
                            var data = new Tuple<List<Player>, List<Question>, GamePack>
                            (
                                MultiplayerCtr.Players,
                                MultiplayerCtr.Questions,
                                MultiplayerCtr.GamePack
                            );
                            mstrResponse = "CMD1" + JsonConvert.SerializeObject(data);
                            break;

                        //Player disconnects/leaves lobby/game.
                        case "CMD2":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.GamePack);
                            break;

                        //Player answers a question.
                        case "CMD3":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.Players);
                            break;
                    }

                    SendMessage(mstrResponse);
                }
            }
        }
    }
}