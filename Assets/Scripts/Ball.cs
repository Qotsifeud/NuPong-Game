using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Events events;

    Rigidbody2D rb;

    Vector2 direction;

    [SerializeField]
    float moveSpeed = 6.0f;

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
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Blocker" || collision.gameObject.tag == "Boundary")
        {
            Vector2 newDirection = Vector2.Reflect(direction, collision.contacts[0].normal);

            direction = newDirection;

            moveSpeed += 0.2f;
        }
        else if(collision.gameObject.tag == "Goal")
        {
            moveSpeed = 4.0f;
            transform.position = new Vector2(0, 0);
            
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
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        if (UnityEngine.Random.Range(0, 1) == 0)
        {
            direction = new Vector2(-1, 0).normalized;
        }
        else
        {
            direction = new Vector2(1, 0).normalized;
        }

        return direction;
    }
}
