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

namespace Qvizzen.Networking
{
    public class Client
    {
        TcpClient TCPClient;

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <param name="serverIP">IPAdress of the server.</param>
        public void Connect(string serverIP, int port)
        {
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
        public List<Lobby> RetriveHosts(int port)
        {
            List<Lobby> lobbies = new List<Lobby>();
            //IPAddress router = ExtensionMethods.GetDefaultGateway();
            //String gateway = ExtensionMethods.StringParseGateway(router.ToString());

            String gateway = "10.28.48.";


            String device = "10.28.53.28";

            Connect(device, port);

            String responseData = SendMessage("GLobby");
            List<Player> players = JsonConvert.DeserializeObject<List<Player>>(responseData);

            string hostname = "";
            int count = 0;

            foreach (Player player in players)
            {
                if (player.Host)
                {
                    hostname = player.Name;
                }
                count++;
            }

            Lobby lobby = new Lobby(device, hostname, count);
            lobbies.Add(lobby);
            Disconnect();

            return lobbies;    










            ////Sends a message to all devices on the network.
            //for (int i=2; i < 254; i++)
            //{
            //    String device = gateway + i.ToString();
            //    Connect(device, port);

            //    try
            //    {
            //        String responseData = SendMessage("Qviz");
            //        if (responseData == "True")
            //        {
            //            responseData = SendMessage("Status");
            //            bool status = JsonConvert.DeserializeObject<bool>(responseData);
            //            if (status)
            //            {
            //                continue;
            //            }

            //            responseData = SendMessage("GLobby");
            //            List<Player> players = JsonConvert.DeserializeObject<List<Player>>(responseData);

            //            string hostname = "";
            //            int count = 0;

            //            foreach (Player player in players)
            //            {
            //                if (player.Host)
            //                {
            //                    hostname = player.Name;
            //                }
            //                count++;
            //            }

            //            Lobby lobby = new Lobby(device, hostname, count);
            //            lobbies.Add(lobby);
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //    }
            //    catch (Exception e) 
            //    {
            //        Console.WriteLine(e.Message);
            //    }

            //    Disconnect();
            //}
            //return lobbies;    
        }

        /// <summary>
        /// Sends the given message to the connected server. Server returns a string appropiate for given message.
        /// </summary>
        public string SendMessage(string message)
        {
            Byte[] SendData = new Byte[256];
            SendData = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = TCPClient.GetStream();
            stream.Write(SendData, 0, SendData.Length);

            Byte[] ReciveData = new byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(ReciveData, 0, ReciveData.Length);
            responseData = System.Text.Encoding.ASCII.GetString(ReciveData, 0, ReciveData.Length);

            return responseData;
        }


        /// <summary>
        /// Reads data from the server and handles commands in the switch.
        /// </summary>
        public void Read()
        {
            while (true)
            {
                NetworkStream stream = TCPClient.GetStream();
                Byte[] ReciveData = new byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(ReciveData, 0, ReciveData.Length);
                responseData = System.Text.Encoding.ASCII.GetString(ReciveData, 0, ReciveData.Length);

                switch (responseData)
                {
                    case "StartGame":
                        //TODO: Start game funtimes yaih!
                        break;
                }


            }
        }











    }
}