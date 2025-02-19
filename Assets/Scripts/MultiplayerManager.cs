using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public NetworkVariable<int> numberOfPlayers = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    bool hostLaunched = false;
    bool clientLaunched = false;

    public TMP_Text awaitingPlayerText;
    public GameObject ballPrefab;

    public GameObject testing;
    Events events;

    public void Awake()
    {
        events = GameObject.FindGameObjectWithTag("GameController").GetComponent<Events>();
    }

    public void Update()
    {

    }

    public void HostGame()
    {
        if (hostLaunched == false)
        {
            RNetworkManager.Singleton.StartServer();
            hostLaunched = true;
        }
    }

    public void ClientJoin()
    {

        if (clientLaunched == false)
        {
            RNetworkManager.Singleton.StartClient();
        }

        //if (numberOfPlayers.Value == 2 && clientLaunched == false)
        //{
        //    clientLaunched = true;
        //}
        //else
        //{
        //    Debug.Log("Failed to start client: " + "\n" + "Player Numbers: " + numberOfPlayers.Value + "\n");
        //}
    }

    
}
