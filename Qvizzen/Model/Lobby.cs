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

namespace Qvizzen.Model
{
    public class Lobby
    {
        public string IPAddress;
        public string Hostname;
        public int Count;

        public Lobby(string ipAddress, string hostname, int count)
        {
            IPAddress = ipAddress;
            Hostname = hostname;
            Count = count;
        }
    }
}