using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PlayerControllerCombatScene : MonoBehaviour
{
    [Header("Movement Components")]
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    [Header("Movement Stats")]
    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpVelocityFalloff = 2f;

    private float _velocityX = 0f;
    private bool _isMoveInput = false;
    private bool _isJumpInput = false;
    private bool _isGrounded = false;
    private float _wallGrabTimer = 0f;
    private bool _isWallGrabbing = false;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;

    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
    public FacingDirection facingDirection = FacingDirection.Right;

    // Attack bool.
    public bool IsAttacking { private get; set; }
    
    // Animation parameter IDs.
    private static readonly int SpeedID = Animator.StringToHash("speed");
    private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
    private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");


    #region Handle Input
    private void Update()
    {
        ReadAttackInput();
        if (IsAttacking)
        {
            _velocityX = 0f;
            return;
        }
        
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
        CheckIfGrabbedToWall();

        animator.SetFloat(SpeedID, Mathf.Abs(_velocityX));
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            if (horizontalAxis < 0)
            {
                sprite.flipX = true;
                facingDirection = FacingDirection.Left;
            }
            else
            {
                sprite.flipX = false;
                facingDirection = FacingDirection.Right;
            }
            
            _isMoveInput = true;
            _velocityX = horizontalAxis;
        }
        else
        {
            _velocityX = 0f;
            _isMoveInput = false;
        }
    }

    private void HandleJumpInput()
    {
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _isJumpInput = true;
        }
    }

    private void CheckIfGrabbedToWall()
    {
        if(_isGrounded)
        {
            _wallGrabTimer = 0f;
        }

        Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
        Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
        if(!_isGrounded)
        {
            if(leftCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
            if(rightCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
        }

        if(!leftCollider && !rightCollider)
        {
            _isWallGrabbing = false;
        }

        if(_wallGrabTimer < wallGrabTimeLimit)
        {
            _isWallGrabbing = false;
        }
    }

    private void CheckIfGrounded()
    {
        Collider2D overlapCircle = Physics2D.OverlapCircle(groundChecker.position, checkGroundRadius, groundLayer);
        _isGrounded = overlapCircle != null;
    }

    private void ReadAttackInput()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        animator.SetTrigger(AttackTriggerID);
        IsAttacking = true;
    }

    #endregion

    #region ApplyPhysics
    private void FixedUpdate()
    {
        ApplyMove();
        ApplyJump();
        ApplyWallGrab();
    }

    private void ApplyMove()
    {
        if (_isMoveInput)
        {
            rb.velocity = new Vector2(_velocityX * moveMultiplier, rb.velocity.y);
        }
    }

    private void ApplyJump()
    {
        if (_isJumpInput)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            _isJumpInput = false;
            animator.SetTrigger(JumpTriggerID);
        }
        // Down force when -ve y
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((fallMultiplier - 1) * Time.deltaTime);
        }
        // Additional down force when below a velocity threshold.
        else if (rb.velocity.y < jumpVelocityFalloff)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((fallMultiplier - 1) * Time.deltaTime);
        }
        // Low jump
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }

    private void ApplyWallGrab()
    {
        // TODO https://github.com/Matthew-J-Spencer/player-controller/blob/main/PlayerController.cs
        // Use that as a reference for wall grabbing.
        if(_isWallGrabbing)
        {
            if (_wallGrabTimer > wallGrabTimeLimit)
            {
                // Slowly slide down
                float velX = rb.velocity.x;
                const float velY = -12;
                rb.velocity = new Vector2(velX, velY);
            }
            else
            {
                // Stay grabbed (default behaviour)
            }
        }
    }

    #endregion
}
