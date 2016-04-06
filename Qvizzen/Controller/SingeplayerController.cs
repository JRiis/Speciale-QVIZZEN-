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
    public class SingeplayerController : GameplayController
    {
        private static SingeplayerController Instance;
        public GameplayActivity Activity;
        public int Score;

        public const int DefaultTimer = 30;
        public const string Playername = "Your Score";

        public static SingeplayerController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SingeplayerController();
            }
            return Instance;
        }

        public void StartGame(GameplayActivity activity)
        {
            Score = 0;
            Activity = activity;



            Activity.UpdateGUI();
        }

    }
}