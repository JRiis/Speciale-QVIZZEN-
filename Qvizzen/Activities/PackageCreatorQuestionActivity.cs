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

namespace Qvizzen.Activities
{
    [Activity(Label = "PackageCreatorPackageActivity")]
    public class PackageCreatorQuestionActivity : ParentActivity
    {
        private ContentController ContentCtr;
        private AnwserAdapter Adapter;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorQuestion);

            //Updates the title
            ContentCtr = ContentController.GetInstance();
            TextView title = FindViewById<TextView>(Resource.Id.textView1);
            title.Text = ContentCtr.CurrentQuestion.Text;

            //Setup content adapter for list.
            ListView listAnwsers = FindViewById<ListView>(Resource.Id.listViewAnwsers);
            Adapter = new AnwserAdapter(this, ContentCtr.CurrentQuestion.Anwsers);
            listAnwsers.Adapter = Adapter;

            //Setup Click Event for List Items.
            listAnwsers.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                ContentCtr.CurrentQuestion = ContentCtr.CurrentPack.Questions[e.Position];
                StartActivity(typeof(PackageCreatorAnwserActivity));
            };

            //Setup Click Event for Create Button.
            Button buttonNewAnwser = FindViewById<Button>(Resource.Id.buttonNewAnwser);
            buttonNewAnwser.Click += delegate
            {
                Anwser newAnwser = new Anwser();
                ContentCtr.CurrentAnwser = newAnwser;
                ContentCtr.CurrentQuestion.Anwsers.Add(newAnwser);
                StartActivity(typeof(PackageCreatorAnwserActivity));
            };

            //Setup Click Event for Delete Button.
            Button buttonDeleteQuestion = FindViewById<Button>(Resource.Id.buttonDeleteQuestion);
            buttonDeleteQuestion.Click += delegate
            {   
                ContentCtr.CurrentPack.Questions.Remove(ContentCtr.CurrentQuestion);
                ContentCtr.CurrentQuestion = null;
                Finish();
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.CurrentQuestion.Text = newText;
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
        }
    }
}