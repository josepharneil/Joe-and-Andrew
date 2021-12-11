using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IState
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Data")]
    [SerializeField] private float _moveMultiplier = 11f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _wallGrabTimeLimit = 0.25f;

    //
    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
    public FacingDirection facingDirection = FacingDirection.Right;
    
    private bool _isMoveInput = false;
    private float _velocityX = 0f;
    
    private bool _isGrounded = false;
    private bool _isJumpInput = false;
    
    private float wallGrabTimer = 0f;
    private bool isWallGrabbing = false;

    
    
    public void OnEnter()
    {}

    public void Tick()
    {        
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
        CheckIfGrabbedToWall();
    }

    #region HandleInput
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
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _isJumpInput = true;
        }
    }    private void CheckIfGrabbedToWall()
    {
        if (_isGrounded)
        {
            wallGrabTimer = 0f;
        }

        Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
        Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
        if (!_isGrounded)
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

        if (wallGrabTimer < _wallGrabTimeLimit)
        {
            isWallGrabbing = false;
        }
    }

    private void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundChecker.position, checkGroundRadius, groundLayer);

        if (collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }
    #endregion


    public void FixedTick()
    {        
        ApplyMove();
        ApplyJump();
        ApplyWallGrab();
    }
    
    private void ApplyMove()
    {
        if (_isMoveInput)
        {
            _rb.velocity = new Vector2(_velocityX * _moveMultiplier, _rb.velocity.y);
        }
    }

    private void ApplyJump()
    {
        if (_isJumpInput)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            _isJumpInput = false;
        }
        ApplyBetterJump();
    }

    private void ApplyBetterJump()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rb.velocity += Vector2.up * Physics2D.gravity * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    
    private void ApplyWallGrab()
    {
        if (isWallGrabbing)
        {
            if (wallGrabTimer > _wallGrabTimeLimit)
            {
                // Slowly slide down
                float vel_x = _rb.velocity.x;
                float vel_y = -12;
                _rb.velocity = new Vector2(vel_x, vel_y);
            }
            else
            {
                // Stay grabbed (default behaviour)
            }
        }
    }

    public void OnExit()
    {}
    
    
}