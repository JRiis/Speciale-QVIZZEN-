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
        public string Hostname;
        public int Count;

        public Lobby(string hostname, int count)
        {
            Hostname = hostname;
            Count = count;
        }
    }
}