using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Qvizzen.Controller;
using Android.Content.PM;
using Android.Media;

namespace Qvizzen
{
    [Activity(Label = "ParentActivity")]
    public class ParentActivity : Activity
    {
        private ContentController ContentCtr;
        public BaseAdapter Adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
        }

        protected override void OnPause()
        {
            base.OnPause();
            ContentCtr = ContentController.GetInstance();
            ContentCtr.SaveContent();
        }

        /// <summary>
        /// Updates the adapter for lobbies to refresh the list.
        /// </summary>
        public void AdapterUpdate()
        {
            RunOnUiThread(() =>
            {
                Adapter.NotifyDataSetChanged();
            });
        }

        /// <summary>
        /// Starts the given activity on the UI thread.
        /// </summary>
        public void StartActivityOnUIThread(Type activity)
        {
            RunOnUiThread(() =>
            {
                StartActivity(activity);
            });
        }
    }
}

