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

namespace Qvizzen.Adapters
{
    public class ScoreAdapter : BaseAdapter<Tuple<string, int>>
    {
        private List<Tuple<string, int>> ScoreList;
        private Activity Context;

        public ScoreAdapter(Activity context, List<Tuple<string, int>> scoreList)
            : base()
        {
            Context = context;
            ScoreList = scoreList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Tuple<string, int> this[int position]
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
                view = Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = ScoreList[position].Item1;
            view.FindViewById<TextView>(Android.Resource.Id.Text1).TextSize = 20;
            return view;
        }
    }
}