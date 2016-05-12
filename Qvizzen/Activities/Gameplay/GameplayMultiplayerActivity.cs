using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Qvizzen.Controller;
using Qvizzen.Adapters;
using Qvizzen.Activities;
using Android.Util;
using Android.Content.PM;
using AndroidSwipeLayout;
using Qvizzen.Extensions;

namespace Qvizzen
{
    [Activity(Label = "GameplayMultiplayerActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameplayMultiplayerActivity : GameplayParentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Gameplay);

            //Setup Controller
            GameplayCtr = MultiplayerController.GetInstance();

            //Setup content adapter for list.
            ListView listScore = FindViewById<ListView>(Resource.Id.listViewScore);
            ScoreAdapter = new ScoreAdapter(this, GameplayCtr.Players);
            listScore.Adapter = Adapter;

            //Setup Swipe Layout
            var swipeLayout = FindViewById<SwipeLayout>(Resource.Id.swipeLayout1);
            swipeLayout.SetShowMode(SwipeLayout.ShowMode.PullOut);
            var scorescreenView = FindViewById(Resource.Id.linearLayoutScorescreen);
            swipeLayout.AddDrag(SwipeLayout.DragEdge.Right, scorescreenView);

            //Starts Gameplay
            GameplayCtr.StartGame(this);
        }

        /// <summary>
        /// Answers the question at given position. Is used for multiplayer when other players answer questions.
        /// </summary>
        public void AnswerQuestion(int position)
        {
            //Finds answer.
            ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
            View view = listAnwsers.GetItemAtPosition(position).JavaCast<View>();
            Anwser item = ExtensionMethods.Cast<Anwser>(listAnwsers.Adapter.GetItem(position));    

            //Updates Variables.
            CanClick = false;
            CountdownTimer.Stop();

            //Checks if correct anwser.
            TextView questionLabel = FindViewById<TextView>(Resource.Id.textViewQuestion);

            if (item.IsCorrect)
            {
                questionLabel.Text = "Correct!";
                var color = new Android.Graphics.Color(50, 237, 50, 255);
                view.SetBackgroundColor(color);
            }
            else
            {
                questionLabel.Text = "Incorrect!";
                var color = new Android.Graphics.Color(237, 50, 50, 255);
                view.SetBackgroundColor(color);
            }

            //Starts Anwser Timer
            AnwserTimer = new Timer();
            AnwserTimer.Interval = AnwserTime;
            AnwserTimer.Elapsed += AnwserTimerTickEvent;
            AnwserTimer.Enabled = true;
            AnwserTimer.AutoReset = false;
        }
    }
}