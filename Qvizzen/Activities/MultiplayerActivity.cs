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
        private ContentController ContentCtr;
        private AnwserAdapter Adapter;
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

            //TODO: Listen for ports n shit, all buncha crazy stuff!
            ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
            Adapter = new AnwserAdapter(this, ContentCtr.CurrentQuestion.Anwsers);
            listAnwsers.Adapter = Adapter;

            //Setup Click Event for List Items.
            listAnwsers.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                ContentCtr.CurrentAnwser = ContentCtr.CurrentQuestion.Anwsers[e.Position];
                StartActivity(typeof(PackageCreatorAnwserActivity));
            };

            //Setup Click Event for Host Button.
            Button buttonNewAnwser = FindViewById<Button>(Resource.Id.buttonNewAnwser);
            buttonNewAnwser.Click += delegate
            {
                //TODO: Host a friggin lobby.
            };

            //Setup Click Event for Join Button.
            Button buttonDeleteQuestion = FindViewById<Button>(Resource.Id.buttonDeleteQuestion);
            buttonDeleteQuestion.Click += delegate
            {   
                //TODO: Join a friggin lobby.
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
            Adapter.NotifyDataSetChanged();
        }
    }
}