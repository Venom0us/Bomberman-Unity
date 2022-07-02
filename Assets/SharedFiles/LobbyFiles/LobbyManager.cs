using Bomberman.ClientFiles;
using Bomberman.SharedFiles.Others;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bomberman.LobbyFiles
{
    public static class LobbyManager
    {
        public static ServerSide Server = new();
        public static ClientSide Client = new(Server);

        public class ClientSide
        {
            public readonly ServerSide Server;

            public ClientSide(ServerSide server)
            {
                Server = server;
            }

            public void Visualize()
            {
                foreach (var player in Server.LobbyQueue)
                {
                    // TODO: Show player in lobby UI
                }
            }
        }

        public class ServerSide
        {
            private readonly List<Player> _lobbyQueue = new();
            private readonly List<Player> _inGame = new();
            public IReadOnlyList<Player> LobbyQueue { get { return _lobbyQueue; } }
            public IReadOnlyList<Player> InGame { get { return _inGame; } }

            public void Join(Player player)
            {
                if (_lobbyQueue.Count == 8)
                {
                    // TODO: Show error message in UI
                    return;
                }
                if (_lobbyQueue.Contains(player)) return;
                player.IsReady = false;
                _lobbyQueue.Add(player);
                Debug.Log("Player joined: " + player.Username);
            }

            public void Leave(Player player)
            {
                player.IsReady = false;
                _lobbyQueue.Remove(player);
                Debug.Log("Player left: " + player.Username);
            }

            public void Start()
            {
                if (_lobbyQueue.Count(a => a.IsReady) < 2) return;

                var readyPlayers = _lobbyQueue.Where(a => a.IsReady).ToArray();
                foreach (var player in readyPlayers)
                {
                    _lobbyQueue.Remove(player);
                    player.Client.Notify(OpCodes.JoinGame);
                    _inGame.Add(player);
                }
            }

            public void EndGame()
            {
                foreach (var player in _inGame)
                {
                    player.Client.Notify(OpCodes.JoinLobby);
                    _lobbyQueue.Add(player);
                }
                _inGame.Clear();
            }
        }
    }
}
