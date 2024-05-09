using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 2.0f;
    public float xMin = -7.0f;
    public float xMax = 7.0f;
    public float zMin = -10.0f;
    public float zMax = 4.0f;

    void Update()
    {
        Vector3 movement = Vector3.zero;


        if (Input.GetKeyDown(KeyCode.A))
        {
            movement.x = -moveDistance;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            movement.x = moveDistance;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            movement.z = moveDistance;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            movement.z = -moveDistance;
        }

        Vector3 newPosition = transform.position + movement;

        bool isInTopLeftCorner = transform.position.x == xMin && transform.position.z == zMax;
        bool isInTopRightCorner = transform.position.x == xMax && transform.position.z == zMax;
        bool isInBottomLeftCorner = transform.position.x == xMin && transform.position.z == zMin;
        bool isInBottomRightCorner = transform.position.x == xMax && transform.position.z == zMin;

        if (isInTopLeftCorner)
        {
            if (movement.x > 0)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
            }
            if (movement.z < 0)
            {
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
            }
        }
        else if (isInTopRightCorner)
        {
            if (movement.x < 0)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
            }
            if (movement.z < 0)
            {
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
            }
        }
        else if (isInBottomLeftCorner)
        {
            if (movement.x > 0)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
            }
            if (movement.z > 0)
            {
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
            }
        }
        else if (isInBottomRightCorner)
        {
            if (movement.x < 0)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
            }
            if (movement.z > 0)
            {
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
            }
        }
        else
        {
            if (transform.position.z <= zMin || transform.position.z >= zMax)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
                newPosition.z = Mathf.Clamp(transform.position.z, zMin, zMax);
            }
            else if (transform.position.x <= xMin || transform.position.x >= xMax)
            {
                newPosition.z = Mathf.Clamp(newPosition.z, zMin, zMax);
                newPosition.x = Mathf.Clamp(transform.position.x, xMin, xMax);
            }
        }

        transform.position = newPosition;
    }
}
