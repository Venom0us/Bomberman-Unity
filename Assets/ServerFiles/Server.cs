using Bomberman.ClientFiles;
using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Bomberman.ServerFiles
{
    public class Server : NetSockets.Server<byte>
    {
        private static Server _instance;
        public static Server Instance { get { return _instance; } }

        public readonly Random Random;

        private readonly PacketHandlerServer PacketHandler = new();
        public readonly Dictionary<TcpClient, Player> Players = new();

        public Server(string ip, int port) : base(ip, port, 8, Constants.MaxPacketSize, new UnityLogger())
        {
            if (_instance != null)
                throw new Exception("An instance of Server class already exists.");
            _instance = this;
            PacketReceived += PacketDispatcher;
            ClientDisconnected += Server_ClientDisconnected;
            Random = new Random();
        }

        private void Server_ClientDisconnected(object sender, EventArgs e)
        {
            var client = (TcpClient)sender;
            if (!Players.TryGetValue(client, out Player player)) return;

            if (LobbyManager.Server.LobbyQueue.Contains(player))
            {
                // Remove from lobby container
                LobbyManager.Server.Leave(player);
                UnityThread.Instance.Execute(() => LobbyManager.Client.Visualize());

                // Notify other players, of this player's departure from lobby
                var otherPlayers = GetOtherPlayers(client);
                foreach (var otherPlayer in otherPlayers)
                    SendPacket(otherPlayer.TcpClient, new Packet<byte>((byte)OpCodes.LeaveLobby, player.Username));
            }
            else if (LobbyManager.Server.InGame.Contains(player))
            {
                // TODO: Notify clients that this player left the game
            }
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
