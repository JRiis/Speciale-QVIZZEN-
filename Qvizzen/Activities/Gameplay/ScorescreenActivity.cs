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
using Qvizzen.Controller;
using Qvizzen.Adapters;
using Qvizzen.Activities;
using Android.Content.PM;

namespace Qvizzen
{
    [Activity(Label = "ScorescreenActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ScorescreenActivity : ParentActivity
    {
        private GameplayController GameplayCtr;
        private ScoreAdapter Adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Scorescreen);

            //Setup content adapter for list.
            if (ContentController.GetInstance().GameIsMultiplayer)
            {
                GameplayCtr = MultiplayerController.GetInstance();
            }
            else
            {
                GameplayCtr = SingleplayerController.GetInstance();
            }

            ListView listScore = FindViewById<ListView>(Resource.Id.listViewScore);
            Adapter = new ScoreAdapter(this, GameplayCtr.Players);
            listScore.Adapter = Adapter;

            //Setup Click Event for button.
            Button buttonMainMenu = FindViewById<Button>(Resource.Id.buttonMainMenu);
            buttonMainMenu.Click += delegate
            {
                if (ContentController.GetInstance().GameIsMultiplayer)
                {
                    MultiplayerController.GetInstance().UnhostServer();
                }
                StartActivity(typeof(MainActivity));
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
        }
    }
}