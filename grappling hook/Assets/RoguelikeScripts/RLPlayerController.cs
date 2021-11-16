using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLPlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private float dashDuration = 0.3f;

    private float dashTimer = float.MaxValue;
    //private float dashHeight;

    //slide scaler is used to tweak how fast the slide rotates
    private Vector2 velocity;
    private bool isMoveInput = false;
    private bool isJumpInput = false;
    private bool isDashStart = false;
    private bool isDashing = false;
    private bool isDashEnd = false;
    private bool hasGroundedSinceLastDash = true;

    private FacingDirection facingDirection = FacingDirection.Right;


    private bool isGrounded = false;

    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;




    // Run
    // Jump
    // Dash / roll
    // Melee hit
    // Single wall jump
    // Parry


    enum FacingDirection
    {
        Left = -1,
        Right = 1
    }


    // Update is called once per frame
    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        HandleDashInput();
        CheckIfGrounded();
    }

    private void FixedUpdate()
    {
        ApplyDash();
        ApplyMove();
        ApplyJump();
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            if( horizontalAxis < 0)
            {
                facingDirection = FacingDirection.Left;
            }
            else
            {
                facingDirection = FacingDirection.Right;
            }
            isMoveInput = true;
            velocity.x = horizontalAxis;
        }
        else
        {
            isMoveInput = false;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumpInput = true;
        }
    }

    private void HandleDashInput()
    {
        if(isGrounded)
        {
            hasGroundedSinceLastDash = true;
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && hasGroundedSinceLastDash)
        {
            isDashStart = true;
            hasGroundedSinceLastDash = false;
        }
    }

    private void ApplyDash()
    {
        UpdateDashState();

        if (isDashStart)
        {
            ApplyStartDash();
        }

        if (isDashing)
        {
            ApplyUpdateDash();
        }

        if (isDashEnd)
        {
            ApplyEndDash();
        }
    }

    private void UpdateDashState()
    {
        bool isDashingThisUpdate = dashTimer < dashDuration;
        // If we were just dashing last update, but no longer dashing now,
        // We want to signify for the end of dashing.
        if (isDashing && !isDashingThisUpdate)
        {
            isDashEnd = true;
        }
        // Update whether we're currently dashing.
        isDashing = isDashingThisUpdate;
    }

    private void ApplyStartDash()
    {
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dashForce * (int)facingDirection, 0);
        isDashStart = false;
        dashTimer = 0f;
    }

    private void ApplyUpdateDash()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        dashTimer += Time.deltaTime;
    }

    private void ApplyEndDash()
    {
        rb.gravityScale = 1f;
        rb.velocity = new Vector2(0, 0);
        isDashEnd = false;
        dashTimer = float.MaxValue;
    }

    private void ApplyMove()
    {
        if (isMoveInput && !isDashing)
        {
            rb.velocity = new Vector2(velocity.x * moveMultiplier, rb.velocity.y);
        }
    }

    private void ApplyJump()
    {
        if (isJumpInput && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumpInput = false;
        }
        ApplyBetterJump();
    }

    private void ApplyBetterJump()
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
        // NOTE @JA This should probably be a down raycast?? Maybe? Layer check is a bit dodge.
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
