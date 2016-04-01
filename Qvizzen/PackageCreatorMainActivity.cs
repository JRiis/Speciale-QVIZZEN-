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

namespace Qvizzen
{
    [Activity(Label = "PackageCreatorMainActivity")]
    public class PackageCreatorMainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PackageCreatorMain);

            //TODO: Figure out list yo.
            ListView listPackages = FindViewById<ListView>(Resource.Id.listViewPackages);


            //listPackages.ItemClick
            /*
            listPackages.add

            listPackages.Adapter = new


            listPackages

                //TODO: How 2 Find color adaptor in funthyme stuff...


            listView = FindViewById<ListView> (Resource.Id.myListView);

            colorItems.Add (new ColorItem () { Color = Android.Graphics.Color.DarkRed,
                                               ColorName = "Dark Red", Code = "8B0000" });
            colorItems.Add (new ColorItem () { Color = Android.Graphics.Color.SlateBlue,
                                               ColorName = "Slate Blue", Code = "6A5ACD" });
            colorItems.Add (new ColorItem () { Color = Android.Graphics.Color.ForestGreen,
                                               ColorName = "Forest Green", Code = "228B22" });

            listView.Adapter = new ColorAdapter (this, colorItems);

            */


            // Create your application here
        }
    }
}