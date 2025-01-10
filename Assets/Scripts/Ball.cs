using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    Events events;

    Rigidbody2D rb;

    NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<Vector2> ballPos = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> speedBoost = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    public NetworkVariable<float> moveSpeed = new NetworkVariable<float>(6.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> savedSpeed = new NetworkVariable<float>(6.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public AudioClip[] audioClips;
    public AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        events = GameObject.FindGameObjectWithTag("GameController").GetComponent<Events>();
        audio = this.GetComponent<AudioSource>();

        RandomStartingDirection();
        StartTimerServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)direction.Value * moveSpeed.Value * Time.deltaTime;

        if(events.gameOver)
        {
            this.GetComponent<NetworkObject>().Despawn(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Blocker" || collision.gameObject.tag == "Boundary")
        {
            PlaySoundServerRpc(0);

            Vector2 newDirection = Vector2.Reflect(direction.Value, collision.contacts[0].normal);

            direction.Value = newDirection;

            moveSpeed.Value += 0.2f;
        }
        else if(collision.gameObject.tag == "Blocker" && speedBoost.Value)
        {
            PlaySoundServerRpc(0);

            speedBoost.Value = false;
            moveSpeed.Value = savedSpeed.Value;
        }
        else if(collision.gameObject.tag == "Goal")
        {
            moveSpeed.Value = 6.0f;
            PlaySoundServerRpc(1);

            if (collision.gameObject.name == "leftGoal")
            {
                events.goalScored(0);
            }
            else
            {
                events.goalScored(1);
            }

            RandomStartingDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Speedgate")
        {
            activateSpeedBoost();
        }
    }

    Vector2 RandomStartingDirection()
    {
        int randomValue = UnityEngine.Random.Range(0, 2) * 2 - 1;

        if (randomValue == -1)
        {
            direction.Value = new Vector2(-1, 0).normalized;
            transform.position = new Vector2(-1, 0);
        }
        else
        {
            direction.Value = new Vector2(1, 0).normalized;
            transform.position = new Vector2(1, 0);
        }

        return direction.Value;
    }

    public void activateSpeedBoost()
    {
        speedBoost.Value = true;

        savedSpeed = moveSpeed;

        moveSpeed.Value = moveSpeed.Value * 1.5f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaySoundServerRpc(int soundIndex)
    {
        PlaySoundClientRpc(soundIndex);
    }

    [ClientRpc]
    private void PlaySoundClientRpc(int soundIndex)
    {
        audio.clip = audioClips[soundIndex];
        audio.Play();
    }

    [ServerRpc (RequireOwnership = false)]
    public void StartTimerServerRpc()
    {
        events.startTimer.Value = true;
    }
}
