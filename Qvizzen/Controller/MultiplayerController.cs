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

using Qvizzen.Model;
using Qvizzen.Networking;
using Newtonsoft.Json;

namespace Qvizzen.Controller
{
    public class MultiplayerController : GameplayController
    {
        public Server Server { private set; get; }
        private Client Client;
        private static MultiplayerController Instance;
        public bool IsHost;
        public List<Lobby> Lobbies;

        private const int Port = 4444;

        /// <summary>
        /// Constructor for MultiplayerController
        /// </summary>
        public MultiplayerController()
        {
            Client = new Client();
            Players = new List<Player>();
            Lobbies = new List<Lobby>();
            IsHost = false;
        }

        /// <summary>
        /// Singleton for MultiplayerController
        /// </summary>
        /// <returns>Instance of controller.</returns>
        public static MultiplayerController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MultiplayerController();
            }
            return Instance;
        }

        public void HostServer()
        {
            Server = new Server();
            Server.StartServer(Port);
            SetupGamePack();
            IsHost = true;
        }

        public void UnhostServer()
        {
            Server.StopServer();
            Server = null;
            IsHost = false;
        }

        public void JoinLobby(String ipAddress)
        {
            Client.Connect(ipAddress, Port);
            
            String responseData = Client.SendMessage("GetGamePack");
            GamePack gamePack = JsonConvert.DeserializeObject<GamePack>(responseData);
            MultiplayerController.GetInstance().GamePack = gamePack;
            foreach (Pack pack in gamePack.Packs)
            {
                ContentController.GetInstance().Content.Add(pack);
            }

            responseData = Client.SendMessage("GetQuestionList");
            Questions = JsonConvert.DeserializeObject<List<Question>>(responseData);

            Client.SendMessage("");
            //TODO:
            //Download GamePack
            //Download QuestionList
            //AddPlayer to Lobby
            //Call ready once downloads are done.


            //TODO: Stuff
        }

        public void LeaveLobby()
        {
            //TODO: Stuff
            //Remove Player From Lobby
        }

        public void GetLobbies()
        {
            Lobbies = Client.RetriveHosts(Port);
        }

        //public void Connect(string serverIP, string message)
        //{
            
        //    switch (message)
        //    {
        //        case "GetGamePack":
        //            GamePack gamePack = JsonConvert.DeserializeObject<GamePack>(reponseData);
        //            MultiplayerController.GetInstance().GamePack = gamePack;
        //            foreach (Pack pack in gamePack.Packs)
        //            {
        //                ContentController.GetInstance().Content.Add(pack);
        //            }
        //            break;

        //        case "GetQuestionList":
        //            //TODO
        //            break;

        //        case "SendAnwser":
        //            //TODO
        //            break;

        //        case "JoinLobby":
        //            //TODO
        //            break;

        //        case "LeaveLobby":
        //            //TODO
        //            break;

        //        case "LeaveGame":
        //            //TODO
        //            break;

        //        default:
        //            //Nothing
        //            break;
        //    }

        //}

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void AddPlayer(string name, bool host)
        {
            Players.Add(new Player(name, host));
        }

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void RemovePlayer(string name)
        {            
            foreach (Player player in Players)
            {
                if (player.Name == name)
                {
                    Players.Remove(player);
                }
            }
        }

        /// <summary>
        /// Checks if anwser is correct and updates score accordingly. 
        /// </summary>
        public new bool AnwserQuestion(Anwser anwser)
        {
            
            
            //TODO: Async send to players.
            
            
            if (anwser.IsCorrect)
            {
                //Correct Anwser
                CurrentPlayer.Score += QuestionValue;
                return true;
            }
            else
            {
                //Wrong Anwser
                return false;
            }
        }

        /// <summary>
        /// Advances the gameplay a turn.
        /// </summary>
        public new void NextTurn()
        {
            
            //TODO: Async send to players.

            //Condition, all players have recived input and responded they have been recived.


            CurrentPlayer = GetNextPlayer();
            Activity.UpdateGUI(GetQuestion(), DefaultTimer, CurrentPlayer.Score, CurrentIndex, Questions.Count);
        }
    }
}