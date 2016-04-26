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

namespace Qvizzen.Controller
{
    public class MultiplayerController : GameplayController
    {
        private static MultiplayerController Instance;
        
        /// <summary>
        /// Constructor for MultiplayerController
        /// </summary>
        public MultiplayerController()
        {
            Players = new List<Player>();
        }

        /// <summary>
        /// Singleton for MultiplayerController
        /// </summary>
        /// <returns>Instance of controller.</returns>
        public static MultiplayerController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MultiplayerController();
            }
            return Instance;
        }

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void AddPlayer(string name)
        {
            Players.Add(new Player(name));
        }

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void RemovePlayer(string name)
        {            
            foreach (Player player in Players)
            {
                if (player.Name == name)
                {
                    Players.Remove(player);
                }
            }
        }

        /// <summary>
        /// Checks if anwser is correct and updates score accordingly. 
        /// </summary>
        public new bool AnwserQuestion(Anwser anwser)
        {
            
            
            //TODO: Async send to players.
            
            
            if (anwser.IsCorrect)
            {
                //Correct Anwser
                CurrentPlayer.Score += QuestionValue;
                return true;
            }
            else
            {
                //Wrong Anwser
                return false;
            }
        }

        /// <summary>
        /// Advances the gameplay a turn.
        /// </summary>
        public new void NextTurn()
        {
            
            //TODO: Async send to players.

            //Condition, all players have recived input and responded they have been recived.
            
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, CurrentPlayer.Score, CurrentIndex, Questions.Count);
        }
    }
}