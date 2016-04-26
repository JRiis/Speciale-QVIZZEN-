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
        public string Name;
        public int Score;
        public bool Host;

        public Player(string name, bool host)
        {
            Name = name;
            Host = host;
            Score = 0;
        }
    }
}