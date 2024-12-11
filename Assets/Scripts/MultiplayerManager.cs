using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ClientJoin()
    {
        NetworkManager.Singleton.StartClient();
    }
}
