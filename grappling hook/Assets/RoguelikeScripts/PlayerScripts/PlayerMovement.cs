using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Setup")]
    public Rigidbody2D rb;
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Setup")]
    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private float velocityX;

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


    private float wallGrabTimer = 0f;
    private bool isWallGrabbing = false;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;

    // Update is called once per frame
    public void MovementUpdate()
    {
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
        CheckIfGrabbedToWall();
    }

    public void MovementFixedUpdate()
    {
        ApplyMove();
        ApplyJump();
        ApplyWallGrab();
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            if (horizontalAxis < 0)
            {
                facingDirection = FacingDirection.Left;
            }
            else
            {
                facingDirection = FacingDirection.Right;
            }
            isMoveInput = true;
            velocityX = horizontalAxis;
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


    private void ApplyMove()
    {
        if (isMoveInput)
        {
            rb.velocity = new Vector2(velocityX * moveMultiplier, rb.velocity.y);
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
        if (isGrounded)
        {
            wallGrabTimer = 0f;
        }

        Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
        Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
        if (!isGrounded)
        {
            if (leftCollider)
            {
                isWallGrabbing = true;
                wallGrabTimer += Time.deltaTime;
            }
            if (rightCollider)
            {
                isWallGrabbing = true;
                wallGrabTimer += Time.deltaTime;
            }
        }
        else
        {

        }

        if (!leftCollider && !rightCollider)
        {
            isWallGrabbing = false;
        }

        if (wallGrabTimer < wallGrabTimeLimit)
        {
            isWallGrabbing = false;
        }
    }

    private void CheckIfGrounded()
    {
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
        if (isWallGrabbing)
        {
            if (wallGrabTimer > wallGrabTimeLimit)
            {
                // Slowly slide down
                float vel_x = rb.velocity.x;
                float vel_y = -12;
                rb.velocity = new Vector2(vel_x, vel_y);
            }
            else
            {
                // Stay grabbed (default behaviour)
            }
        }
    }

}
