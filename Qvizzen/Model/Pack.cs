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
    class Pack
    {
        public string Name;
        public List<Question> Questions;

        Pack()
        {
            Name = "";
            Questions = new List<Question>();
        }
    }
}