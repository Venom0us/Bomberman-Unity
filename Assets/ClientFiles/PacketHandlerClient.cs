using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System.Net.Sockets;

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
                    JoinLobby();
                    break;
            }
        }

        private void JoinLobby()
        {
            LobbyManager.Server.Join(Client.Player);
            LobbyManager.Client.Visualize();
        }
    }
}
