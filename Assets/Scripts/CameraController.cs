using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public bool useOffsetValues;
    public float rotateSpeed;
    public Transform pivot;
    public float maxViewAngle;
    public float minViewAngle;
    public bool invertY;
    
    private float currentXRotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!useOffsetValues)
        {
            offset = target.position - transform.position;
        }

        pivot.transform.position = target.transform.position;
        //pivot.transform.parent = target.transform;
        pivot.transform.parent = null;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
            // Don't update camera if the game is paused
        if (GameManager.gm != null && GameManager.gm.pauseGame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // Lock and hide the cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        pivot.transform.position = target.transform.position;

        // Mouse input
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
        
        // Arrow key input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal -= rotateSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal += rotateSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vertical += rotateSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vertical -= rotateSpeed;
        }
        
        // Apply horizontal rotation
        pivot.Rotate(0, horizontal, 0);

        // Apply vertical rotation with invert option
        if (invertY)
        {
            currentXRotation += vertical;
        } else 
        {
            currentXRotation -= vertical;
        }

        // Clamp vertical rotation to prevent over-rotation
        currentXRotation = Mathf.Clamp(currentXRotation, minViewAngle, maxViewAngle);
        
        // Apply the clamped rotation
        pivot.rotation = Quaternion.Euler(currentXRotation, pivot.eulerAngles.y, 0);

        float desiredYAngle = pivot.eulerAngles.y;
        float desiredXAngle = pivot.eulerAngles.x;
        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = target.position - (rotation * offset);

        if (transform.position.y < target.position.y)
        {
            transform.position = new Vector3(transform.position.x, target.position.y - .5f, transform.position.z);
        }

        transform.LookAt(target);
    }
}
