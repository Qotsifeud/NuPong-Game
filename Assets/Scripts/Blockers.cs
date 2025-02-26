using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Blockers : NetworkBehaviour
{
    public float moveSpeed = 6.0f;
    public float spinSpeed = 200.0f;

    GameObject leftBlocker, rightBlocker;

    public GameObject downAnimEmitter, upAnimEmitter;

    public GameObject ballPrefab, sppedGatePrefab;

    NetworkVariable<Vector3> blockerPos = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<Quaternion> blockerRot = new NetworkVariable<Quaternion>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    private bool touchingBtmBounds = false;

    bool ballSpawned = false;

    [SerializeField]
    private bool touchingTopBounds = false;

    MultiplayerManager mpManager = new MultiplayerManager();
    Events events = new Events();

    // Start is called before the first frame update
    void Start()
    {
        if(IsServer)
        {
            leftBlocker = this.gameObject;
        }
        else if(IsClient)
        {
            rightBlocker = this.gameObject;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        mpManager = GameObject.Find("Multiplayer Manager").GetComponent<MultiplayerManager>();

        if (IsServer && IsOwner)
        {
            PlayerJoinedServerRpc();
            this.transform.position = new Vector3(-7, 0, 0);
            updateNetworkPosition();
        }
        else if (IsClient && IsOwner)
        {
            PlayerJoinedServerRpc();
            this.gameObject.transform.position = new Vector3(7, 0, 0);
            updateNetworkPosition();
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if(events.gameOver == true)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }

            if(IsServer)
            {

                if(mpManager.numberOfPlayers.Value == 2 && ballSpawned == false)
                {
                    Debug.Log("Function Called");
                    PlayersConnectedServerRpc();
                }
            }


            downAnimEmitter.SetActive(false);
            upAnimEmitter.SetActive(false);

            if (!touchingBtmBounds)
            {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.RightShift) && this.gameObject.Equals(rightBlocker))
                    {
                        transform.Translate(Vector2.down * (moveSpeed * 2) * Time.deltaTime, Space.World);
                        updateNetworkPosition();

                        if (touchingTopBounds)
                        {
                            touchingTopBounds = false;
                        }
                    }

                    transform.Translate(Vector2.down * moveSpeed * Time.deltaTime, Space.World);
                    updateNetworkPosition();

                    if (touchingTopBounds)
                    {
                        touchingTopBounds = false;
                    }

                    downAnimEmitter.SetActive(true);
                }
            }

            if (!touchingTopBounds)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.RightShift) && this.gameObject.Equals(rightBlocker))
                    {
                        transform.Translate(Vector2.up * (moveSpeed * 2) * Time.deltaTime, Space.World);
                        updateNetworkPosition();

                        if (touchingBtmBounds)
                        {
                            touchingBtmBounds = false;
                        }
                    }

                    transform.Translate(Vector2.up * moveSpeed * Time.deltaTime, Space.World);
                    updateNetworkPosition();

                    if (touchingBtmBounds)
                    {
                        touchingBtmBounds = false;
                    }

                    upAnimEmitter.SetActive(true);
                }
            }

            if (!touchingBtmBounds && gameObject.transform.rotation != new Quaternion(0, 0, 0, 0) || !touchingTopBounds && gameObject.transform.rotation != new Quaternion(0, 0, 0, 0))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(0, 0, 1 * spinSpeed * Time.deltaTime, Space.World);
                    updateNetworkPosition();
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(0, 0, -1 * spinSpeed * Time.deltaTime, Space.World);
                    updateNetworkPosition();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "topBounds")
        {
            touchingTopBounds = true;
            TouchingBoundariesServerRPC(1, this.OwnerClientId);
        }
        else if (collision.gameObject.name == "btmBounds")
        {
            touchingBtmBounds = true;
            TouchingBoundariesServerRPC(0, this.OwnerClientId);
        }
    }

    // Network Code
    //****************************************************
    private void updateNetworkPosition()
    {
        blockerPos.Value = transform.position;
        blockerRot.Value = transform.rotation;
    }

    [ServerRpc]
    private void TouchingBoundariesServerRPC(int boundaryNumber, ulong clientId)
    {
        TouchingBoundariesClientRPC(boundaryNumber);
    }

    [ClientRpc]
    private void TouchingBoundariesClientRPC(int boundaryNumber)
    {
        if(IsOwner)
        {
            return;
        }

        if (boundaryNumber == 0) 
        { 
            touchingBtmBounds = true; 
        }
        else 
        { 
            touchingTopBounds = false; 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerJoinedServerRpc()
    {
        mpManager.numberOfPlayers.Value += 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayersConnectedServerRpc()
    {
        //awaitingPlayerText.gameObject.SetActive(false);

        if (IsServer)
        {
            GameObject ballGameObject = Instantiate(ballPrefab, new Vector3(0, 0, 0), new Quaternion());
            ballGameObject.GetComponent<NetworkObject>().Spawn(true);

            GameObject speedGateObject = Instantiate(sppedGatePrefab, new Vector3(0, 0, 0), new Quaternion());
            speedGateObject.GetComponent<NetworkObject>().Spawn(true);

            Debug.Log("Function Called");
            ballSpawned = true;
        }


    }
}
