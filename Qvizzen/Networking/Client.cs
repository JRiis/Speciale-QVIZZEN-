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
        MultiplayerController MultiplayerCtr;

        public Client()
        {
            WriteQueue = new Queue<string>();
            MultiplayerCtr = MultiplayerController.GetInstance();
        }

        /// <summary>
        /// Connects the client to the server on given ipaddress and port.
        /// </summary>
        public void Connect(string serverIP, int port)
        {
            TCPClient = new TcpClient(serverIP, port);
            
            ReadThread = new Thread(ReadTCP);
            ReadThread.Start();
            
            WriteThread = new Thread(WriteTCP);
            WriteThread.Start();
        }

        /// <summary>
        /// Disconnects client from server.
        /// </summary>
        public void Disconnect()
        {
            TCPClient.Close();
            
            ReadThread.Abort();
            ReadThread = null;

            WriteThread.Abort();
            WriteThread = null;
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
        public void Broadcast(int port)
        {
            UDPClient = new UdpClient();
            Endpoint = new IPEndPoint(IPAddress.Any, 0);
            UDPClient.EnableBroadcast = true;
            byte[] broadcast = Encoding.ASCII.GetBytes("Qviz");
            UDPClient.Send(broadcast, broadcast.Length, new IPEndPoint(IPAddress.Broadcast, port));
            UDPReadThread = new Thread(new ThreadStart( delegate 
            { 
                ReadUDP(port);
            }));
            UDPReadThread.Start();
        }

        /// <summary>
        /// Stops broadcasting for server lists. Should be called once connecting to a client.
        /// </summary>
        public void StopBroadcast()
        {
            UDPReadThread.Abort();
            UDPReadThread = null;
        }

        /// <summary>
        /// Reads data from the server and handles commands in the switch.
        /// </summary>
        public void ReadTCP()
        {
            while (true)
            {
                NetworkStream stream = TCPClient.GetStream();
                Byte[] ReciveData = new byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(ReciveData, 0, ReciveData.Length);
                responseData = System.Text.Encoding.ASCII.GetString(ReciveData, 0, ReciveData.Length);

                string command = responseData.Substring(0, 4);

                switch (command)
                {
                    //Player connects/joins lobby.
                    case "CMD1":
                        string json = responseData.Substring(5);
                        var data = JsonConvert.DeserializeObject<Tuple<List<Player>, List<Question>, GamePack>>(json);
                        
                        MultiplayerCtr.Players = data.Item1;
                        MultiplayerCtr.Questions = data.Item2;
                        MultiplayerCtr.GamePack = data.Item3;
                        
                        MultiplayerCtr.JoinLobby();
                        break;

                    //Player disconnects/leaves lobby/game.
                    case "CMD2":
                        break;

                    //Player answers a question.
                    case "CMD3":
                        break;
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
                    String message = WriteQueue.Dequeue();
                    Byte[] SendData = new Byte[256];
                    SendData = System.Text.Encoding.ASCII.GetBytes(message);
                    NetworkStream stream = TCPClient.GetStream();
                    stream.Write(SendData, 0, SendData.Length);
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
                byte[] reciveData = UDPClient.Receive(ref Endpoint);
                string responseData = System.Text.Encoding.ASCII.GetString(reciveData, 0, reciveData.Length);
                Lobby lobby = JsonConvert.DeserializeObject<Lobby>(responseData);

                foreach (Lobby check in MultiplayerCtr.Lobbies)
                {
                    if (check.IPAddress == lobby.IPAddress)
                    {
                        continue;
                    }
                }

                MultiplayerCtr.Lobbies.Add(lobby);
                MultiplayerCtr.UpdateAdapter();
            }
        }
    }
}