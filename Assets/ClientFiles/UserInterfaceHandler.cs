using Bomberman.ClientFiles;
using Bomberman.Libraries;
using Bomberman.LobbyFiles;
using Bomberman.SharedFiles.Others;
using TMPro;
using UnityEngine;

namespace Bomberman.ClientFiles
{
    public class UserInterfaceHandler : SingletonBehaviour<UserInterfaceHandler>
    {
        [SerializeField]
        private GameObject[] _lobbySlots;
        [SerializeField]
        private Color[] _bombermanIconColors;
        [SerializeField]
        private TMP_Text _errorText;

        public GameObject GetLobbySlot(int slotIndex)
        {
            return _lobbySlots[slotIndex];
        }

        public Color GetBombermanIconColor(int slotIndex)
        {
            return _bombermanIconColors[slotIndex];
        }

        public void SetErrorMessage(string msg)
        {
            if (_errorText == null) return;
            _errorText.text = msg;
        }

        public void JoinAndHostButton(GameObject container)
        {
            var username = container.transform.GetChild(2).GetComponent<TMP_InputField>().text;
            var ipAddress = container.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            Client.Instance.ConnectAndHost(username, ipAddress);
        }

        public void JoinButton(GameObject container)
        {
            var username = container.transform.GetChild(2).GetComponent<TMP_InputField>().text;
            var ipAddress = container.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            Client.Instance.Connect(username, ipAddress);
        }

        public void ReadyUpButton()
        {
            Client.Instance.Player.IsReady = !Client.Instance.Player.IsReady;
            LobbyManager.Server.ReadyUp(Client.Instance.Player.Username, Client.Instance.Player.IsReady);
            LobbyManager.Client.Visualize();

            // Notify server to tell other clients about the ready state of this player
            Client.Instance.Notify(OpCodes.ReadyUp, Player.LobbyState.Serialize(Client.Instance.Player));
        }

        public void LeaveLobbyButton()
        {
            Client.Instance.Disconnect();
            Client.Instance.Transition(Scenes.MainMenu);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}
