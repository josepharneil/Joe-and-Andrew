using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerController : MonoBehaviour
{
    [Header("Config")]
    public Rigidbody2D rb;

    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;


    private Vector2 velocity;

    private bool isMoveInput = false;
    private bool isJumpInput = false;

    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
    public FacingDirection GetFacingDirection()
    {
        return facingDirection;
    }

    private FacingDirection facingDirection = FacingDirection.Right;
    [HideInInspector] public bool isGrounded = false;

    [SerializeField] private Transform isGroundedChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;

    // @JA TODO BAD BAD BAD CIRCULAR REFERENCE
    [SerializeField] private RL_PlayerControllerDash dashController;

    // Run
    // Jump
    // Dash / roll
    // Melee hit
    // Single wall jump
    // Parry


    // Update is called once per frame
    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
    }

    private void FixedUpdate()
    {
        ApplyMove();
        ApplyJump();
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0 && dashController.IsNotDashing())
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && dashController.IsNotDashing())
        {
            isJumpInput = true;
        }
    }





    private void ApplyMove()
    {
        if (isMoveInput)
        {
            rb.velocity = new Vector2(velocity.x * moveMultiplier, rb.velocity.y);
        }
    }

    private void ApplyJump()
    {
        if (isJumpInput)
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
