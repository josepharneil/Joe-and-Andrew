using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerController : MonoBehaviour
{
    [Header("Config")]
    public Rigidbody2D rb;
    [SerializeField] private RL_PlayerStats playerStats;
    // @JA TODO BAD BAD BAD CIRCULAR REFERENCE
    [SerializeField] private RL_PlayerControllerDash dashController;

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

    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;

    private float wallGrabTimer = 0f;
    private bool isWallGrabbing = false;
    private float initialGravityScale = 0f;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;
    [SerializeField] private float wallGrabFallgGravityScale = 0.05f;


    // Run
    // Jump
    // Dash / roll
    // Melee hit
    // Single wall jump
    // Parry

    private void Awake()
    {
        initialGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!playerStats.IsPlayerDead())
        {
            HandleMoveInput();
            HandleJumpInput();
            CheckIfGrounded();
            CheckIfGrabbedToWall();
        }
    }

    private void FixedUpdate()
    {
        ApplyMove();
        ApplyJump();
        ApplyWallGrab();
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

    private void CheckIfGrabbedToWall()
    {
        if(isGrounded)
        {
            wallGrabTimer = 0f;
        }
        if(wallGrabTimer < wallGrabTimeLimit)
        {
            Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
            if(leftCollider)
            {
                isWallGrabbing = true;
                wallGrabTimer += Time.deltaTime;
            }
            Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
            if(rightCollider)
            {
                isWallGrabbing = true;
                wallGrabTimer += Time.deltaTime;
            }
            if(!leftCollider && !rightCollider)
            {
                isWallGrabbing = false;
            }
        }
    }

    private void CheckIfGrounded()
    {
        // NOTE @JA This should probably be a down raycast?? Maybe? Layer check is a bit dodge.
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, checkGroundRadius, groundLayer);

        if (collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void ApplyWallGrab()
    {
        if(isWallGrabbing)
        {
            if (wallGrabTimer > wallGrabTimeLimit)
            {
                // Slowly slide down
                float vel_x = 0f;
                float vel_y = rb.velocity.y;
                rb.velocity = new Vector2(vel_x, vel_y);
                rb.gravityScale = wallGrabFallgGravityScale;
            }
            else
            {
                // Stay grabbed (default behaviour)
            }
        }
        else
        {
            rb.gravityScale = initialGravityScale;
        }
    }

}
