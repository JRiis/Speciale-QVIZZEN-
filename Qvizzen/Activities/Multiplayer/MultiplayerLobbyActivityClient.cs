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
    [Activity(Label = "MultiplayerLobbyActivityClient", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerLobbyActivityClient : ParentActivity
    {
        private MultiplayerController MultiplayerCtr;
        private PlayerAdapter Adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MultiplayerLobbyClient);

            //Setup content adapter for list.
            MultiplayerCtr = MultiplayerController.GetInstance();
            ListView listPlayers = FindViewById<ListView>(Resource.Id.listViewPlayers);
            Adapter = new PlayerAdapter(this, MultiplayerCtr.Players);
            listPlayers.Adapter = Adapter;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
        }
    }
}