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

namespace Qvizzen.Networking
{
    public class Server
    {
        List<SocketHelper> Clients = new List<SocketHelper>();
        Thread TCPThread;
        Thread UDPThread;
        TcpListener TCPListener = null;
        bool InGame = false;

        /// <summary>
        /// Starts the server. The server then starts two threads to listen for connections and
        /// one to broadcast to the network for clients to discover the server.
        /// </summary>
        public void StartServer(int tcpPort, int udpPort)
        {
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

        /// <summary>
        /// Stops the server from running.
        /// </summary>
        public void StopServer()
        {
            TCPListener.Stop();
        }

        private class SocketHelper
        {
            MultiplayerController MultiplayerCtr = MultiplayerController.GetInstance();
            Queue<string> WriteQueue = new Queue<string>();
            Thread ReadThread;
            Thread WriteThread;
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

                    String command;
                    if (mstrMessage.Length < 6)
                    {
                        command = mstrMessage;
                    }
                    else
                    {
                        command = mstrMessage.Substring(0, 6);
                    }

                    switch (command)
                    {
                        case "Qviz":
                            mstrResponse = "True";
                            break;
                    
                        case "GLobby":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.Players);
                            break;
                    
                        case "GPacks":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.GamePack);
                            break;

                        case "GQList":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.Questions);
                            break;

                        case "Status":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerCtr.Server.InGame);
                            break;

                        case "JLobby":

                            //string playername = 

                            ////Check if string anwser.
                            //if (mstrMessage.Substring(0, 5) == "Anwser")
                            //{
                            //    //TODO: Funtimes...
                            //}

                            //MultiplayerCtr.Players.Add(new Model.Player(playername, false));
                    
                    
                        //TODO


                            break;

                        case "LLobby":
                            //TODO
                            break;

                        case "LeaveGame":
                            //TODO
                            break;

                        case "Anwser":

                            //TODO: how to receive anwsers?



                            break;
                    }

                    SendMessage(mstrResponse);
                }
            }
        }
    }
}