using Bomberman.ClientFiles;
using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System.Net.Sockets;

namespace Bomberman.ServerFiles
{
    public class PacketHandlerServer
    {
        private Player Player;
        private TcpClient Client;
        private Server Server { get { return Server.Instance; } }

        public void Dispatcher(TcpClient client, Packet<byte> packet)
        {
            Client = client;
            Player = Server.ClientToPlayer(Client);
            var opCode = (OpCodes)packet.OpCode;
            switch (opCode)
            {
                case OpCodes.JoinServer:
                    JoinServer(packet.Arguments);
                    break;
                case OpCodes.JoinLobby:
                    JoinLobby();
                    break;
                case OpCodes.ReadyUp:
                    ReadyUp(packet.Arguments);
                    break;
            }
        }

        private void JoinLobby()
        {
            LobbyManager.Server.Join(Player);

            // Inform other connected clients
            var otherPlayers = Server.GetOtherPlayers(Client);
            foreach (var player in otherPlayers)
            {
                Server.SendPacket(player.TcpClient, new Packet<byte>((byte)OpCodes.JoinLobby, Player.Username));
            }

            // Send new client, info about existing connected clients in lobby
            foreach (var player in otherPlayers)
                Server.SendPacket(Client, new Packet<byte>((byte)OpCodes.JoinLobby, player.Username));
        }

        private void JoinServer(string arguments)
        {
            var args = Player.Creation.DeserializeCreation(arguments);
            var userName = args.Username;
            var isHost = args.IsHost;

            // Add to player list + join lobby
            Server.Players.Add(Client, new Player(Client, userName, isHost));
            Player = Server.ClientToPlayer(Client);
            JoinLobby();
        }

        private void ReadyUp(string arguments)
        {
            var lobbyState = Player.LobbyState.DeserializeLobbyState(arguments);
            LobbyManager.Server.ReadyUp(lobbyState.Username, lobbyState.IsReady);

            // Inform other players of this player's readiness
            var otherPlayers = Server.GetOtherPlayers(Client);
            foreach (var otherPlayer in otherPlayers)
                Server.SendPacket(otherPlayer.TcpClient, new Packet<byte>((byte)OpCodes.ReadyUp, arguments));
        }
    }
}
