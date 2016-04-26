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
    public class ScoreAdapter : BaseAdapter<Player>
    {
        private List<Player> ScoreList;
        private Activity Context;

        public ScoreAdapter(Activity context, List<Player> scoreList)
            : base()
        {
            Context = context;
            ScoreList = scoreList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Player this[int position]
        {
            get { return ScoreList[position]; }
        }
        public override int Count
        {
            get { return ScoreList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Resource.Layout.ScoreCustomListItem, null);
            }
            view.FindViewById<TextView>(Resource.Id.Text1).Text = ScoreList[position].Name;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = ScoreList[position].Score.ToString();
            view.FindViewById<TextView>(Resource.Id.Text1).TextSize = 20;
            view.FindViewById<TextView>(Resource.Id.Text2).TextSize = 20;
            return view;
        }
    }
}