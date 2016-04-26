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
using Qvizzen.Model;

namespace Qvizzen.Adapters
{
    public class PlayerAdapter : BaseAdapter<Player>
    {
        private List<Player> PlayerList;
        private Activity Context;

        public PlayerAdapter(Activity context, List<Player> scoreList)
            : base()
        {
            Context = context;
            PlayerList = scoreList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Player this[int position]
        {
            get { return PlayerList[position]; }
        }
        public override int Count
        {
            get { return PlayerList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Resource.Layout.ScoreCustomListItem, null);
            }
            
            //Setup text elements.
            view.FindViewById<TextView>(Resource.Id.Text1).Text = PlayerList[position].Name;
            view.FindViewById<TextView>(Resource.Id.Text1).TextSize = 20;
            
            //Check if host, to make host bold.
            if (PlayerList[position].Host)
            {
                view.FindViewById<TextView>(Resource.Id.Text1).SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.Text1).SetTypeface(null, Android.Graphics.TypefaceStyle.Normal);
            }


            return view;
        }
    }
}