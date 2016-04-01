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

            //Creator Button
            Button buttonCreator = FindViewById<Button>(Resource.Id.buttonCreator);
            buttonCreator.Click += delegate 
            {
                //SetContentView(Resource.Layout.PackageCreatorMain);
                StartActivity(typeof(PackageCreatorMainActivity));


                //AddContentView(Resource.Layout.PackageCreatorMain);
            };
        }
    }
}

