using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Qvizzen.Controller;

namespace Qvizzen
{
    [Activity(Label = "Qvizzen", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ParentActivity
    {
        private ContentController ContentCtr;
        
        protected override void OnCreate(Bundle bundle)
        {
            //Setup GUI
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Loads content from phone.
            ContentCtr = ContentController.GetInstance();
            ContentCtr.LoadContent();

            //Setups singleplayer button.
            Button buttonSingleplayer = FindViewById<Button>(Resource.Id.buttonSingleplayer);
            buttonSingleplayer.Click += delegate
            {
                StartActivity(typeof(SingleplayerPackageSelectionActivity));
            };

            //Setups content creator button.
            Button buttonCreator = FindViewById<Button>(Resource.Id.buttonCreator);
            buttonCreator.Click += delegate 
            {
                StartActivity(typeof(PackageCreatorMainActivity));
            };


        }
    }
}

