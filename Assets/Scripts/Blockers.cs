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

    NetworkVariable<Vector3> blockerPos = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<Quaternion> blockerRot = new NetworkVariable<Quaternion>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    private bool touchingBtmBounds = false;

    [SerializeField]
    private bool touchingTopBounds = false;

    MultiplayerManager mpManager = new MultiplayerManager();

    // Start is called before the first frame update
    void Start()
    {

        mpManager = GameObject.Find("Multiplayer Manager").GetComponent<MultiplayerManager>();

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

        if (IsServer && IsOwner)
        {
            this.transform.position = new Vector3(-7, 0, 0);
            updateNetworkPosition();
        }
        else if (IsClient && IsOwner)
        {
            this.gameObject.transform.position = new Vector3(7, 0, 0);
            updateNetworkPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
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
}
