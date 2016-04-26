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
using Newtonsoft.Json;
using System.IO;

namespace Qvizzen.Controller
{
    public class ContentController
    {
        public string Name;
        public const string Playername = "Name";
        public const string Filename = "Content";
        private static ContentController Instance;
        public Pack CurrentPack;
        public Question CurrentQuestion;
        public Anwser CurrentAnwser;
        public List<Pack> Content;

        public ContentController()
        {
            Content = new List<Pack>();
            Name = "";
        }

        public static ContentController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ContentController();
            }
            return Instance;
        }

        /// <summary>
        /// Saves all content from memory to an JSON file.
        /// </summary>
        public void SaveContent()
        {
            //Packages
            var json = JsonConvert.SerializeObject(Content);
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Filename);
            System.IO.File.WriteAllText(filePath, json);

            //Playername
            filePath = Path.Combine(documentsPath, Playername);
            System.IO.File.WriteAllText(filePath, Name);
        }

        /// <summary>
        /// Loads all content stored on the phone.
        /// </summary>
        public void LoadContent()
        {
            //Packages
            try
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, Filename);
                var json = System.IO.File.ReadAllText(filePath);
                Content = JsonConvert.DeserializeObject<List<Pack>>(json);
            }
            catch (System.IO.FileNotFoundException) { }

            //Playername
            try
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, Playername);
                var name = System.IO.File.ReadAllText(filePath);
                Name = name;
            }
            catch (System.IO.FileNotFoundException) { }
        }
    }
}