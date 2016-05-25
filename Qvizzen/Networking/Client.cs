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
using Newtonsoft.Json;
using Qvizzen.Controller;
using Qvizzen.Model;
using System.Net;

using Qvizzen.Extensions;
using System.Threading;

namespace Qvizzen.Networking
{
    public class Client
    {
        TcpClient TCPClient;
        UdpClient UDPClient;
        Thread ReadThread;
        Thread UDPReadThread;
        IPEndPoint Endpoint;
        Queue<string> WriteQueue;
        Thread WriteThread;
        Thread BroadcastThread;
        Thread PingThread;
        public MultiplayerController MultiplayerCtr;

        private const int BufferSize = 256000;
        private const char Delimiter = '\0';

        public Client()
        {
            WriteQueue = new Queue<string>();
        }

        /// <summary>
        /// Connects the client to the server on given ipaddress and port.
        /// </summary>
        public void Connect(string serverIP, int port)
        {
            try
            {
                TCPClient = new TcpClient(serverIP, port);
                TCPClient.SendBufferSize = BufferSize;
                TCPClient.ReceiveBufferSize = BufferSize;

                ReadThread = new Thread(ReadTCP);
                ReadThread.Start();

                WriteThread = new Thread(WriteTCP);
                WriteThread.Start();

                PingThread = new Thread(Ping);
                PingThread.Start();

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                //Host not connected lost conenction something oh noes!!
                foreach (Lobby lobby in MultiplayerCtr.Lobbies)
                {
                    if (lobby.IPAddress == serverIP)
                    {
                        MultiplayerCtr.Lobbies.Remove(lobby);
                        break;
                    }
                }
                MultiplayerCtr.UpdateAdapter();
            }
        }

        /// <summary>
        /// Disconnects client from server.
        /// </summary>
        public void Disconnect()
        {            
            ReadThread.Abort();
            WriteThread.Abort();
            PingThread.Abort();

            WriteThread = null;
            ReadThread = null;
            PingThread = null;

            TCPClient.GetStream().Close();
            TCPClient.Close();
        }

        /// <summary>
        /// Adds a message to the message queue to be send to server.
        /// </summary>
        public void SendMessage(string message)
        {
            WriteQueue.Enqueue(message);
        }

        /// <summary>
        /// Broadcasts on the local network to retrive a list of all servers hosting Qvizzen.
        /// </summary>
        public void StartBroadcast(int port)
        {
            UDPClient = new UdpClient();
            Endpoint = new IPEndPoint(IPAddress.Any, 0);
            UDPClient.EnableBroadcast = true;

            BroadcastThread = new Thread(new ThreadStart(delegate
            {
                Broadcast(port);
            }));
            BroadcastThread.Start();

            UDPReadThread = new Thread(new ThreadStart( delegate 
            { 
                ReadUDP(port);
            }));
            UDPReadThread.Start();
        }

        /// <summary>
        /// Handles broadcasting.
        /// </summary>
        private void Broadcast(int port)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                    byte[] broadcast = Encoding.ASCII.GetBytes("Qviz");
                    UDPClient.Send(broadcast, broadcast.Length, new IPEndPoint(IPAddress.Broadcast, port));
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    //TODO: Error message or something? Not connected to internet error.
                    continue;
                }
                catch (System.ObjectDisposedException ex)
                {
                    //Expected when broadcast is closing. Do nothing.
                }
            }
        }

        /// <summary>
        /// Stops broadcasting for server lists. Should be called once connecting to a client.
        /// </summary>
        public void StopBroadcast()
        {
            try
            {
                UDPReadThread.Abort();
                //UDPReadThread = null;

                BroadcastThread.Abort();
                //BroadcastThread = null;

                UDPClient.Close();
                //UDPClient = null;
            }
            catch (System.NullReferenceException ex)
            {
                //Do Nothing
            }
        }


        /// <summary>
        /// Constantly pings the host to check for active connection.
        /// </summary>
        public void Ping()
        {
            string message = JsonConvert.SerializeObject(new List<string>() 
            {
                "Ping", 
            });

            while (true)
            {
                Thread.Sleep(3000);
                SendMessage(message);
            } 
        }

        /// <summary>
        /// Reads data from the server and handles commands in the switch.
        /// </summary>
        public void ReadTCP()
        {
            Byte[] LeftOverBuffer = new byte[BufferSize];
            
            while (true)
            {
                try
                {
                    Thread.Sleep(10);
                    Byte[] ReciveData = new byte[BufferSize];
                    NetworkStream stream = TCPClient.GetStream();
                    GetMessageFromStream(stream, ref ReciveData, Delimiter, LeftOverBuffer);
                    //stream.Read(ReciveData, 0, ReciveData.Length);
                    string json = System.Text.Encoding.ASCII.GetString(ReciveData, 0, ReciveData.Length);
                    json += "]";
                    List<string> message = JsonConvert.DeserializeObject<List<string>>(json);

                    string command = message[0];

                    switch (command)
                    {
                        //Player connects/joins lobby.
                        case "Connect":
                            MultiplayerCtr.Players = JsonConvert.DeserializeObject<List<Player>>(message[1]);
                            MultiplayerCtr.Questions = JsonConvert.DeserializeObject<List<Question>>(message[2]);
                            MultiplayerCtr.GamePack = JsonConvert.DeserializeObject<GamePack>(message[3]);

                            var ctr = ContentController.GetInstance();
                            foreach (Pack newPack in JsonConvert.DeserializeObject<GamePack>(message[3]).Packs)
                            {
                                foreach (Pack pack in ctr.Content)
                                {
                                    if (newPack.Name == pack.Name)
                                    {
                                        ctr.Content.Remove(pack);
                                        break;
                                    }
                                }

                                ctr.Content.Add(newPack);
                            }

                            MultiplayerCtr.JoinLobby();
                            break;

                        //Player update list for lobby.
                        case "UpdatePlayerList":
                            string textData2 = message[1];
                            var data2 = JsonConvert.DeserializeObject<List<Player>>(textData2);
                            MultiplayerCtr.Players = data2;
                            MultiplayerCtr.UpdateAdapter();
                            break;

                        //Player recives game have started.
                        case "Start":
                            MultiplayerCtr.StartMultiplayerGame();
                            break;

                        //Player answers a question.
                        case "Answer":
                            if (ContentController.GetInstance().IPAddress != message[2])
                            {
                                MultiplayerCtr.AnwserQuestionActivity(int.Parse(message[1]));
                            }
                            break;

                        //Host stops the game.
                        case "Unhost":
                            MultiplayerCtr.FinishActivity();
                            MultiplayerCtr.Joining = false;
                            break;

                        //Ping
                        case "Ping":
                            //Do Nothing...
                            break;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    Console.WriteLine("Client Read Exception: " + ex.Message);
                    MultiplayerCtr.FinishActivity();
                    MultiplayerCtr.Joining = false;
                    Disconnect();
                    break;
                }
                catch (ObjectDisposedException ex)
                {
                    //Do nothing.
                }
            }
        }

        /// <summary>
        /// Sends data to the currently connected server from the message queue.
        /// </summary>
        public void WriteTCP()
        {
            while (true)
            {
                Thread.Sleep(10);
                if (WriteQueue.Count != 0)
                {
                    try
                    {
                        String message = WriteQueue.Dequeue();
                        Byte[] SendData = new Byte[BufferSize];
                        SendData = System.Text.Encoding.ASCII.GetBytes(message);
                        NetworkStream stream = TCPClient.GetStream();
                        stream.Write(SendData, 0, SendData.Length);

                        //Determines message content to check for special case.
                        string command = JsonConvert.DeserializeObject<List<string>>(message)[0];
                        switch (command)
                        {
                            case "RageQuit":
                                Disconnect();
                                break;
                        }
                    }
                    catch (System.IO.IOException ex)
                    {
                        //Finishes activtity.
                        Console.WriteLine("Client Write Exception: " + ex.Message);
                        MultiplayerCtr.FinishActivity();
                        Disconnect();
                        break;
                    }
                    catch (ObjectDisposedException ex)
                    {
                        //Do nothing.
                    }
                }
            }
        }


        /// <summary>
        /// Reads data from hosts via udp and creates lobbies for the controller.
        /// </summary>
        public void ReadUDP(int port)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(10);
                    byte[] reciveData = UDPClient.Receive(ref Endpoint);
                    string responseData = System.Text.Encoding.ASCII.GetString(reciveData, 0, reciveData.Length);
                    Lobby lobby = JsonConvert.DeserializeObject<Lobby>(responseData);

                    foreach (Lobby check in MultiplayerCtr.Lobbies)
                    {
                        if (check.IPAddress == lobby.IPAddress)
                        {
                            MultiplayerCtr.Lobbies.Remove(check);
                            break;
                        }
                    }

                    MultiplayerCtr.Lobbies.Add(lobby);
                    MultiplayerCtr.UpdateAdapter();
                }
                catch (Exception ex)
                {
                    //gg
                }
            }
        }


        /// <summary>
        /// Blocking method that returns a complete message from the network stream. Message is considered complete
        /// after the given delimiter. Leftover from that read operation is stored in leftover array which is reused,
        /// upon next method call to ensure no data is lost.
        /// </summary>
        /// <param name="stream">Network stream of the TcpClient.</param>
        /// <param name="buffer">Reference to the buffer to store complete message.</param>
        /// <param name="deli">Deliminater character used for message end.</param>
        /// <param name="leftover">Buffer to store and reuse leftover data from read operations.</param>
        private void GetMessageFromStream(NetworkStream stream, ref Byte[] buffer, char deli, Byte[] leftover)
        {
            try
            {
                //Gets data from the leftover buffer.
                int size = leftover.Count(s => s != 0);
                Array.Copy(leftover, 0, buffer, 0, size);
                int readSoFar = size;

                //Continuesly reads until a deliminiter is found.
                bool messageComplete = false;
                while (!messageComplete)
                {
                    var read = stream.Read(buffer, readSoFar, buffer.Length - readSoFar);
                    var chars = System.Text.Encoding.ASCII.GetChars(buffer, readSoFar, buffer.Length - readSoFar);

                    //Checks for delimiter.
                    for (int i = 0; i <= chars.Length; i++)
                    {
                        if (chars[i] == deli)
                        {
                            messageComplete = true;
                            if (i != chars.Length) //Note, never true.
                            {
                                //Stores leftover from read operation in the leftover buffer.
                                Array.Copy(buffer, i, leftover, 0, i);
                            }

                            //Resizes the messsage buffer to size of message.
                            Array.Resize<Byte>(ref buffer, i);
                            break;
                        }
                    }

                    readSoFar += read;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}