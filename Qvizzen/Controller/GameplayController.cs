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
    public class GameplayController
    {
        public GamePack GamePack;
        public GameplayParentActivity Activity;
        public List<Question> Questions;
        public int CurrentIndex;
        public bool FinalQuestion;
        public List<Player> Players;
        public Player CurrentPlayer;
        public int PlayerIndex;
        public bool IsIngame;

        public const int DefaultTimer = 20;
        public const int QuestionValue = 1;
        
        /// <summary>
        /// Unwraps the current GamePack on controller to a list of questions and
        /// randomizes their order in the list.
        /// </summary>
        public void SetupGamePack()
        {
            Questions = new List<Question>();
            foreach (Pack pack in GamePack.Packs)
            {
                foreach (Question question in pack.Questions)
                {
                    Questions.Add(question);
                }
            }
            Questions.Shuffle();
        }

        /// <summary>
        /// Starts gameplay on current gameplay activity.
        /// </summary>
        /// <param name="activity">GameplayActivity</param>
        public virtual void StartGame(GameplayParentActivity activity)
        {
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

        /// <summary>
        /// Gets the next player and updates the playerindex for current player. Resets player index
        /// to 0, if exceeds the amount of players in the game, essentially looping through players.
        /// If a player is disconnected skips that player.
        /// </summary>
        /// <returns>Player</returns>
        public Player GetNextPlayer()
        {
            Player player = Players[PlayerIndex];
            PlayerIndex += 1;
            if (PlayerIndex == Players.Count)
            {
                PlayerIndex = 0;
            }
            if (!player.IsConnected)
            {
                player = GetNextPlayer();
            }
            return player;
        }

        /// <summary>
        /// Returns a boolean indicating wether player is current player.
        /// </summary>
        public bool IsYourTurn()
        {
            bool boolean = false;
            if (CurrentPlayer.IPAddress == ContentController.GetInstance().IPAddress)
            {
                boolean = true;
            }
            return boolean;
        }

        /// <summary>
        /// Advances the gameplay a turn.
        /// </summary>
        public void NextTurn()
        {
            CurrentPlayer = GetNextPlayer();
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, CurrentPlayer.Score, CurrentIndex, Questions.Count);
        }


        /// <summary>
        /// Checks if anwser is correct and updates score accordingly. 
        /// </summary>
        public virtual bool AnwserQuestion(Anwser anwser, int position)
        {
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
        /// Gets the next question from all questions and updates current index.
        /// </summary>
        /// <returns>Question</returns>
        public Question GetQuestion()
        {
            Question question = Questions[CurrentIndex];
            CurrentIndex++;

            if (CurrentIndex == Questions.Count)
            {
                FinalQuestion = true;
            }

            return question;
        }


        /// <summary>
        /// Handles cleaing up of variables to reset for a new game.
        /// </summary>
         public virtual void EndGame()
        {
            IsIngame = false;
            Players.Clear();
        }
    }
}