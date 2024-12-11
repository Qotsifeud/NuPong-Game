using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGate : MonoBehaviour
{

    Ball ball = new Ball();

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
    }

    private void Update()
    {
        this.transform.position = new Vector3(transform.position.x, ((float)Mathf.Sin(Time.time)) * 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "ball")
        {
            ball.activateSpeedBoost();
        }
    }
}
