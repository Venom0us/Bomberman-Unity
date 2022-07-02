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

        public class Creation
        {
            public string Username;
            public bool IsHost;

            public static string SerializeCreation(Player player)
            {
                var creation = new Creation { Username = player.Username, IsHost = player.IsHost };
                return JsonUtility.ToJson(creation);
            }

            public static Creation DeserializeCreation(string json)
            {
                return JsonUtility.FromJson<Creation>(json);
            }
        }

        public class LobbyState
        {
            public string Username;
            public bool IsReady;

            public static string SerializeLobbyState(Player player)
            {
                var lobbyState = new LobbyState { Username = player.Username, IsReady = player.IsReady };
                return JsonUtility.ToJson(lobbyState);
            }

            public static LobbyState DeserializeLobbyState(string json)
            {
                return JsonUtility.FromJson<LobbyState>(json);
            }
        }

        public bool Equals(Player other)
        {
            return Username.Equals(other.Username);
        }
    }
}
