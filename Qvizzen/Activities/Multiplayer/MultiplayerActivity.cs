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
using Android.Content.PM;
using System.Threading;

namespace Qvizzen.Activities
{
    [Activity(Label = "MultiplayerActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerActivity : ParentActivity
    {
        private MultiplayerController MultiplayerCtr;
        private ContentController ContentCtr;
        private String SelectedLobbyAddress;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Multiplayer);

            //Setup multiplayer controller.
            MultiplayerCtr = MultiplayerController.GetInstance();
            MultiplayerCtr.AdapterActivity = this;

            //Updates the title
            ContentCtr = ContentController.GetInstance();
            TextView title = FindViewById<TextView>(Resource.Id.textViewPlayerName);
            title.Text = ContentCtr.Name;

            //Setup Click Event for Host Button.
            Button buttonHost = FindViewById<Button>(Resource.Id.buttonHost);
            buttonHost.Click += delegate
            {
                StartActivity(typeof(MultiplayerPackageSelectionActivity));
            };

            //Setup Click Event for Join Button.
            SelectedLobbyAddress = "";
            Button buttonJoin = FindViewById<Button>(Resource.Id.buttonJoin);
            buttonJoin.Click += delegate
            {
                if (SelectedLobbyAddress != "")
                {
                    MultiplayerCtr.BeginJoinLobby(SelectedLobbyAddress);
                }
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.Name = newText;
            };

            //Setups adapter for lobbies.
            ListView listLobbies = FindViewById<ListView>(Resource.Id.listViewLobbies);
            MultiplayerCtr.BeginGetLobbies();
            Adapter = new LobbyAdapter(this, MultiplayerCtr.Lobbies);
            listLobbies.Adapter = Adapter;

            //Setup click event for lobby list.
            listLobbies.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                String selectedIPAddress = MultiplayerCtr.Lobbies[e.Position].IPAddress;
                if (selectedIPAddress == SelectedLobbyAddress)
                {
                    SelectedLobbyAddress = "";
                }
                else
                {
                    SelectedLobbyAddress = selectedIPAddress;
                }
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
            ListView listLobbies = FindViewById<ListView>(Resource.Id.listViewLobbies);
            MultiplayerCtr.BeginGetLobbies();
            Adapter = new LobbyAdapter(this, MultiplayerCtr.Lobbies);
            listLobbies.Adapter = Adapter;
            MultiplayerCtr.Joining = false;
        }

        protected override void OnStop()
        {
            base.OnStop();
            MultiplayerCtr.StopGetLobbies();
            MultiplayerCtr.Lobbies.Clear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MultiplayerCtr.StopGetLobbies();
            MultiplayerCtr.Lobbies.Clear();
        }
    }
}