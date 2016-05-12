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

            //Starts Gameplay
            ContentController.GetInstance().GameIsMultiplayer = true;
            GameplayCtr.StartGame(this);
        }
    }
}