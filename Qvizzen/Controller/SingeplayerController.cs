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

namespace Qvizzen.Controller
{
    public class SingeplayerController : GameplayController
    {
        private static SingeplayerController Instance;
        public GameplayActivity Activity;
        public int Score;
        public List<Question> Questions;
        public int CurrentIndex;

        public const int DefaultTimer = 30;
        public const string Playername = "Your Score";

        public static SingeplayerController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SingeplayerController();
            }
            return Instance;
        }

        /// <summary>
        /// Starts gameplay on current gameplay activity.
        /// </summary>
        /// <param name="activity">Activity to handle GUI.</param>
        public void StartGame(GameplayActivity activity)
        {
            //Setup Variables
            Questions = new List<Question>();
            CurrentIndex = 0;
            Activity = activity;
            Score = 0;

            //Unwrap the GamePack and shuffle the Questions.
            foreach (Pack pack in GamePack.Packs)
            {
                foreach (Question question in pack.Questions)
                {
                    Questions.Add(question);
                }
            }
            Questions.Shuffle();

            //Update GUI
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, Score, 0, Questions.Count);
        }


        /// <summary>
        /// Gets the next question from all questions and updates current index.
        /// </summary>
        /// <returns>Question</returns>
        public Question GetQuestion()
        {
            Question question = Questions[CurrentIndex];
            CurrentIndex++;

            //TODO: Check for final question.

            return question;
        }
    }
}