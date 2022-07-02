using Bomberman.ClientFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Bomberman.ServerFiles
{
    public class Server : NetSockets.Server<byte>
    {
        private static Server _instance;
        public static Server Instance { get { return _instance; } }

        private readonly PacketHandlerServer PacketHandler = new();
        public readonly Dictionary<TcpClient, Player> Players = new();

        public Server(string ip, int port) : base(ip, port, 8, Constants.MaxPacketSize, new UnityLogger())
        {
            if (_instance != null)
                throw new Exception("An instance of Server class already exists.");
            _instance = this;
            PacketReceived += PacketDispatcher;
        }

        private void PacketDispatcher(object sender, PacketArgs<byte> e)
        {
            PacketHandler.Dispatcher(e.Client, e.Packet);
        }

        public Player ClientToPlayer(TcpClient client)
        {
            Players.TryGetValue(client, out Player player);
            return player;
        }

        public IEnumerable<Player> GetOtherPlayers(TcpClient current)
        {
            foreach (var kvp in Players)
            {
                if (kvp.Key.Equals(current)) continue;
                yield return kvp.Value;
            }
        }
    }
}
