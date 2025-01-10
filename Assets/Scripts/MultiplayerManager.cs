using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
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
        if(events.gameOver)
        {
            if (IsServer)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("MP Manager Spawned");
    }

    public void HostGame()
    {
        if (hostLaunched == false)
        {
            NetworkManager.Singleton.StartHost();
            hostLaunched = true;
        }
    }

    public void ClientJoin()
    {

        if (clientLaunched == false)
        {
            NetworkManager.Singleton.StartClient();
        }

        if (numberOfPlayers.Value == 2 && clientLaunched == false)
        {
            clientLaunched = true;
        }
        else
        {
            Debug.Log("Failed to start client: " + "\n" + "Player Numbers: " + numberOfPlayers.Value + "\n");
        }
    }

    
}
