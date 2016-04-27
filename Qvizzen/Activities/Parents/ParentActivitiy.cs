using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Qvizzen.Controller;
using Android.Content.PM;

namespace Qvizzen
{
    [Activity(Label = "ParentActivity")]
    public class ParentActivity : Activity
    {
        private ContentController ContentCtr;

        protected override void OnPause()
        {
            base.OnPause();
            ContentCtr = ContentController.GetInstance();
            ContentCtr.SaveContent();
        }
    }
}

