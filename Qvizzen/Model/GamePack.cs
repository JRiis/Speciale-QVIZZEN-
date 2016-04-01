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
    public class GamePack
    {
        public List<Pack> Packs;
        
        public GamePack()
        {
            Packs = new List<Pack>();
        }
    }
}