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

namespace Qvizzen.Activities
{
    [Activity(Label = "MultiplayerActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerActivity : ParentActivity
    {
        private MultiplayerController MultiplayerCtr;
        private ContentController ContentCtr;
        private LobbyAdapter Adapter;
        private const int AnwserLimit = 4;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Multiplayer);

            //Updates the title
            ContentCtr = ContentController.GetInstance();
            TextView title = FindViewById<TextView>(Resource.Id.textViewPlayerName);
            title.Text = ContentCtr.Name;

            //Setup content adapter for list.
            MultiplayerCtr = MultiplayerController.GetInstance();
            ListView listLobbies = FindViewById<ListView>(Resource.Id.listViewLobbies);
            Adapter = new LobbyAdapter(this, MultiplayerCtr.Lobbies);
            listLobbies.Adapter = Adapter;

            //Setup Click Event for List Items.
            listLobbies.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                //TODO: Select lobby funthyme.
                
                ContentCtr.CurrentAnwser = ContentCtr.CurrentQuestion.Anwsers[e.Position];
                StartActivity(typeof(PackageCreatorAnwserActivity));
            };

            //Setup Click Event for Host Button.
            Button buttonHost = FindViewById<Button>(Resource.Id.buttonHost);
            buttonHost.Click += delegate
            {
                //TODO: Host a friggin lobby.
                StartActivity(typeof(MultiplayerPackageSelectionActivity));
            };

            //Setup Click Event for Join Button.
            Button buttonJoin = FindViewById<Button>(Resource.Id.buttonJoin);
            buttonJoin.Click += delegate
            {   
                //TODO: Join a friggin lobby.
                var client = new NetworkController.Client();
                client.Connect("10.28.53.28", "GetGamePack");
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.Name = newText;
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            //Adapter.NotifyDataSetChanged();
        }
    }
}