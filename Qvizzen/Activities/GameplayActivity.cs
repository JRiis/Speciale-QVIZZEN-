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

namespace Qvizzen
{    
    [Activity(Label = "GameplayActivity")]
    public class GameplayActivity : ParentActivity
    {
        private SingeplayerController SingleplayerCtr;
        private AnwserAdapterGameplay Adapter;
        private Timer CountdownTimer;
        private Timer AnwserTimer;
        private int DisplayTime;
        private Question CurrentQuestion;
        private bool CanClick;

        private const double AnwserTime = 3000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Gameplay);

            //Setup Controller
            SingleplayerCtr = SingeplayerController.GetInstance();

            //Setup Countdown Timer
            CountdownTimer = new Timer();
            CountdownTimer.Interval = 1000;
            CountdownTimer.Enabled = true;
            CountdownTimer.Elapsed += TimerTickEvent;

            //Setup Anwser Timer
            AnwserTimer = new Timer();
            AnwserTimer.Interval = AnwserTime;
            AnwserTimer.Enabled = true;
            AnwserTimer.Elapsed += AnwserTimerTickEvent;

            //Starts Gameplay
            SingleplayerCtr.StartGame(this);
        }

        public void TimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Updates label with timer.
            DisplayTime -= 1;
            TextView timerLabel = FindViewById<TextView>(Resource.Id.textView2);
            timerLabel.Text = DisplayTime.ToString();

            //If timer is zero.
            if (DisplayTime == 0)
            {
                //TODO: Run time-up event.
                //Stops Timer
            }
        }

        public void AnwserTimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Starts next question.
            AnwserTimer.Stop();
            SingleplayerCtr.NextTurn();
        }

        /// <summary>
        /// Updates the GUI and sets it up for the next question.
        /// </summary>
        public void UpdateGUI(Question question, int time, int score, int count, int total)
        {
            //Updates Variables
            CountdownTimer.Start();
            DisplayTime = time;
            CurrentQuestion = question;
            CanClick = true;
            
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
                //Updates Variables.
                CanClick = false;
                CountdownTimer.Stop();
                AnwserTimer.Start();

                //Checks if correct anwser.
                if ( SingleplayerCtr.AnwserQuestion(CurrentQuestion.Anwsers[e.Position]) )
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
            };
        }
    }
}