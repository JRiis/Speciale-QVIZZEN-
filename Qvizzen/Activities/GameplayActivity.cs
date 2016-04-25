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
    [Activity(Label = "GameplayActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameplayActivity : ParentActivity
    {
        private SingleplayerController SingleplayerCtr;
        private AnwserAdapterGameplay Adapter;
        private ScoreAdapter ScoreAdapter;
        private Timer CountdownTimer;
        private Timer AnwserTimer;
        private int DisplayTime;
        private Question CurrentQuestion;
        private bool CanClick;

        private const double AnwserTime = 1500;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Gameplay);

            //Setup Controller
            SingleplayerCtr = SingleplayerController.GetInstance();

            //Setup content adapter for list.
            SingleplayerCtr = SingleplayerController.GetInstance();
            ListView listScore = FindViewById<ListView>(Resource.Id.listViewScore);
            ScoreAdapter = new ScoreAdapter(this, SingleplayerCtr.Players);
            listScore.Adapter = Adapter;

            //Setup Swipe Layout
            var swipeLayout = FindViewById<SwipeLayout>(Resource.Id.swipeLayout1);
            swipeLayout.SetShowMode(SwipeLayout.ShowMode.PullOut);
            var scorescreenView = FindViewById(Resource.Id.linearLayoutScorescreen);
            swipeLayout.AddDrag(SwipeLayout.DragEdge.Right, scorescreenView);

            //Starts Gameplay
            SingleplayerCtr.StartGame(this);
        }

        public void TimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread( () =>
            {
                //Updates label with timer.
                DisplayTime -= 1;
                TextView timerLabel = FindViewById<TextView>(Resource.Id.textViewTime);
                timerLabel.Text = DisplayTime.ToString();

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
                if (SingleplayerCtr.FinalQuestion)
                {
                    //Ends the game.
                    StartActivity(typeof(ScorescreenActivity));
                }
                else
                {
                    //Advances a turn.
                    SingleplayerCtr.NextTurn();
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
                //Checks if CanClick.
                if (CanClick)
                {
                    //Updates Variables.
                    CanClick = false;
                    CountdownTimer.Stop();

                    //Checks if correct anwser.
                    if (SingleplayerCtr.AnwserQuestion(CurrentQuestion.Anwsers[e.Position]))
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