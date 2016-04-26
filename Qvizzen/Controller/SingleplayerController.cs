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

using Qvizzen.Extensions;
using Qvizzen.Activities;
using Qvizzen.Model;

namespace Qvizzen.Controller
{
    public class SingleplayerController : GameplayController
    {
        private static SingleplayerController Instance;
        
        /// <summary>
        /// Constructor for SingleplayerController
        /// </summary>
        public SingleplayerController()
        {
            Players = new List<Player>();
            Player player = new Player("Your Score", true);
            Players.Add(player);
        }

        /// <summary>
        /// Singleton for SingleplayerController
        /// </summary>
        /// <returns>Instance of controller.</returns>
        public static SingleplayerController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SingleplayerController();
            }
            return Instance;
        }
    }
}