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

namespace Qvizzen.Adapters
{
    public class LobbyAdapter : BaseAdapter<MultiplayerController.Lobby>
    {
        private List<MultiplayerController.Lobby> LobbyList;
        private Activity Context;

        public LobbyAdapter(Activity context, List<MultiplayerController.Lobby> scoreList)
            : base()
        {
            Context = context;
            LobbyList = scoreList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override MultiplayerController.Lobby this[int position]
        {
            get { return LobbyList[position]; }
        }
        public override int Count
        {
            get { return LobbyList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Resource.Layout.ScoreCustomListItem, null);
            }
            view.FindViewById<TextView>(Resource.Id.Text1).Text = LobbyList[position].Hostname;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = LobbyList[position].Count.ToString();
            view.FindViewById<TextView>(Resource.Id.Text1).TextSize = 20;
            view.FindViewById<TextView>(Resource.Id.Text2).TextSize = 20;
            return view;
        }
    }
}