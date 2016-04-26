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
    [Activity(Label = "MultiplayerLobbyActivityHost", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerLobbyActivityHost : ParentActivity
    {
        private MultiplayerController MultiplayerCtr;
        private PlayerAdapter Adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MultiplayerLobbyHost);

            //Setup content adapter for list.
            MultiplayerCtr = MultiplayerController.GetInstance();
            ListView listPlayers = FindViewById<ListView>(Resource.Id.listViewPlayers);
            Adapter = new PlayerAdapter(this, MultiplayerCtr.Players);
            listPlayers.Adapter = Adapter;

            //Setup Click Event for button.
            Button buttonStartGame = FindViewById<Button>(Resource.Id.buttonStartMultiplayerGame);
            buttonStartGame.Click += delegate
            {
                StartActivity(typeof(GameplayActivity));

                //TODO: Start multiplayer gameplay.
            };

            //Hosts dat server doe.
            MultiplayerCtr.HostServer();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
            //TODO: Reconnect
        }

        protected override void OnStop()
        {
            base.OnStop();
            //TODO: Disconnect? Also perhaps boolean to check if player is host.
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //TODO: Disconnect/Unhost.
            MultiplayerCtr.UnhostServer();
        }
    }
}