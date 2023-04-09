using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Test our player object 
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);
        rb.velocity = (movement * speed);
    }
}
