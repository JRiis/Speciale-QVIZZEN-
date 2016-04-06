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
        public const int QuestionValue = 1;

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
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, Score, 1, Questions.Count);
        }

        
        /// <summary>
        /// Advances the gameplay a turn.
        /// </summary>
        public void NextTurn()
        {
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, Score, CurrentIndex, Questions.Count);
        }


        /// <summary>
        /// Checks if anwser is correct and updates score accordingly. 
        /// </summary>
        public bool AnwserQuestion(Anwser anwser)
        {
            if (anwser.IsCorrect)
            {
                //Correct Anwser
                Score += QuestionValue;
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

            //TODO: Check for final question.

            return question;
        }




    }
}