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
    public class MultiplePackAdapter : BaseAdapter<Pack>
    {
        private List<Pack> PackList;
        private Activity Context;

        public MultiplePackAdapter(Activity context, List<Pack> packList) : base()
        {
            Context = context;
            PackList = packList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Pack this[int position]
        {
            get { return PackList[position]; }
        }
        public override int Count
        {
            get { return PackList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemChecked, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = PackList[position].Name;
            view.FindViewById<TextView>(Android.Resource.Id.Text1).TextSize = 20;
            return view;
        }
    }
}