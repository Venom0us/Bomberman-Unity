using Bomberman.Libraries;
using Bomberman.ServerFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System;
using System.Collections;
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
            _client.Disconnected += ClientDisconnected;
        }

        private void ClientDisconnected(object sender, NetSockets.Client<byte>.ClientArgs e)
        {
            if (e != null)
            {
                // TODO: Show message in UI
            }
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
            message = null;
            if (username.Length > 20)
            {
                message = "Username too long, must be <= than 20 characters.";
                return false;
            }
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
            string errorMsg = null;
            Task.Run(() => 
            {
                try 
                {
                    _server.Run();
                } catch(Exception e)
                {
                    errorMsg = e.Message;
                }
            });

            // Wait until server is running correctly
            while (errorMsg == null && !_server.IsRunning)
            { }

            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                // TODO: Show error message on UI
                return;
            }

            if (_client.Connect(ip, Convert.ToInt32(port)))
            {
                Player = new Player(this, username, true);

                // Notify server we joined
                Notify(OpCodes.JoinServer, Player.Creation.Serialize(Player));
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

                // Notify server we joined
                Notify(OpCodes.JoinServer, Player.Creation.Serialize(Player));
            }
        }

        public void Notify(OpCodes opCode, string arguments = null)
        {
            _client.SendPacket(new Packet<byte>((byte)opCode, arguments));
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
                _client.Disconnect();
            if (_server != null && _server.IsRunning)
                _server.Shutdown();

            _server = null;
        }

        public void Transition(Scenes scene, Action executeActionAfterLoad = null)
        {
            StartCoroutine(InitializeScene(scene, executeActionAfterLoad));
        }

        private IEnumerator InitializeScene(Scenes scene, Action executeActionAfterLoad = null)
        {
            yield return StartCoroutine(WaitUntilSceneLoaded(scene, executeActionAfterLoad));
        }

        private IEnumerator WaitUntilSceneLoaded(Scenes scene, Action executeActionAfterLoad = null)
        {
            CurrentScene = scene;
            var callback = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

            while (!callback.isDone)
            {
                yield return null;
            }

            // Pass one frame so all start/awake methods are executed
            yield return new WaitForEndOfFrame();

            CurrentScene = scene;

            executeActionAfterLoad?.Invoke();
        }
    }
}
