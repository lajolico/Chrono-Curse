using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    // Object Variables
    Rigidbody2D rb;
    Vector2 movement;
    public float playerSpeed = 0.2f;

    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

                // Animator
        myAnimator = GetComponent<Animator>();
    }

    void OnMove(InputValue iv) {
        movement = iv.Get<Vector2>();
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(rb.position + movement * playerSpeed * Time.deltaTime);
        if(GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            myAnimator.SetInteger("Status", 0);
        } else
        {
            myAnimator.SetInteger("Status", 1);
        }
    }
}