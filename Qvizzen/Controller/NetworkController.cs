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

namespace Qvizzen.Controller
{
    class NetworkController
    {
        private class Server
        {
            static string output = "";

            public void CreateListener()
            {
                TcpListener tcpListener = null;
                IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

                try
                {
                    tcpListener = new TcpListener(ipAddress, 13);
                    tcpListener.Start();
                    output = "Waiting for a connection...";
                }
                catch (Exception e)
                {
                    output = "Error " + e.ToString();
                }

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

                    //Ligegyldig
                    mstrMessage = mstrMessage.Substring(0, 5);
                    if (mstrMessage.Equals("Hello"))
                    {
                        mstrResponse = "Goodye";
                    }
                    else
                    {
                        mstrResponse = "What?";
                    }

                    bytesSent = Encoding.ASCII.GetBytes(mstrResponse);
                    stream.Write(bytesSent, 0, bytesSent.Length);
                }

            }
        }

        private class Client
        {




        }


    }
}