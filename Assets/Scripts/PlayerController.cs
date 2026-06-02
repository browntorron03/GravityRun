using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int moveSpeed;
    //public Rigidbody theRB;
    public float jumpForce;
    public float gravityScale;
    public CharacterController controller;

    public Animator anim;
    public Transform pivot;
    public float rotateSpeed;
    private Vector3 moveDirection;

    public GameObject playerModel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //theRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*theRB.linearVelocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, theRB.linearVelocity.y, Input.GetAxis("Vertical") * moveSpeed);

        if (Input.GetButtonDown("Jump"))
        {
            theRB.linearVelocity = new Vector3(theRB.linearVelocity.x, jumpForce, theRB.linearVelocity.z);
        }*/

        float yStore = moveDirection.y;
        // Use only WASD keys for player movement, arrow keys reserved for camera
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        if (Input.GetKey(KeyCode.W))
            verticalInput += 1f;
        if (Input.GetKey(KeyCode.S))
            verticalInput -= 1f;
        if (Input.GetKey(KeyCode.D))
            horizontalInput += 1f;
        if (Input.GetKey(KeyCode.A))
            horizontalInput -= 1f;
        
        moveDirection = (transform.forward * verticalInput) + (transform.right * horizontalInput);
        moveDirection = moveDirection.normalized * moveSpeed;
        moveDirection.y = yStore;

        if (controller.isGrounded) 
        {
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale);
        controller.Move(moveDirection * Time.deltaTime);

        if (horizontalInput != 0 || verticalInput != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        //anim.SetBool("IsGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)));
    }
}
