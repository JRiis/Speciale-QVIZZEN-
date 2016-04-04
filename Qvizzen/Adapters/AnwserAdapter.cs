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
    public class AnwserAdapter : BaseAdapter<Anwser>
    {
        private List<Anwser> AnwserList;
        private Activity Context;

        public AnwserAdapter(Activity context, List<Anwser> anwserList)
            : base()
        {
            Context = context;
            AnwserList = anwserList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Anwser this[int position]
        {
            get { return AnwserList[position]; }
        }
        public override int Count
        {
            get { return AnwserList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = AnwserList[position].Text;
            return view;
        }
    }
}