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
    public class Pack
    {
        public string Name;
        public List<Question> Questions;

        public Pack()
        {
            Name = "Name Your Pack";
            Questions = new List<Question>();
        }
    }
}