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

namespace Qvizzen.Controller
{
    public class ContentController
    {
        private static ContentController Instance;
        public Pack CurrentPack;
        public Question CurrentQuestion;
        public Anwser CurrentAnwser;
        public List<Pack> Content;

        public ContentController()
        {
            Content = new List<Pack>();
        }

        public static ContentController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ContentController();
            }
            return Instance;
        }
    }
}