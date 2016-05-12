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

namespace Qvizzen
{
    [Activity(Label = "GameplayParentActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameplayParentActivity : ParentActivity
    {
        internal GameplayController GameplayCtr;
        internal ScoreAdapter ScoreAdapter;
        internal Timer CountdownTimer;
        internal Timer AnwserTimer;
        internal int DisplayTime;
        internal Question CurrentQuestion;
        internal bool CanClick;
        internal bool IsYourTurn;

        internal const double AnwserTime = 1500;

        public void TimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread( () =>
            {
                //Updates label with timer.
                DisplayTime -= 1;
                TextView timerLabel = FindViewById<TextView>(Resource.Id.textViewTime);
                timerLabel.Text = DisplayTime.ToString();

                //TODO:
                //If timer is low, give feedback to indicate short time remaining, by changing color.

                //If timer is zero.
                if (DisplayTime == 0)
                {
                    //Updates Variables.
                    CanClick = false;
                    CountdownTimer.Stop();
                    TextView questionLabel = FindViewById<TextView>(Resource.Id.textViewQuestion);
                    questionLabel.Text = "Times's Up!";

                    //Setup Anwser Timer
                    AnwserTimer = new Timer();
                    AnwserTimer.Interval = AnwserTime;
                    AnwserTimer.Elapsed += AnwserTimerTickEvent;
                    AnwserTimer.Enabled = true;
                    AnwserTimer.AutoReset = false;
                }
            });
        }

        public void AnwserTimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread( () =>
            {
                //Checks if final question.
                if (GameplayCtr.FinalQuestion)
                {
                    //Ends the game.
                    StartActivity(typeof(ScorescreenActivity));
                }
                else
                {
                    //Advances a turn.
                    GameplayCtr.NextTurn();
                }
                AnwserTimer.Stop();
            });    
        }

        /// <summary>
        /// Updates the GUI and sets it up for the next question.
        /// </summary>
        public void UpdateGUI(Question question, int time, int score, int count, int total)
        {
            //Setup Countdown Timer
            CountdownTimer = new Timer();
            CountdownTimer.Interval = 1000;
            CountdownTimer.Enabled = false;
            CountdownTimer.Elapsed += TimerTickEvent;
            CountdownTimer.Enabled = true;
            CountdownTimer.AutoReset = true;
            
            //Updates Variables
            DisplayTime = time;
            CurrentQuestion = question;
            IsYourTurn = GameplayCtr.IsYourTurn();
            CanClick = IsYourTurn;

            //Updates Labels
            TextView timeLabel = FindViewById<TextView>(Resource.Id.textViewTime);
            timeLabel.Text = time.ToString();
            TextView questionLabel = FindViewById<TextView>(Resource.Id.textViewQuestion);
            questionLabel.Text = question.Text;
            TextView scoreLabel = FindViewById<TextView>(Resource.Id.textViewScore);
            scoreLabel.Text = "Score: " + score.ToString();
            TextView progressLabel = FindViewById<TextView>(Resource.Id.textViewProgress);
            progressLabel.Text = count.ToString() + "/" + total.ToString();

            //Setup content adapter for list.
            ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
            Adapter = new AnwserAdapterGameplay(this, question.Anwsers);
            listAnwsers.Adapter = Adapter;

            //Setup Click Event for List Items.
            listAnwsers.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                //Checks if CanClick.
                if (CanClick)
                {
                    //Updates Variables.
                    CanClick = false;
                    CountdownTimer.Stop();

                    //Checks if correct anwser.
                    if (GameplayCtr.AnwserQuestion(CurrentQuestion.Anwsers[e.Position]))
                    {
                        questionLabel.Text = "Correct!";
                        var color = new Android.Graphics.Color(50, 237, 50, 255);
                        e.View.SetBackgroundColor(color);
                    }
                    else
                    {
                        questionLabel.Text = "Incorrect!";
                        var color = new Android.Graphics.Color(237, 50, 50, 255);
                        e.View.SetBackgroundColor(color);
                    }

                    //Setup Anwser Timer
                    AnwserTimer = new Timer();
                    AnwserTimer.Interval = AnwserTime;
                    AnwserTimer.Elapsed += AnwserTimerTickEvent;
                    AnwserTimer.Enabled = true;
                    AnwserTimer.AutoReset = false;
                }
            };
        }

        protected override void OnStop()
        {
            base.OnStop();
            try
            {
                //Pauses Timers
                CountdownTimer.Stop();
                AnwserTimer.Stop();
            }
            catch (System.NullReferenceException) { }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                //Resumes Timers
                CountdownTimer.Start();
                AnwserTimer.Start();
            }
            catch (System.NullReferenceException) { }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                //Destroys Timers
                CountdownTimer.Dispose();
                AnwserTimer.Dispose();
            }
            catch (System.NullReferenceException) { }
        }
    }
}