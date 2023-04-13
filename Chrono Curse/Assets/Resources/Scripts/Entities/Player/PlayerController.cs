using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


//Test our player object 
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;

    Rigidbody2D rb;

    public Transform playerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);
        rb.velocity = (movement * speed);
    }
}
