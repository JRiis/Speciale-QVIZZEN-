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

namespace Qvizzen
{
    public class Question
    {
        public string Text;
        public List<Anwser> Anwsers;

        public Question()
        {
            Text = "";
            Anwsers = new List<Anwser>();
        }
    }
}