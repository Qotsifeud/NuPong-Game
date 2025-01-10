using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpeedGate : NetworkBehaviour
{

    public NetworkVariable<Vector2> position = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private void Update()
    {
        if(!IsServer)
        {
            return;
        }

        position.Value = new Vector3(transform.position.x, ((float)Mathf.Sin(Time.time)) * 2);

        this.transform.position = position.Value;
    }
}
