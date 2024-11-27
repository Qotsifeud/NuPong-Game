using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blockers : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float spinSpeed = 200.0f;


    [SerializeField]
    private bool touchingBtmBounds = false;

    [SerializeField]
    private bool touchingTopBounds = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!touchingBtmBounds)
        {
            if (Input.GetKey(KeyCode.S) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.DownArrow) && this.gameObject.name == "rightBlocker")
            {
                if (Input.GetKey(KeyCode.LeftShift) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.RightShift) && this.gameObject.name == "rightBlocker")
                {
                    transform.Translate(Vector2.down * (moveSpeed * 2) * Time.deltaTime, Space.World);

                    if (touchingTopBounds)
                    {
                        touchingTopBounds = false;
                    }
                }

                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime, Space.World);

                if (touchingTopBounds)
                {
                    touchingTopBounds = false;
                }
            }
        }
        
        if(!touchingTopBounds)
        {
            if (Input.GetKey(KeyCode.W) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.UpArrow) && this.gameObject.name == "rightBlocker")
            {
                if (Input.GetKey(KeyCode.LeftShift) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.RightShift) && this.gameObject.name == "rightBlocker")
                {
                    transform.Translate(Vector2.up * (moveSpeed * 2) * Time.deltaTime, Space.World);

                    if (touchingBtmBounds)
                    {
                        touchingBtmBounds = false;
                    }
                }

                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime, Space.World);

                if (touchingBtmBounds)
                {
                    touchingBtmBounds = false;
                }
            }
        }

        if(!touchingBtmBounds || !touchingTopBounds)
        {
            if (Input.GetKey(KeyCode.A) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.LeftArrow) && this.gameObject.name == "rightBlocker")
            {
                transform.Rotate(0, 0, 1 * spinSpeed * Time.deltaTime, Space.World);
            }
            else if (Input.GetKey(KeyCode.D) && this.gameObject.name == "leftBlocker" || Input.GetKey(KeyCode.RightArrow) && this.gameObject.name == "rightBlocker")
            {
                transform.Rotate(0, 0, -1 * spinSpeed * Time.deltaTime, Space.World);
            }
        }
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "topBounds")
        {
            touchingTopBounds = true;
        }
        else if (collision.gameObject.name == "btmBounds")
        {
            touchingBtmBounds = true;
        }
    }
}
