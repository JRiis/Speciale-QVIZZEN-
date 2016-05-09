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
using System.Threading;
using Qvizzen.Activities;

namespace Qvizzen.Controller
{
    public class MultiplayerController : GameplayController
    {
        public Server Server { private set; get; }
        private Client Client;
        private static MultiplayerController Instance;
        public bool IsHost;
        public bool InGame;
        public List<Lobby> Lobbies;
        private Thread ServerThread;
        public MultiplayerActivity MultiplayerActivity;

        private const int TCPPort = 4444;
        private const int UDPBroadcastPort = 4445;
        private const int UDPListenPort = 4446;

        /// <summary>
        /// Constructor for MultiplayerController
        /// </summary>
        public MultiplayerController()
        {
            Client = new Client();
            Players = new List<Player>();
            Lobbies = new List<Lobby>();
            IsHost = false;
            InGame = false;
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
            ServerThread = new Thread(new ThreadStart(delegate
            {
                Server.StartServer(TCPPort, UDPListenPort);
            }));
            ServerThread.Start();
            SetupGamePack();
            IsHost = true;
        }

        public void UnhostServer()
        {
            ServerThread.Abort();
            Server.StopServer();
            ServerThread = null;
            Server = null;
            Players.Clear();
            IsHost = false;
        }

        public void BeginJoinLobby(String ipAddress)
        {
            Client.Connect(ipAddress, TCPPort);
            Client.SendMessage("Connect");
        }

        public void BeginLeaveLobby()
        {
            //TODO: Stuff
            //Remove Player From Lobby
        }

        public void BeginGetLobbies()
        {
            Client.Broadcast(UDPBroadcastPort);
        }

        public void StopGetLobbies()
        {
            Client.StopBroadcast();
        }


        public void UpdateAdapter()
        {
            MultiplayerActivity.AdapterUpdate();
        }


        public void JoinLobby()
        {

        }







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