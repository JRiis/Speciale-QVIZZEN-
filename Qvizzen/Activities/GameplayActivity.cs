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
        private ContentController ContentCtr;
        private SingeplayerController SingleplayerCtr;
        private AnwserAdapter Adapter;
        private Timer CountdownTimer;
        private int DisplayTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Gameplay);

            //Setup Controller
            SingleplayerCtr = SingeplayerController.GetInstance();

            //Setup Timer
            CountdownTimer = new Timer();
            CountdownTimer.Interval = 1000;
            CountdownTimer.Enabled = true;
            CountdownTimer.Elapsed += TimerTickEvent;

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
            }
        }

        public void UpdateGUI(Question question, int time)
        {
            //Restarts Timer
            DisplayTime = time;
            CountdownTimer.Enabled = true;
            

            //Update question label.
            TextView timerLabel = FindViewById<TextView>(Resource.Id.textViewQuestion);


            //Setup content adapter for list.
            ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
            Adapter = new AnwserAdapter(this, question.Anwsers);
            listAnwsers.Adapter = Adapter;




        }




    }
}