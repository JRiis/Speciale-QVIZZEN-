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
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Qvizzen.Extensions
{
    static class ExtensionMethods
    {
        private static Random rng = new Random();
        
        /// <summary>
        /// Randomizes the order of elements in a List<T>.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Returns the local IPAddress of the machine.
        /// </summary>
        public static IPAddress GetLocalIPAddress()
        {
            IPAddress ipAddress = null;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip;
                    break;
                }
            }
            return ipAddress;
        }

        /// <summary>
        /// Returns the default gateway device is currently connected to.
        /// </summary>
        public static IPAddress GetDefaultGateway()
        {

            NetworkInterface card = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (card == null)
                return null;
            GatewayIPAddressInformation address = card.GetIPProperties().GatewayAddresses.FirstOrDefault();
            if (address == null)
                return null;

            return address.Address;



            //IPAddress gateway = NetworkInterface.GetAllNetworkInterfaces().
            //    Where(e => e.OperationalStatus == OperationalStatus.Up).
            //    SelectMany(e => e.GetIPProperties().GatewayAddresses).FirstOrDefault().Address;
            //return gateway;
        }

        public static string StringParseGateway(string str)
        {
            int length = str.Length;
            int noDots = 0;
            string parsed = "";
            for (int i = 0; i < length; i++ )
            {
                string index = str.ElementAt(i).ToString();
                parsed += index;
                if (index == ".")
                {
                    noDots += 1;
                    if (noDots == 3)
                    {
                        break;
                    }
                }
            }
            return parsed;
        }
    }
}