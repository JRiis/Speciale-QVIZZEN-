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
            GameplayCtr.SetupGamePack();
            GameplayCtr.StartGame(this);
        }
    }
}