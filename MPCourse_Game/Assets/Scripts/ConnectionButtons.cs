using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }
}
