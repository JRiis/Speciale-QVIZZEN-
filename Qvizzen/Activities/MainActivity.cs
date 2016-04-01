using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Qvizzen
{
    [Activity(Label = "Qvizzen", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            //Setup
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Setups button activity.
            Button buttonCreator = FindViewById<Button>(Resource.Id.buttonCreator);
            buttonCreator.Click += delegate 
            {
                StartActivity(typeof(PackageCreatorMainActivity));
            };
        }
    }
}

