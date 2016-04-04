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
    public class PackageCreatorPackageActivity : Activity
    {
        private ContentController ContentCtr;
        private QuestionAdapter Adapter;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorPackage);

            //Updates the title
            ContentCtr = ContentController.GetInstance();
            TextView title = FindViewById<TextView>(Resource.Id.textView1);
            title.Text = ContentCtr.CurrentPack.Name;

            //Setup content adapter for list.
            ListView listQuestions = FindViewById<ListView>(Resource.Id.listViewQuestions);
            Adapter = new QuestionAdapter(this, ContentCtr.CurrentPack.Questions);
            listQuestions.Adapter = Adapter;

            //Setup Click Event for List Items.
            listQuestions.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                ContentCtr.CurrentQuestion = ContentCtr.CurrentPack.Questions[e.Position];
                StartActivity(typeof(MainActivity));
            };

            //Setup Click Event for Create Button.
            Button buttonNewQuestion = FindViewById<Button>(Resource.Id.buttonNewQuestion);
            buttonNewQuestion.Click += delegate
            {
                Pack newPack = new Pack();
                ContentCtr.CurrentPack = newPack;
                ContentCtr.Content.Add(newPack);
                StartActivity(typeof(PackageCreatorMainActivity));
            };

            //Setup Click Event for Delete Button.
            Button buttonDeletePackage = FindViewById<Button>(Resource.Id.buttonDeletePack);
            buttonDeletePackage.Click += delegate
            {   
                ContentCtr.Content.Remove(ContentCtr.CurrentPack);
                ContentCtr.CurrentPack = null;
                Finish();
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.CurrentPack.Name = newText;
            };
        }
    }
}