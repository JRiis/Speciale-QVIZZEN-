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
using Qvizzen.Extensions;
using Android.Media;

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
        internal MediaPlayer TimerTickSound;

        internal const double AnwserTime = 1500;

        public void TimerTickEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread( () =>
            {
                //Updates label with timer.
                DisplayTime -= 1;
                TextView timerLabel = FindViewById<TextView>(Resource.Id.textViewTime);
                timerLabel.Text = DisplayTime.ToString();

                //If timer is low, give feedback to indicate short time remaining, by changing color.
                if (DisplayTime == 10)
                {
                    var color = new Android.Graphics.Color(237, 50, 50, 255);
                    timerLabel.SetTextColor(color);

                    //Starts a timer sound for low timer adventureh.
                    TimerTickSound = MediaPlayer.Create(this, Resource.Raw.TimerTicking);
                    TimerTickSound.Looping = true;
                    TimerTickSound.Start();
                }

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

                    //Plays Incorrect sound.
                    MediaPlayer Sound = MediaPlayer.Create(this, Resource.Raw.Incorrect);
                    Sound.Start();

                    //Stop timer ticking.
                    try
                    {
                        TimerTickSound.Stop();
                    }
                    catch (System.NullReferenceException ex)
                    {

                    }
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
                    StartActivityForResult(typeof(ScorescreenActivity), 0);
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
            TextView playername = FindViewById<TextView>(Resource.Id.textViewGameplayPlayername);
            playername.Text = GameplayCtr.CurrentPlayer.Name;

            //Set color of timer label.
            var black = new Android.Graphics.Color(0, 0, 0, 255);
            timeLabel.SetTextColor(black);

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

                    //Stop timer ticking.
                    try
                    {
                        TimerTickSound.Stop();
                    }
                    catch (System.NullReferenceException ex)
                    {

                    }

                    if (GameplayCtr.AnwserQuestion(CurrentQuestion.Anwsers[e.Position], e.Position))
                    {
                        //Updates question label.
                        questionLabel.Text = "Correct!";
                        var color = new Android.Graphics.Color(50, 237, 50, 255);
                        e.View.SetBackgroundColor(color);
                        
                        //Plays correct sound.
                        MediaPlayer Sound = MediaPlayer.Create(this, Resource.Raw.Correct);
                        Sound.Start();
                    }
                    else
                    {
                        //Updates question label.
                        questionLabel.Text = "Incorrect!";
                        var color = new Android.Graphics.Color(237, 50, 50, 255);
                        e.View.SetBackgroundColor(color);

                        //Plays Incorrect sound.
                        MediaPlayer Sound = MediaPlayer.Create(this, Resource.Raw.Incorrect);
                        Sound.Start();
                    }

                    //Updates score label.
                    scoreLabel.Text = "Score: " + GameplayCtr.CurrentPlayer.Score.ToString();

                    //Starts Anwser Timer
                    AnwserTimer = new Timer();
                    AnwserTimer.Interval = AnwserTime;
                    AnwserTimer.Elapsed += AnwserTimerTickEvent;
                    AnwserTimer.Enabled = true;
                    AnwserTimer.AutoReset = false;
                }
            };
        }

        /// <summary>
        /// Answers the question at given position. Is used for multiplayer when other players answer questions.
        /// </summary>
        public void AnswerQuestion(int position)
        {
            RunOnUiThread( () =>
            {
                //Finds answer.
                ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
                View view = listAnwsers.GetChildAt(position);
                Anwser item = ExtensionMethods.Cast<Anwser>(listAnwsers.Adapter.GetItem(position));

                //Updates Variables.
                CanClick = false;
                CountdownTimer.Stop();

                //Stop timer ticking.
                try
                {
                    TimerTickSound.Stop();
                }
                catch (System.NullReferenceException ex)
                {

                }

                //Checks if correct anwser.
                TextView questionLabel = FindViewById<TextView>(Resource.Id.textViewQuestion);

                if (item.IsCorrect)
                {
                    //Updates question label.
                    questionLabel.Text = "Correct!";
                    var color = new Android.Graphics.Color(50, 237, 50, 255);
                    view.SetBackgroundColor(color);
                    GameplayCtr.CurrentPlayer.Score += GameplayController.QuestionValue;

                    //Plays correct sound.
                    MediaPlayer Sound = MediaPlayer.Create(this, Resource.Raw.Correct);
                    Sound.Start();
                }
                else
                {
                    //Updates question label.
                    questionLabel.Text = "Incorrect!";
                    var color = new Android.Graphics.Color(237, 50, 50, 255);
                    view.SetBackgroundColor(color);

                    //Plays Incorrect sound.
                    MediaPlayer Sound = MediaPlayer.Create(this, Resource.Raw.Incorrect);
                    Sound.Start();
                }

                //Updates score label.
                TextView scoreLabel = FindViewById<TextView>(Resource.Id.textViewScore);
                scoreLabel.Text = "Score: " + GameplayCtr.CurrentPlayer.Score.ToString();

                //Starts Anwser Timer
                AnwserTimer = new Timer();
                AnwserTimer.Interval = AnwserTime;
                AnwserTimer.Elapsed += AnwserTimerTickEvent;
                AnwserTimer.Enabled = true;
                AnwserTimer.AutoReset = false;
            });
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
            GameplayCtr.IsIngame = false;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0)
            {
                SetResult(Result.Ok);
                Finish();
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            SetResult(Result.Ok);
            Finish();
        }
    }
}