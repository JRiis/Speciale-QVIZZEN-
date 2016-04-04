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
    public class PackageCreatorAnwserActivity : ParentActivity
    {
        private ContentController ContentCtr;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorAnwser);

            //Updates title text.
            ContentCtr = ContentController.GetInstance();
            TextView title = FindViewById<TextView>(Resource.Id.textView1);
            title.Text = ContentCtr.CurrentAnwser.Text;

            //Updates checkbox boolean.
            CheckBox isCorrectBox = FindViewById<CheckBox>(Resource.Id.checkBox1);
            isCorrectBox.Checked = ContentCtr.CurrentAnwser.IsCorrect;

            //Setup Click Event for Delete Button.
            Button buttonDeleteAnwser = FindViewById<Button>(Resource.Id.buttonDeleteAnwser);
            buttonDeleteAnwser.Click += delegate
            {   
                ContentCtr.CurrentQuestion.Anwsers.Remove(ContentCtr.CurrentAnwser);
                ContentCtr.CurrentAnwser = null;
                Finish();
            };

            //Setup Click Event for Checkbox.
            isCorrectBox.Click += delegate
            {
                ContentCtr.CurrentAnwser.IsCorrect = isCorrectBox.Checked;
            };

            //Setup Edit Event for Title.
            title.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                string newText = e.Text.ToString();
                ContentCtr.CurrentAnwser.Text = newText;
            };
        }
    }
}