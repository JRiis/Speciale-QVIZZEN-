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
    public class GameplayController
    {
        public GamePack GamePack;

        public void StartGame(GamePack gamePack)
        {
            GamePack = gamePack;



        }
    }
}