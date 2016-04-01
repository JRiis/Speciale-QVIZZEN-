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

namespace Qvizzen
{    
    [Activity(Label = "PackageCreatorMainActivity")]
    public class PackageCreatorMainActivity : Activity
    {
        private ContentController ContentCtr;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Creates GUI
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorMain);

            //Setup content adapter for list.
            ContentCtr = ContentController.GetInstance();
            
            
            
            ListView listPackages = FindViewById<ListView>(Resource.Id.listViewPackages);
            var adapter = new PackAdapter(this, ContentCtr.Content);
            listPackages.Adapter = adapter;
            
            //TEST
            var test = new Pack();
            ContentCtr.Content.Add(test);
            //TEST


            //Setup Click Event for list.
            listPackages.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                StartActivity(typeof(MainActivity));

                var i = e.Position;
            };
        }
    }
}