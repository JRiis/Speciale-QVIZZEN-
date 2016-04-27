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
        TcpListener TCPListener = null;
        bool InGame = false;

        /// <summary>
        /// Starts the server and listens for connections.
        /// Returned values can be found in the SocketHelper class.
        /// </summary>
        public void StartServer(int port)
        {
            IPAddress ipAddress = Dns.GetHostEntry(String.Empty).AddressList[0];
            TCPListener = new TcpListener(ipAddress, port);
            TCPListener.Start();

            while (true)
            {
                Thread.Sleep(10);
                TcpClient tcpClient = TCPListener.AcceptTcpClient();
                byte[] bytes = new byte[256];
                NetworkStream stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                SocketHelper helper = new SocketHelper();
                helper.ProcessMsg(tcpClient, stream, bytes);
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
            TcpClient mscClient;
            string mstrMessage;
            string mstrResponse;
            byte[] bytesSent;

            /// <summary>
            /// Processes the message and returns the requested information.
            /// </summary>
            public void ProcessMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
            {
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


                    default:
                        mstrResponse = "Error";
                        break;
                }

                bytesSent = Encoding.ASCII.GetBytes(mstrResponse);
                stream.Write(bytesSent, 0, bytesSent.Length);
            }
        }
    }
}