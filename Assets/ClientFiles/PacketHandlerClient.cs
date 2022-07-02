using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using NetSockets.PacketHandling;
using System.Linq;
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
                    JoinLobby(packet.Arguments);
                    break;
                case OpCodes.ReadyUp:
                    ReadyUp(packet.Arguments);
                    break;
                case OpCodes.LeaveLobby:
                    LeaveLobby(packet.Arguments);
                    break;
                case OpCodes.InitLobby:
                    InitLobby(packet.Arguments);
                    break;
            }
        }

        private void LeaveLobby(string arguments)
        {
            var player = LobbyManager.Server.LobbyQueue.FirstOrDefault(a => a.Username.Equals(arguments));
            if (player == null) return;
            LobbyManager.Server.Leave(player);
            UpdateLobby();
        }

        private void ReadyUp(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize(arguments);
            LobbyManager.Server.ReadyUp(lobbyState.Username, lobbyState.IsReady);
            UpdateLobby();
        }

        private void JoinLobby(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize(arguments);

            Player player;
            if (Client.Instance.Player.Username.Equals(lobbyState.Username))
                player = Client.Instance.Player;
            else
                player = new Player((TcpClient)null, lobbyState.Username, false);

            player.LobbySlot = lobbyState.LobbyIndex;
            player.BombermanIconSlot = lobbyState.IconIndex;

            LobbyManager.Server.Join(player);
            UpdateLobbyPlayer(player);
        }

        private void InitLobby(string arguments)
        {
            var lobbyState = Player.LobbyState.Deserialize<Player.LobbyState>(arguments);
            foreach (var state in lobbyState)
            {
                Player player;
                if (Client.Instance.Player.Username.Equals(state.Username))
                    player = Client.Instance.Player;
                else
                    player = new Player((TcpClient)null, state.Username, false);

                player.LobbySlot = state.LobbyIndex;
                player.BombermanIconSlot = state.IconIndex;
                LobbyManager.Server.Join(player);
            }
            UpdateLobby();
        }

        private void UpdateLobby()
        {
            UnityThread.Instance.Execute(() => LobbyManager.Client.Visualize());
        }

        private void UpdateLobbyPlayer(Player player)
        {
            UnityThread.Instance.Execute(() => LobbyManager.Client.VisualizePlayer(player));
        }
    }
}
