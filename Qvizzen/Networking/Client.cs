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
        public List<Lobby> RetriveHosts()
        {
            List<Lobby> lobbies = new List<Lobby>();
            IPAddress[] hosts = Dns.GetHostEntry(String.Empty).AddressList;
            foreach (IPAddress ip in hosts)
            {
                Connect(ip.ToString());
                String responseData = SendMessage("GetLobbyInfo");
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

                Lobby lobby = new Lobby(ip.ToString(), hostname, count);
                lobbies.Add(lobby);
            }
            return lobbies;
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
            stream.Close();

            return responseData;
        }
    }
}