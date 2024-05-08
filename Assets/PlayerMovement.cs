using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    public float xMin = -7.0f;
    public float xMax = 7.0f;
    public float zMin = -10.0f; 
    public float zMax = 4.0f;
    void Update()
    {
        
        Vector3 movement = new Vector3(0,0,0);

        if(Input.GetKeyDown(KeyCode.A) == true)
        {
            movement = new Vector3(-2, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.D) == true)
        {
            movement = new Vector3(2, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.W) == true)
        {
            movement = new Vector3(0, 0, 2);
        }
        else if(Input.GetKeyDown(KeyCode.S) == true)
        {
            movement = new Vector3(0, 0, -2);
        }
        Vector3 newPosition = transform.position + movement;

        newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
        newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);

        transform.position = newPosition;
    }
}