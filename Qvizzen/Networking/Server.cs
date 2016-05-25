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
        Thread PingThread;
        TcpListener TCPListener = null;

        private const int BufferSize = 256000;
        public const char Delimiter = '\0';

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

            //Starts a ping thread to keep connection alive.
            PingThread = new Thread(new ThreadStart(delegate
            {
                Ping();
            }));
            PingThread.Start();
        }

        /// <summary>
        /// Stops the server from running.
        /// </summary>
        public void StopServer()
        {
            TCPThread.Abort();
            PingThread.Abort();
            StopUDPListen();

            foreach (SocketHelper client in Clients)
            {
                client.MscClient.GetStream().Close();
                client.MscClient.Close();
                client.ReadThread.Abort();
                client.WriteThread.Abort();
            }

            TCPListener.Stop();
            Clients.Clear();
        }

        /// <summary>
        /// Stops the UDPListen thread.
        /// </summary>
        public void StopUDPListen()
        {
            try
            {
                UDPThread.Abort();
            }
            catch (NullReferenceException ex)
            {
                //Do Nothing.
            }
        }


        /// <summary>
        /// Constantly pings clients with messages to see if they disconnect.
        /// </summary>
        private void Ping()
        {
            string message = JsonConvert.SerializeObject(new List<string>() 
            {
                "Ping", 
            });

            while (true)
            {
                Thread.Sleep(3000);
                SendMessageToClients(message);
            } 
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

        /// <summary>
        /// Listen for broadcasts on the UDPPort and responds with a lobby list for qvizzen players.
        /// </summary>
        /// <param name="port"></param>
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
                    Lobby lobby = new Lobby(ipAddress.ToString(), ContentController.GetInstance().Name, MultiplayerController.GetInstance().Players.Count);
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
            public TcpClient MscClient;
            List<string> MstrMessage;
            string MstrResponse;
            string ClientIPAddress;

            /// <summary>
            /// Starts the given client in two ned threads for reading and writing.
            /// </summary>
            public void StartClient(TcpClient client)
            {
                //Sets client variable.
                MscClient = client;
                MscClient.SendBufferSize = BufferSize;
                MscClient.ReceiveBufferSize = BufferSize;
                
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
            /// Disconnects the client from the server and stops all threads for client.
            /// </summary>
            public void DisconnectClient()
            {                                
                //Removes client from server.
                MultiplayerCtr.Server.Clients.Remove(this);
                MscClient.GetStream().Close();
                MscClient.Close();
                
                //Checks if ingame.
                if (MultiplayerCtr.IsIngame)
                {
                    //Sets player as disconnected on player list.
                    Player disconnectedPlayer = null;
                    foreach (Player player in MultiplayerCtr.Players)
                    {
                        if (player.IPAddress == ClientIPAddress)
                        {
                            player.IsConnected = false;
                            disconnectedPlayer = player;
                            break;
                        }
                    }
                }
                
                //Else must be in lobby.
                else
                {
                    //Removes player from player list.
                    foreach (Player player in MultiplayerCtr.Players)
                    {
                        if (player.IPAddress == ClientIPAddress)
                        {
                            MultiplayerCtr.Players.Remove(player);
                            break;
                        }
                    }
                }

                //Sends updated playerlist to all connected players.
                string message = JsonConvert.SerializeObject(new List<string>() 
                {
                    "UpdatePlayerList", 
                    JsonConvert.SerializeObject(MultiplayerCtr.Players)
                });

                MultiplayerCtr.Server.SendMessageToClients(message);
                MultiplayerCtr.UpdateAdapter();

                //Stops Threads
                ReadThread.Abort();
                WriteThread.Abort();
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
                        try
                        {
                            String message = WriteQueue.Dequeue();
                            byte[] bytesSent = new byte[message.Length + 1];
                            NetworkStream stream = client.GetStream();
                            bytesSent = Encoding.ASCII.GetBytes(message);
                            bytesSent[bytesSent.Length - 1] = Encoding.ASCII.GetBytes(new char[] { Delimiter })[0];
                            stream.Write(bytesSent, 0, bytesSent.Length);
                        }
                        catch (System.IO.IOException ex)
                        {
                            DisconnectClient();
                            Console.WriteLine("Server Write Error" + ex.Message);
                            break;
                        }
                        catch (ObjectDisposedException)
                        {
                            //Do nothing
                        }
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
                    try
                    {
                        Thread.Sleep(10);
                        byte[] bytesReceived = new byte[BufferSize];
                        NetworkStream stream = client.GetStream();
                        stream.Read(bytesReceived, 0, bytesReceived.Length);
                        string json = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
                        MstrMessage = JsonConvert.DeserializeObject<List<string>>(json);

                        string command = MstrMessage[0];

                        string defaultAnswer = JsonConvert.SerializeObject(new List<string>() 
                        {
                            "Default",
                        });

                        switch (command)
                        {
                            //Player connects/joins lobby.
                            case "Connect":
                                MultiplayerCtr.AddPlayer(MstrMessage[1], MstrMessage[2], false);
                                MstrResponse = JsonConvert.SerializeObject(new List<string>() 
                                {
                                    "Connect", 
                                    JsonConvert.SerializeObject(MultiplayerCtr.Players),
                                    JsonConvert.SerializeObject(MultiplayerCtr.Questions),
                                    JsonConvert.SerializeObject(MultiplayerCtr.GamePack)
                                });

                                ClientIPAddress = MstrMessage[1];
                                MultiplayerCtr.UpdateAdapter();
                                break;

                            //Player disconnects/leaves lobby/game.
                            case "RageQuit":
                                DisconnectClient();
                                MstrResponse = defaultAnswer;
                                break;

                            //Player answers a question.
                            case "Answer":
                                string message = JsonConvert.SerializeObject(new List<string>() 
                                {
                                    "Answer",
                                    MstrMessage[1],
                                    MstrMessage[2]
                                });
                                MultiplayerCtr.Server.SendMessageToClients(message);
                                MultiplayerCtr.AnwserQuestionActivity(int.Parse(MstrMessage[1]));
                                MstrResponse = defaultAnswer;
                                break;

                            default:
                                MstrResponse = defaultAnswer;
                                break;
                        }

                        SendMessage(MstrResponse);
                    }
                    catch (System.IO.IOException ex)
                    {
                        DisconnectClient();
                        Console.WriteLine("Server Read Error" + ex.Message);
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        //Do nothing.
                    }
                }
            }
        }
    }
}