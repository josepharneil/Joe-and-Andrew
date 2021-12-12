using System;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("Movement Components")]
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private PlayerDodgeRoll dodgeRoll;
 
    [Header("Movement Stats")]
    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    
    private float _velocityX;
    private bool _isMoveInput = false;
    private bool _isJumpInput = false;
    private bool _isRollInput = false;
    private bool _isGrounded = false;
    
    private float _wallGrabTimer = 0f;
    private bool _isWallGrabbing = false;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;

    private enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
    private FacingDirection _facingDirection = FacingDirection.Right;

    
    
    [Header("Weapons")]
    private IMeleeWeapon _currentWeapon;
    [SerializeField] private SwordWeapon swordWeapon;
    private bool _isAttacking = false;
    
    private void Awake()
    {
        _currentWeapon = swordWeapon;
    }

    #region Handle Input
    private void Update()
    {
        if (!_isAttacking)
        {
            HandleMoveInput();
            HandleJumpInput();
            //HandleRollInput();
            CheckIfGrounded();
            CheckIfGrabbedToWall();
            ReadAttackInput();
        }
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0 && dodgeRoll.rollState == PlayerDodgeRoll.RollState.NotRolling)
        {
            _facingDirection = horizontalAxis < 0 ? FacingDirection.Left : FacingDirection.Right;
            _isMoveInput = true;
            _velocityX = horizontalAxis;
        }
        else
        {
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

    private void HandleRollInput()
    {
        if(_isGrounded && Input.GetKeyDown(KeyCode.S))
        {
            _isRollInput = true;
        }
        else
        {
            _isRollInput = false;
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
        // NOTE @JA This should probably be a down raycast?? Maybe? Layer check is a bit dodge.
        Collider2D overlapCircle = Physics2D.OverlapCircle(groundChecker.position, checkGroundRadius, groundLayer);
        _isGrounded = overlapCircle != null;
    }

    private void ReadAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _isAttacking = true;
            _currentWeapon.StartLightAttack(_facingDirection == FacingDirection.Left,
                () => _isAttacking = false);
            _velocityX = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _isAttacking = true;
            _currentWeapon.StartHeavyAttack(_facingDirection == FacingDirection.Left,
                () => _isAttacking = false);
            _velocityX = 0f;
        }
    }
    #endregion

    #region ApplyPhysics
    private void FixedUpdate()
    {
        ApplyMove();
        //ApplyRoll();
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
        }
        // Better jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((fallMultiplier - 1) * Time.deltaTime);
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }

    private void ApplyWallGrab()
    {
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

    /*private void ApplyRoll()
    {
        switch (dodgeRoll.rollState)
        {
            case PlayerDodgeRoll.RollState.NotRolling:
                if (_isRollInput)
                {
                    dodgeRoll.DoRoll(_isGrounded);
                    _isRollInput = false;
                }
                break;
            case PlayerDodgeRoll.RollState.Start:
                dodgeRoll.StartRoll(_facingDirection);
                break;
            case PlayerDodgeRoll.RollState.Rolling:
                dodgeRoll.UpdateRoll(_facingDirection);
                break;
            case PlayerDodgeRoll.RollState.End:
                dodgeRoll.EndRoll();
                break;
        }
    }*/

    #endregion
}
