using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(transform.position, -transform.right);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.right * 5, Color.white);

        if(Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log("HIT!");
        }
    }
}
