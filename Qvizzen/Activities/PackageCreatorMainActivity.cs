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

namespace Qvizzen
{    
    [Activity(Label = "PackageCreatorMainActivity")]
    public class PackageCreatorMainActivity : Activity
    {
        private ContentController ContentCtr;
        private PackAdapter Adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorMain);
            //Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            //Setup content adapter for list.
            ContentCtr = ContentController.GetInstance();
            ListView listPackages = FindViewById<ListView>(Resource.Id.listViewPackages);
            Adapter = new PackAdapter(this, ContentCtr.Content);
            listPackages.Adapter = Adapter;

            //Setup Click Event for List Items.
            listPackages.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                ContentCtr.CurrentPack = ContentCtr.Content[e.Position];
                StartActivity(typeof(PackageCreatorPackageActivity));
            };

            //Setup Click Event for button.
            Button buttonNewPackage = FindViewById<Button>(Resource.Id.buttonNewPackage);
            buttonNewPackage.Click += delegate
            {
                Pack newPack = new Pack();
                ContentCtr.CurrentPack = newPack;
                ContentCtr.Content.Add(newPack);
                StartActivity(typeof(PackageCreatorPackageActivity));
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            Adapter.NotifyDataSetChanged();
        }
    }
}