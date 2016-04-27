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
        private LobbyAdapter Adapter;
        private String SelectedLobbyAddress;
        private Thread AdapterThread;
        private Thread JoinThread;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Multiplayer);

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
            Button buttonJoin = FindViewById<Button>(Resource.Id.buttonJoin);
            buttonJoin.Click += delegate
            {
                if (SelectedLobbyAddress != "")
                {
                    JoinThread = new Thread(new ThreadStart(delegate
                    {
                        MultiplayerCtr.JoinLobby(SelectedLobbyAddress);
                        RunOnUiThread(() =>
                        {
                            StartActivity(typeof(MultiplayerLobbyActivityClient));
                        });
                    }));
                    JoinThread.Start();
                }    
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.Name = newText;
            };

            //Starts a thread to setup adapter.
            AdapterThread = new Thread(new ThreadStart(delegate 
            {
                //Setup content adapter for list.
                MultiplayerCtr = MultiplayerController.GetInstance();
                MultiplayerCtr.GetLobbies();

                RunOnUiThread( () =>
                {
                    ListView listLobbies = FindViewById<ListView>(Resource.Id.listViewLobbies);
                    Adapter = new LobbyAdapter(this, MultiplayerCtr.Lobbies);
                    listLobbies.Adapter = Adapter;

                    //Setup Click Event for List Items.
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
                }); 
            }));

            AdapterThread.Start();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //Adapter.NotifyDataSetChanged();
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
            try
            {
                AdapterThread.Abort();
                JoinThread.Abort();
            }
            catch (NullReferenceException) { }
        }
    }
}