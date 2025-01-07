using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    Events events;

    Rigidbody2D rb;

    NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<Vector2> ballPos = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);



    [SerializeField]
    public float moveSpeed = 6.0f;
    float savedSpeed;

    public bool speedBoost = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        events = GameObject.FindGameObjectWithTag("GameController").GetComponent<Events>();


        RandomStartingDirection();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)direction.Value * moveSpeed * Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Blocker" || collision.gameObject.tag == "Boundary")
        {
            Vector2 newDirection = Vector2.Reflect(direction.Value, collision.contacts[0].normal);

            direction.Value = newDirection;

            moveSpeed += 0.2f;
        }
        else if(collision.gameObject.tag == "Blocker" && speedBoost)
        {
            speedBoost = false;
            moveSpeed = savedSpeed;
        }
        else if(collision.gameObject.tag == "Goal")
        {
            moveSpeed = 6.0f;
            
            if(collision.gameObject.name == "leftGoal")
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
        speedBoost = true;

        savedSpeed = moveSpeed;

        moveSpeed = moveSpeed * 1.5f;
    }
}
