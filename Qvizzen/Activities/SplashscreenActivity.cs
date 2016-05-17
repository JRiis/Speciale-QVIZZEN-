using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Qvizzen.Controller;
using Android.Content.PM;
using Qvizzen.Activities;
using Android.Media;
using Android.Support.V7.App;

namespace Qvizzen
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, Label = "Qvizzen", Icon = "@drawable/icon", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {        
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}