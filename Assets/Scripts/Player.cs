using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _jump = 600f;

    private bool onGround;
    void Start()
    {
        onGround = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // left and right movement
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * horizontalInput * _speed * Time.deltaTime);

        // Jump functionality if player is on the ground
        if (onGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(new Vector2(rb.velocity.x, _jump));
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player is on the ground
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Player is off the ground
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }
}
