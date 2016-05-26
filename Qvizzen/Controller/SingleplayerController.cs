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

        /// <summary>
        /// Starts gameplay on current gameplay activity.
        /// </summary>
        /// <param name="activity">GameplayActivity</param>
        public override void StartGame(GameplayParentActivity activity)
        {   
            //Creates a singleplayer player instance.
            Player newPlayer = new Player(ContentController.GetInstance().IPAddress, "Your Score", true);
            Players.Add(newPlayer);
            
            //Setup Variables
            FinalQuestion = false;
            CurrentIndex = 0;
            PlayerIndex = 0;
            Activity = activity;
            CurrentPlayer = GetNextPlayer();
            foreach (Player player in Players)
            {
                player.Score = 0;
            }

            //Update GUI
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, CurrentPlayer.Score, 1, Questions.Count);
        }
    }
}