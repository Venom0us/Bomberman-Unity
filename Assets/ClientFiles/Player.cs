using Bomberman.Libraries;
using System;
using System.Net.Sockets;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class Player : IEquatable<Player>
    {
        public readonly TcpClient TcpClient;
        public readonly Client Client;
        public readonly string Username;
        public Vector3 Position;
        public readonly bool IsHost;
        public bool IsReady;

        public int LobbySlot = -1;
        public int BombermanIconSlot = -1;

        public Player(Client server, string username, bool isHost)
        {
            Client = server;
            Username = username;
            IsHost = isHost;
        }

        public Player(TcpClient client, string username, bool isHost)
        {
            TcpClient = client;
            Username = username;
            IsHost = isHost;
        }

        [Serializable]
        public class Creation
        {
            public string Username;
            public bool IsHost;

            public static string Serialize(Player player)
            {
                var creation = new Creation { Username = player.Username, IsHost = player.IsHost };
                return JsonUtility.ToJson(creation);
            }

            public static Creation Deserialize(string json)
            {
                return JsonUtility.FromJson<Creation>(json);
            }
        }

        [Serializable]
        public class LobbyState
        {
            public string Username;
            public bool IsReady;
            public int LobbyIndex;
            public int IconIndex;

            public static string Serialize(Player player)
            {
                var lobbyState = new LobbyState 
                { 
                    Username = player.Username, 
                    IsReady = player.IsReady,
                    LobbyIndex = player.LobbySlot,
                    IconIndex = player.BombermanIconSlot
                };
                return JsonUtility.ToJson(lobbyState);
            }

            public static LobbyState Deserialize(string json)
            {
                return JsonUtility.FromJson<LobbyState>(json);
            }

            public static string Serialize(Player[] players)
            {
                var state = new LobbyState[players.Length];
                for (int i = 0; i < players.Length; i++)
                    state[i] = new LobbyState
                    {
                        Username = players[i].Username,
                        IsReady = players[i].IsReady,
                        LobbyIndex = players[i].LobbySlot,
                        IconIndex = players[i].BombermanIconSlot
                    };
                return JsonHelper.ToJson(state);
            }

            public static T[] Deserialize<T>(string json)
            {
                return JsonHelper.FromJson<T>(json);
            }
        }

        public bool Equals(Player other)
        {
            return Username.Equals(other.Username, StringComparison.OrdinalIgnoreCase);
        }
    }
}
