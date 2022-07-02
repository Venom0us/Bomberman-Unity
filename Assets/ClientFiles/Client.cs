using Bomberman.Libraries;
using Bomberman.ServerFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bomberman.ClientFiles
{
    public class Client : SingletonBehaviour<Client>
    {
        private NetSockets.Client<byte> _client;
        private Server _server;

        public bool IsConnected { get { return _client != null && _client.IsConnected; } }
        public Scenes CurrentScene { get; private set; } = Scenes.MainMenu;
        public Player Player { get; private set; }

        private readonly PacketHandlerClient PacketHandler = new();

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _client = new(Constants.MaxPacketSize, new UnityLogger())
            {
                AutoUpdate = false,
                HeartbeatInterval = 10
            };
            _client.PacketReceived += PacketDispatcher;
        }

        private void Update()
        {
            _client.Update();
        }

        private void PacketDispatcher(object sender, PacketArgs<byte> e)
        {
            PacketHandler.Dispatcher(e.Client, e.Packet);
        }

        private bool IsValidUsername(string username, out string message)
        {
            // TODO: Implement length limitation + check with existing usernames on the server
            message = null;
            return true;
        }

        public void ConnectAndHost(string username, string ipAddress)
        {
            if (!IsValidUsername(username, out string message))
            {
                // TODO: Show some error message on UI
                return;
            }

            var splittedIp = ipAddress.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (splittedIp.Length != 2) return;
            var ip = splittedIp[0];
            var port = Convert.ToInt32(splittedIp[1].Trim());

            try
            {
                _server = new Server(ip, port);
            }
            catch (Exception)
            {
                // TODO: Show some error message on UI
                return;
            }

            // Run server on seperate thread
            Task.Run(() => _server.Run());

            // Wait until server is running correctly
            while (!_server.IsRunning)
            { }

            if (_client.Connect(ip, Convert.ToInt32(port)))
            {
                Player = new Player(this, username, true);

                // Let server know we are joining the lobby
                Notify(OpCodes.JoinServer, Player.Creation.SerializeCreation(Player));

                // TODO: transition to lobby
            }
            else
            {
                _server.Shutdown();
            }
        }

        private void OnApplicationQuit()
        {
            if (_client.IsConnected)
                _client.Disconnect();
            if (_server != null && _server.IsRunning)
                _server.Shutdown();
        }

        public void Connect(string username, string ipAddress)
        {
            if (_client == null) return;

            var splittedIp = ipAddress.Split(':');
            if (splittedIp.Length != 2) return;
            var ip = splittedIp[0];
            var port = splittedIp[1].Trim();
            
            if (_client.Connect(ip, Convert.ToInt32(port)))
            {
                Player = new Player(this, username, false);

                // Let server know we are joining the lobby
                Notify(OpCodes.JoinServer, Player.Creation.SerializeCreation(Player));

                // TODO: transition to lobby
            }
        }

        public void Notify(OpCodes opCode, string arguments = null)
        {
            _client.SendPacket(new Packet<byte>((byte)opCode, arguments));
        }

        public void Transition(Scenes scene)
        {
            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
            CurrentScene = scene;
        }
    }
}
