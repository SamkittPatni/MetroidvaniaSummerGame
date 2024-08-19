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
    private bool dashTransparancy = false;
    private bool dashFinish = false;

    private float _fallMultiplier = 3f;
    private float _lowJumpMultiplier = 4f;
    [SerializeField] private float _jump = 18f;

    [SerializeField] private float _direction = -1f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer tr;

    [SerializeField] public Vector2 boxSize;
    [SerializeField] public float castDistance;
    [SerializeField] public LayerMask layerMask;

    private bool inputLock;

    private bool onGround;
    void Start()
    {
        inputLock = false;
        onGround = false;
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        // Movement Input
        float horizontalInput = Input.GetAxis("Horizontal");
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputRelease = Input.GetButtonUp("Jump");
        var dashInput = Input.GetButtonDown("Fire3");
        onGround = GroundCheck();

        // Left and right movement
        if (!inputLock)
        {
            transform.Translate(Vector2.right * horizontalInput * _speed * Time.deltaTime);
        }

        // Dash movement logic
        if (dashInput && canDash && !inputLock)
        {
            StartCoroutine(Dash());
        }

        if (onGround && !inputLock && dashFinish)
        {
            canDash = true;
        }


        // Jump functionality if player is on the ground
        if (jumpInput && onGround && !inputLock)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jump);
        }

        if (jumpInputRelease && rb.velocity.y > 0 && !inputLock)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        // Altering gravity for faster fall
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }

        // Finding player direction
        if (Input.GetKey("a") && !inputLock)
        {
            _direction = -1f;
        }
        if (Input.GetKey("d") && !inputLock)
        {
            _direction = 1f;
        }

    }

    private bool GroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * castDistance, boxSize);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
        // Player is on the ground
        //if (collision.gameObject.tag == "Ground")
        //{
            //onGround = true;
        //}
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
        // Player is off the ground
        //if (collision.gameObject.tag == "Ground")
        //{
            //onGround = false;
        //}
    //}

    private IEnumerator Dash()
    {
        // Locking input
        inputLock = true;
        dashFinish = false;
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

        // Unlocking input
        inputLock = false;

        // Dash Cooldown
        yield return new WaitForSeconds(_dashingTime * 1.5f);
        dashFinish = true;
    }
}
