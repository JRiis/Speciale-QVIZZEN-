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
using System.Threading;

namespace Qvizzen
{
    [Activity(Label = "MultiplayerLobbyActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerLobbyActivity : ParentActivity
    {
        private MultiplayerController MultiplayerCtr;
        private PlayerAdapter Adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MultiplayerLobby);

            //Setup content adapter for list.

            //TODO: List players or add em or something when join n stuff ye dig?
            MultiplayerCtr = MultiplayerController.GetInstance();
            ListView listScore = FindViewById<ListView>(Resource.Id.listViewScore);
            Adapter = new PlayerAdapter(this, MultiplayerCtr.Players);
            listScore.Adapter = Adapter;

            //Setup Click Event for button.
            Button buttonStartGame = FindViewById<Button>(Resource.Id.buttonStartMultiplayerGame);
            buttonStartGame.Click += delegate
            {
                StartActivity(typeof(GameplayActivity));

                //TODO: Start multiplayer gameplay.
            };






            //TEST - Start server n crash dat phone.
            var server = NetworkController.GetInstance().Host();
            var thread = new Thread(new ThreadStart(server.CreateListener));
            thread.Start();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
        }
    }
}