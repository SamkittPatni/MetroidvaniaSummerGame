using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashingTime = 0.2f;
    private bool canDash = true;

    private float _fallMultiplier = 5f;
    private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _jump = 18f;

    [SerializeField] private float _direction = -1f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer tr;

    private bool onGround;
    void Start()
    {
        onGround = false;
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // left and right movement
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * horizontalInput * _speed * Time.deltaTime);
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            StartCoroutine(Dash());
            //rb.AddForce(new Vector2(_dashSpeed, 0));
        }
        if (onGround)
        {
            canDash = true;
        }
        // Jump functionality if player is on the ground
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jump);
        }

        // Altering gravity for faster fall
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        // Allowing for different jump heights
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Finding player direction
        if (Input.GetKeyDown("a"))
        {
            _direction = -1f;
        }
        if (Input.GetKeyDown("d"))
        {
            _direction = 1f;
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

    private IEnumerator Dash()
    {
        // Setting up dash
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        tr.emitting = true;

        // Dash
        rb.velocity = new Vector2(_direction * _dashSpeed, 0f);
        yield return new WaitForSeconds(_dashingTime);

        // Resetting parameters
        rb.gravityScale = originalGravity;
        tr.emitting = false;
        rb.velocity = new Vector2(0f, 0f);

        // Dash Cooldown
        yield return new WaitForSeconds(_dashingTime);
        if (onGround)
        {
            canDash = true;
        }
    }
}
