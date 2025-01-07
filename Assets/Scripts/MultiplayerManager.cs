using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public NetworkVariable<int> numberOfPlayers = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public TMP_Text awaitingPlayerText;
    public GameObject ballPrefab;

    public void HostGame()
    {
        NetworkManager.Singleton.StartHost();
        numberOfPlayers.Value += 1;

        awaitingPlayerText.gameObject.SetActive(true);
    }

    public void ClientJoin()
    {
        NetworkManager.Singleton.StartClient();
        numberOfPlayers.Value += 1;

        if(numberOfPlayers.Value == 2)
        {
            PlayersConnectedServerRpc();
        }
    }

    [ServerRpc]
    public void PlayersConnectedServerRpc()
    {
        awaitingPlayerText.gameObject.SetActive(false);

        GameObject instance = Instantiate(ballPrefab, new Vector3(0, 0, 0), new Quaternion());
        instance.GetComponent<NetworkObject>().Spawn();


        PlayersConnectedClientRpc();
    }

    [ClientRpc]
    public void PlayersConnectedClientRpc()
    {
        

        awaitingPlayerText.gameObject.SetActive(false);
    }
}
