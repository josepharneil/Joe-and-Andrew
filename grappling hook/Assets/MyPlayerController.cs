using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    // https://craftgames.co/unity-2d-platformer-movement/

    [Header("Config")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveScale = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private Vector2 velocity;
    private bool isGrounded = false;

    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public GrappleScript grappleScript;


    // Update is called once per frame
    void Update()
    {
        if(!EffectorManager.Instance.CurrentEffectsDisablePlayerInput())
        {
            Move();
            Jump();
            BetterJump();
            CheckIfGrounded();
        }
    }

    private void Move()
    {
        if (!grappleScript.isGrappling)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            if (horizontalAxis != 0 )
            {
                velocity.x = horizontalAxis;
                rb.velocity = new Vector2(velocity.x * moveScale, rb.velocity.y);
            }
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
