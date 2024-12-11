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

    // Start is called before the first frame update
    void Start()
    {
        blockerPos.Value = this.transform.position;

        if(IsHost)
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
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (!touchingBtmBounds)
            {
                if (Input.GetKey(KeyCode.S) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.DownArrow) && this.gameObject.Equals(rightBlocker))
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
                if (Input.GetKey(KeyCode.W) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.UpArrow) && this.gameObject.Equals(rightBlocker))
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

            if (!touchingBtmBounds || !touchingTopBounds)
            {
                if (Input.GetKey(KeyCode.A) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.LeftArrow) && this.gameObject.Equals(rightBlocker))
                {
                    transform.Rotate(0, 0, 1 * spinSpeed * Time.deltaTime, Space.World);
                    updateNetworkPosition();
                }
                else if (Input.GetKey(KeyCode.D) && this.gameObject.Equals(leftBlocker) || Input.GetKey(KeyCode.RightArrow) && this.gameObject.Equals(rightBlocker))
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
