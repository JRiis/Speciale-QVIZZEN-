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
        public List<Lobby> Lobbies;
        private Thread ServerThread;
        public ParentActivity AdapterActivity;
        public MultiplayerActivity MultiplayerActivity;

        private const int TCPPort = 4444;
        private const int UDPBroadcastPort = 4445;
        private const int UDPListenPort = 4445;

        /// <summary>
        /// Constructor for MultiplayerController
        /// </summary>
        public MultiplayerController()
        {
            Client = new Client();
            Client.MultiplayerCtr = this;
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
            Server.StopServer();
            ServerThread.Abort();
            ServerThread = null;
            Server = null;
            Players.Clear();
            IsHost = false;
        }

        public void BeginJoinLobby(String ipAddress)
        {
            Client.Connect(ipAddress, TCPPort);
            ContentController ctr = ContentController.GetInstance();
            string message = JsonConvert.SerializeObject(new List<string>() 
            {
                "Connect", 
                ctr.IPAddress, 
                ctr.Name
            });
            Client.SendMessage(message);
        }

        public void BeginLeaveLobby()
        {
            string message = JsonConvert.SerializeObject(new List<string>() 
            {
                "RageQuit"
            });
            Client.SendMessage(message);
        }

        public void BeginGetLobbies()
        {
            Client.StartBroadcast(UDPBroadcastPort);
        }

        public void StopGetLobbies()
        {
            Client.StopBroadcast();
        }


        public void UpdateAdapter()
        {
            AdapterActivity.AdapterUpdate();
        }

        public void JoinLobby()
        {
            AdapterActivity.StartActivityOnUIThread(typeof(MultiplayerLobbyActivityClient));
        }

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void AddPlayer(string ipAddress, string name, bool host)
        {
            Players.Add(new Player(ipAddress, name, host));
        }

        /// <summary>
        /// Creates and adds a new player to players list.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public void RemovePlayer(string name) //TODO: Identifier/IP instead of name.
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
        /// Starts gameplay activity for all clients currently connected to the host.
        /// </summary>
        public void StartMultiplayerGame()
        {
            if (IsHost)
            {
                string message = JsonConvert.SerializeObject(new List<string>() 
                {
                    "Start"
                });
                Server.SendMessageToClients(message);
                Server.StopUDPListen();
            }

            AdapterActivity.StartActivityOnUIThread(typeof(GameplayMultiplayerActivity));
        }


        /// <summary>
        /// Forcefully answers the question at position on activity. Used for multiplayer when other players answer.
        /// </summary>
        public void AnwserQuestionActivity(int position)
        {
            Activity.AnswerQuestion(position);
        }

        /// <summary>
        /// Checks if anwser is correct and updates score accordingly. 
        /// </summary>
        public override bool AnwserQuestion(Anwser anwser, int position)
        {
            string message = JsonConvert.SerializeObject(new List<string>() 
            {
                "Answer",
                position.ToString(),
                ContentController.GetInstance().IPAddress
            });
            
            if (IsHost)
            {
                Server.SendMessageToClients(message);
            }
            else
            {
                Client.SendMessage(message);
            }
            
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