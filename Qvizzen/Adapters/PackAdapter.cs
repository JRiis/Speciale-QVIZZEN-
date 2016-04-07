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
    public class PackAdapter : BaseAdapter<Pack>
    {
        private List<Pack> PackList;
        private Activity Context;

        public PackAdapter(Activity context, List<Pack> packList) : base()
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
                view = Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }
            var cell = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            cell.Text = PackList[position].Name;
            cell.TextSize = 20;
            return view;
        }
    }
}