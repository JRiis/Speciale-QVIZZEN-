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
using Android.Util;
using Android.Content.PM;

namespace Qvizzen
{
    [Activity(Label = "MultiplayerPackageSelectionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MultiplayerPackageSelectionActivity : ParentActivity
    {
        private ContentController ContentCtr;
        private MultiplayerController MultiplayerCtr;
        private MultiplePackAdapter Adapter;
        private List<Pack> SelectedPacks;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MultiplayerPackageSelection);

            //Setup content adapter for list.
            ContentCtr = ContentController.GetInstance();
            ListView listPackages = FindViewById<ListView>(Resource.Id.listViewPackages);
            Adapter = new MultiplePackAdapter(this, ContentCtr.Content);
            listPackages.Adapter = Adapter;

            //Setup Click Event for List Items.
            SelectedPacks = new List<Pack>();
            listPackages.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                if (listPackages.CheckedItemPositions.Get(e.Position))
                {
                    SelectedPacks.Add(ContentCtr.Content[e.Position]);
                }
                else
                {
                    SelectedPacks.Remove(ContentCtr.Content[e.Position]);
                }
            };

            //Setup Click Event for button.
            MultiplayerCtr = MultiplayerController.GetInstance();
            Button buttonCreateLobby = FindViewById<Button>(Resource.Id.buttonCreateLobby);
            buttonCreateLobby.Click += delegate
            {
                //Confirms selected packages.
                if (SelectedPacks.Count > 0)
                {
                    GamePack gamePack = new GamePack();
                    gamePack.Packs = SelectedPacks;
                    MultiplayerCtr.GamePack = gamePack;
                    MultiplayerCtr.AddPlayer(ContentCtr.IPAddress, ContentCtr.Name, true);
                    StartActivity(typeof(MultiplayerLobbyActivityHost));
                    //TODO: Host multiplayer lobby.

                }
            };
        }
    }
}