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

namespace Qvizzen.Networking
{
    public class Client
    {
        TcpClient TCPClient;

        public const int Port = 4444;

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <param name="serverIP">IPAdress of the server.</param>
        public void Connect(string serverIP)
        {
            Int32 port = Port;
            TCPClient = new TcpClient(serverIP, port);
        }

        /// <summary>
        /// Disconnects client from server.
        /// </summary>
        public void Disconnect()
        {
            TCPClient.Close();
        }


        /// <summary>
        /// Retrives a list of hosts on the local network and creates a list of lobbies from it.
        /// </summary>
        public List<Lobby> RetriveHosts
        {


        }

        /// <summary>
        /// Sends the given message to the server. Server returns a string appropiate for given message.
        /// Handling of these returned values can be done in the switch statement.
        /// </summary>
        public void SendMessage(string message)
        {
            Byte[] SendData = new Byte[256];
            SendData = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = TCPClient.GetStream();
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
        }
    }
}