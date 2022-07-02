using Bomberman.Assets.ClientFiles;
using Bomberman.ClientFiles;
using Bomberman.SharedFiles.Others;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                var queue = Server.LobbyQueue;
                for (var slot=0; slot < queue.Count; slot++)
                {
                    var player = queue[slot];

                    // Retrieve UI data
                    var slotObj = UserInterfaceHandler.Instance.GetLobbySlot(slot);
                    var icon = slotObj.transform.GetChild(0).GetComponent<Image>();
                    var label = slotObj.transform.GetChild(1).GetComponent<TMP_Text>();

                    // Set username
                    label.text = player.Username;

                    // Setting ready state
                    label.color = player.IsReady ? Color.green : Color.red;

                    // Get valid random bomberman icon for this player
                    if (player.BombermanIconSlot == -1)
                    {
                        var validSlots = new List<int>();
                        for (int i = 0; i < 8; i++)
                            validSlots.Add(i);

                        // Assign new bomberman icon
                        var alreadyTakenSlots = queue.Select(a => a.BombermanIconSlot).Where(a => a != -1).ToArray();

                        validSlots.RemoveAll(a => alreadyTakenSlots.Contains(a));
                        player.BombermanIconSlot = validSlots[Random.Range(0, validSlots.Count)];
                    }

                    // Set bomberman icon
                    icon.color = UserInterfaceHandler.Instance.GetBombermanIconColor(player.BombermanIconSlot);

                    // Activate the slot
                    slotObj.SetActive(true);
                }

                // Deactivate slots that are not used
                var inactiveSlots = 8 - queue.Count;
                for (int i=0; i < inactiveSlots; i++)
                {
                    var slotObj = UserInterfaceHandler.Instance.GetLobbySlot(queue.Count + i);
                    slotObj.SetActive(false);
                }
            }
        }

        public class ServerSide
        {
            private readonly List<Player> _lobbyQueue = new();
            private readonly List<Player> _inGame = new();
            public IReadOnlyList<Player> LobbyQueue { get { return _lobbyQueue.ToArray(); } }
            public IReadOnlyList<Player> InGame { get { return _inGame.ToArray(); } }

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

            public void ReadyUp(string userName, bool isReady)
            {
                var player = _lobbyQueue.FirstOrDefault(a => a.Username.Equals(userName));
                if (player == null) return;
                player.IsReady = isReady;
                Debug.Log($"Player {(isReady ? "ready" : "not ready")}: {player.Username}");
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
