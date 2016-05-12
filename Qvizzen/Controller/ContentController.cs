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
using System.Net;

namespace Qvizzen.Controller
{
    public class ContentController
    {
        public string Name;
        public string IPAddress;
        public const string Playername = "Name";
        public const string Filename = "Content";
        private static ContentController Instance;
        public Pack CurrentPack;
        public Question CurrentQuestion;
        public Anwser CurrentAnwser;
        public List<Pack> Content;
        public bool GameIsMultiplayer;

        public ContentController()
        {
            Name = "";
            IPAddress = Dns.GetHostEntry(String.Empty).AddressList[0].ToString();
            Content = new List<Pack>();
        }

        /// <summary>
        /// Creates a few test packages.
        /// </summary>
        public void TestSetup()
        {
            for (int i=0; i < 10; i++)
            {
                bool havePack = false;
                foreach (Pack pack in Content)
                {
                    if (pack.Name == "Pack #" + i.ToString())
                    {
                        havePack = true;
                        break;
                    }
                }

                if (havePack)
                {
                    continue;
                }
                
                Pack newPack = new Pack();
                newPack.Name = "Pack #" + i.ToString();
                for (int i2 = 0; i2 < 10; i2++)
                {
                    Question question = new Question();
                    question.Text = "Question #" + i2.ToString();
                    newPack.Questions.Add(question);

                    for (int i3 = 0; i3 < 4; i3++)
                    {
                        Anwser anwser = new Anwser();
                        anwser.Text = "Answer #" + i3.ToString();
                        if (i3 == 1)
                        {
                            anwser.IsCorrect = true;
                        }
                        question.Anwsers.Add(anwser);
                    }
                }
                Content.Add(newPack);
            }
            
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