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

namespace Qvizzen
{
    [Activity(Label = "GameplaySingleplayerActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameplaySingleplayerActivity : GameplayParentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Gameplay);

            //TODO: Boolean for multiplayer/singleplayer.

            //Setup Controller
            GameplayCtr = SingleplayerController.GetInstance();

            //Set playername.
            TextView playername = FindViewById<TextView>(Resource.Id.textViewGameplayPlayername);
            playername.Text = "";

            //Starts Gameplay
            ContentController.GetInstance().GameIsMultiplayer = false;
            GameplayCtr.SetupGamePack();
            GameplayCtr.StartGame(this);
        }
    }
}