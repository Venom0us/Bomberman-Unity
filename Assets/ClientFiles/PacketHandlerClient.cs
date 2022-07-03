using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class PacketHandlerClient
    {
        private TcpClient Server;
        private Client Client { get { return Client.Instance; } }

        public void Dispatcher(TcpClient server, Packet<byte> packet)
        {
            Server = server;
            var opCode = (OpCodes)packet.OpCode;
            switch (opCode)
            {
                case OpCodes.JoinLobby:
                    JoinLobby(packet.Arguments);
                    break;
                case OpCodes.ReadyUp:
                    ReadyUp(packet.Arguments);
                    break;
                case OpCodes.LeaveLobby:
                    LeaveLobby(packet.Arguments);
                    break;
                case OpCodes.InitLobby:
                    InitLobby(packet.Arguments);
                    break;
                case OpCodes.JoinGame:
                    JoinGame(packet.Arguments);
                    break;
                case OpCodes.Move:
                    Move(packet.Arguments);
                    break;
            }
        }

        private void JoinGame(string arguments)
        {
            // Transition to the game scene
            Client.Transition(Scenes.Game, () =>
            {
                var readyPlayers = LobbyManager.Server.Start();

                // Spawn all players on grid
                var players = Player.Minimal.Deserialize<Player.Minimal>(arguments);
                var container = new GameObject("Bombermans");
                foreach (var player in players)
                {
                    var position = new Vector2(player.X, player.Y);
                    var bomberman = Object.Instantiate(PrefabContainer.Instance.Bomberman, position, Quaternion.identity);
                    bomberman.transform.parent = container.transform;

                    var match = readyPlayers.FirstOrDefault(a => a.Username.Equals(player.Username, System.StringComparison.OrdinalIgnoreCase));
                    if (match == null) continue;

                    match.Bomberman = bomberman;
                    match.Bomberman.Move(player.X, player.Y);
                }
            });
        }

        private void Move(string arguments)
        {
            // Convert coords
            var player = Player.Minimal.Deserialize(arguments);
            var match = LobbyManager.Server.InGame.FirstOrDefault(a => a.Username.Equals(player.Username, System.StringComparison.OrdinalIgnoreCase));
            if (match == null) return;

            var coordX = player.X;
            var coordY = player.Y;

            // Move bomberman
            match.Bomberman.Move(coordX, coordY);
        }

        private void LeaveLobby(string arguments)
        {
            var player = LobbyManager.Server.LobbyQueue.FirstOrDefault(a => a.Username.Equals(arguments));
            if (player == null) return;
            LobbyManager.Server.Leave(player);
            UpdateLobby();
        }

        private void ReadyUp(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize(arguments);
            LobbyManager.Server.ReadyUp(lobbyState.Username, lobbyState.IsReady);
            UpdateLobby();
        }

        private void JoinLobby(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize(arguments);

            Player player;
            if (Client.Player.Username.Equals(lobbyState.Username))
                player = Client.Player;
            else
                player = new Player((TcpClient)null, lobbyState.Username, false);

            player.LobbySlot = lobbyState.LobbyIndex;
            player.BombermanIconSlot = lobbyState.IconIndex;

            LobbyManager.Server.Join(player);
            UpdateLobbyPlayer(player);
        }

        private void InitLobby(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize<Player.LobbyState>(arguments);
            foreach (var state in lobbyState)
            {
                Player player;
                if (Client.Player.Username.Equals(state.Username))
                    player = Client.Player;
                else
                    player = new Player((TcpClient)null, state.Username, false);

                player.LobbySlot = state.LobbyIndex;
                player.BombermanIconSlot = state.IconIndex;
                LobbyManager.Server.Join(player);
            }

            // Load lobby scene
            Client.Transition(Scenes.Lobby, () =>
            {
                UpdateLobby();
            });
        }

        private void UpdateLobby()
        {
            UnityThread.Instance.Execute(() => LobbyManager.Client.Visualize());
        }

        private void UpdateLobbyPlayer(Player player)
        {
            UnityThread.Instance.Execute(() => LobbyManager.Client.VisualizePlayer(player));
        }
    }
}
