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
    public class Player
    {
        public string IPAddress;
        public string Name;
        public int Score;
        public bool Host;
        public bool IsConnected;

        public Player(string ipAddress, string name, bool host)
        {
            IPAddress = ipAddress;
            Name = name;
            Host = host;
            Score = 0;
            IsConnected = true;
        }
    }
}