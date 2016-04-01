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
    public class QuestionAdapter : BaseAdapter<Question>
    {
        private List<Question> QuestionList;
        private Activity Context;

        public QuestionAdapter(Activity context, List<Question> questionList)
            : base()
        {
            Context = context;
            QuestionList = questionList;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Question this[int position]
        {
            get { return QuestionList[position]; }
        }
        public override int Count
        {
            get { return QuestionList.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = QuestionList[position].Text;
            return view;
        }
    }
}