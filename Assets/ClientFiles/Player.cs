using System.Net.Sockets;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class Player
    {
        public readonly TcpClient TcpClient;
        public readonly Client Client;
        public readonly string Username;
        public Vector3 Position;
        public readonly bool IsHost;
        public bool IsReady;

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
    }
}
