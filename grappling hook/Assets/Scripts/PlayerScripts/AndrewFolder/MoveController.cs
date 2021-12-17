using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveController : MonoBehaviour
{
    [Header("Movement Components")]
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Stats")]
    [SerializeField] private float walkMoveMultiplier = 11f;
    [SerializeField] private float runMoveMultiplier = 11f;
    [SerializeField] [Range(0f, 1f)] private float accelerationRate;
    [SerializeField] [Range(0f, 1f)] private float decelerationRate;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve decelerationCurve;

    [SerializeField] private float lerpCurrent=0;
    

    [Header("Jumping Stats")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpVelocityFalloff = 2f;
    

    private float _velocityX;
    private bool _isMoveInput = false;
    private bool _isJumpInput = false;
    private bool _isGrounded = false;

    [SerializeField] private RunState _runState = RunState.Stopped;

    private float _wallGrabTimer = 0f;
    private bool _isWallGrabbing = false;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;

    private enum FacingDirection
    {
        Left = -1,
        Right = 1
    }

    private enum RunState
    {
        Stopped,
        Accelerating,
        Running,
        Decelerating,
        ChangingDirection
    }

    private FacingDirection _facingDirection = FacingDirection.Right;

    private bool _isParrying = false;
    private bool _isAttacking = false;

    #region Handle input

    // Update is called once per frame
    void Update()
    {
        if (_isAttacking || _isParrying)
        {
            return;
        }
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
        CheckIfGrabbedToWall();
        //ReadAttackInput();
        //ReadParryInput();
    }

    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
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

    private void CheckIfGrabbedToWall()
    {
        if (_isGrounded)
        {
            _wallGrabTimer = 0f;
        }

        Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
        Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
        if (!_isGrounded)
        {
            if (leftCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
            if (rightCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
        }

        if (!leftCollider && !rightCollider)
        {
            _isWallGrabbing = false;
        }

        if (_wallGrabTimer < wallGrabTimeLimit)
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
            if (Mathf.Sign(_velocityX) != Mathf.Sign(rb.velocity.x))
            {
                _runState = RunState.Accelerating;
            }
            switch (_runState)
            {
                case RunState.Stopped:
                    StartSpeedChange();
                    break;
                case RunState.Accelerating:
                    Accelerate();
                    break;
                case RunState.Running:
                    ApplyRun();
                    break;
                case RunState.Decelerating:
                    Accelerate();
                    break;
                case RunState.ChangingDirection:
                    break;
            }
        }else if(!_isMoveInput && _runState != RunState.Stopped)
        {
            if (_runState == RunState.Running)
            {
                StartSpeedChange();
            }

            Decelerate();
        }
    } 
    private void StartSpeedChange()
    {
        switch (_runState)
        {
            case RunState.Stopped:
                _runState = RunState.Accelerating;
                break;
            case RunState.Running:
                    _runState = RunState.Decelerating;
                break;
            default:
                Debug.Log("Invalid input to speed change");
                break;
        }
        lerpCurrent = 0;
    }
    private void Accelerate()
    {
        Debug.Log("accelerating");
        lerpCurrent = Mathf.MoveTowards(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(_velocityX * runMoveMultiplier, rb.velocity.y), accelerationCurve.Evaluate(lerpCurrent));
        if(Mathf.Abs(rb.velocity.x) >= runMoveMultiplier)
        {
            _runState = RunState.Running;
        }
    }

    private void ApplyRun()
    {
        lerpCurrent = 0f;
        Debug.Log("Runnig");
        rb.velocity = new Vector2(_velocityX*runMoveMultiplier,rb.velocity.y);
    }

    private void Decelerate()
    {
       Debug.Log("Decelerating");
       lerpCurrent = Mathf.MoveTowards(lerpCurrent, 1f, decelerationRate*  Time.deltaTime);
       rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0f, rb.velocity.y), decelerationCurve.Evaluate(lerpCurrent));
      if(rb.velocity.x == 0f)
        {
            Debug.Log("Stopped");
            _runState = RunState.Stopped;
        }
    }


    private void ApplyJump()
    {
        if (_isJumpInput)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            _isJumpInput = false;
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
        if (_isWallGrabbing)
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
