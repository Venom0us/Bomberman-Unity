﻿using Bomberman.ClientFiles;
using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System.Collections.Generic;
using System.Linq;
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

            // Set slot indexes
            Player.LobbySlot = LobbyManager.Server.LobbyQueue.Count - 1;
            
            var validSlots = new List<int>();
            for (int i = 0; i < 8; i++)
                validSlots.Add(i);

            // Assign new bomberman icon
            var alreadyTakenSlots = LobbyManager.Server.LobbyQueue.Select(a => a.BombermanIconSlot).Where(a => a != -1).ToArray();
            validSlots.RemoveAll(a => alreadyTakenSlots.Contains(a));
            Player.BombermanIconSlot = validSlots[Server.Random.Next(0, validSlots.Count)];

            // Inform other connected clients
            foreach (var player in Server.GetOtherPlayers(Client))
            {
                Server.SendPacket(player.TcpClient, new Packet<byte>((byte)OpCodes.JoinLobby, Player.LobbyState.Serialize(Player)));
            }

            // Send new client, info about existing connected clients in lobby
            Server.SendPacket(Client, new Packet<byte>((byte)OpCodes.InitLobby, Player.LobbyState.Serialize(Server.Players.Values.ToArray())));
        }

        private void JoinServer(string arguments)
        {
            var args = Player.Creation.Deserialize(arguments);
            var userName = args.Username;
            var isHost = args.IsHost;

            // Add to player list + join lobby
            Server.Players.Add(Client, new Player(Client, userName, isHost));
            Player = Server.ClientToPlayer(Client);
            JoinLobby();
        }

        private void ReadyUp(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize(arguments);
            LobbyManager.Server.ReadyUp(lobbyState.Username, lobbyState.IsReady);

            // Inform other players of this player's readiness
            var otherPlayers = Server.GetOtherPlayers(Client);
            foreach (var otherPlayer in otherPlayers)
                Server.SendPacket(otherPlayer.TcpClient, new Packet<byte>((byte)OpCodes.ReadyUp, arguments));
        }
    }
}
