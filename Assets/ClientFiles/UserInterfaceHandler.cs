using Bomberman.ClientFiles;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Bomberman.Assets.ClientFiles
{
    public class UserInterfaceHandler : MonoBehaviour
    {
        public void JoinAndHostButton(GameObject container)
        {
            var username = container.transform.GetChild(2).GetComponent<TMP_InputField>().text;
            var ipAddress = container.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            Debug.Log("Username: " + username);
            Debug.Log("IpAddress: " + ipAddress);
            Client.Instance.ConnectAndHost(username, ipAddress);
        }

        public void JoinButton(GameObject container)
        {
            var username = container.transform.GetChild(2).GetComponent<TMP_InputField>().text;
            var ipAddress = container.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            Client.Instance.Connect(username, ipAddress);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}
