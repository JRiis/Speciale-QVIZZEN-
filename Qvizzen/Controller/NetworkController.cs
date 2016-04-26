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

namespace Qvizzen.Controller
{
    class NetworkController
    {
        private static NetworkController Instance;

        private class Server
        {
            public void CreateListener()
            {
                TcpListener tcpListener = null;
                IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
                tcpListener = new TcpListener(ipAddress, 13);
                tcpListener.Start();

                while (true)
                {
                    Thread.Sleep(10);
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    byte[] bytes = new byte[256];
                    NetworkStream stream = tcpClient.GetStream();
                    stream.Read(bytes, 0, bytes.Length);
                    SocketHelper helper = new SocketHelper();
                    helper.ProcessMsg(tcpClient, stream, bytes);
                }
            }

            private class SocketHelper
            {
                TcpClient mscClient;
                string mstrMessage;
                string mstrResponse;
                byte[] bytesSent;

                public void ProcessMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
                {
                    mstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
                    mscClient = client;

                    switch (mstrMessage)
                    {
                        case "GetGamePack":
                            mstrResponse = JsonConvert.SerializeObject(MultiplayerController.GetInstance().GamePack);
                            break;

                        case "GetQuestionList":
                            //TODO
                            break;

                        case "SendAnwser":
                            //TODO
                            break;

                        case "JoinLobby":
                            //TODO
                            break;

                        case "LeaveLobby":
                            //TODO
                            break;

                        case "LeaveGame":
                            //TODO
                            break;

                        default:
                            //Nothing
                            break;
                    }

                    bytesSent = Encoding.ASCII.GetBytes(mstrResponse);
                    stream.Write(bytesSent, 0, bytesSent.Length);
                }

            }
        }

        private class Client
        {
            public void Connect(string serverIP, string message)
            {
                Int32 port = 13;
                TcpClient client = new TcpClient(serverIP, port);
                Byte[] SendData = new Byte[256];
                SendData = System.Text.Encoding.ASCII.GetBytes(message);
                NetworkStream stream = client.GetStream();
                stream.Write(SendData, 0, SendData.Length);
                Byte[] ReciveData = new byte[256];
                String reponseData = String.Empty;
                Int32 bytes = stream.Read(ReciveData, 0, ReciveData.Length);
                reponseData = System.Text.Encoding.ASCII.GetString(ReciveData, 0, ReciveData.Length);

                switch (message)
                {
                    case "GetGamePack":
                        GamePack gamePack = JsonConvert.DeserializeObject<GamePack>(reponseData);
                        MultiplayerController.GetInstance().GamePack = gamePack;
                        foreach (Pack pack in gamePack.Packs)
                        {
                            ContentController.GetInstance().Content.Add(pack);
                        }
                        break;

                    case "GetQuestionList":
                        //TODO
                        break;

                    case "SendAnwser":
                        //TODO
                        break;

                    case "JoinLobby":
                        //TODO
                        break;

                    case "LeaveLobby":
                        //TODO
                        break;

                    case "LeaveGame":
                        //TODO
                        break;

                    default:
                        //Nothing
                        break;
                }

                stream.Close();
                client.Close();
            }
        }

        /// <summary>
        /// Constructor for NetworkController
        /// </summary>
        public NetworkController()
        {
            //Nuthin atm
        }

        /// <summary>
        /// Singleton for NetworkController
        /// </summary>
        /// <returns>Instance of controller.</returns>
        public static NetworkController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new NetworkController();
            }
            return Instance;
        }
    }
}