using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1

[RequireComponent(typeof( MoveController))]
public class PlayerInputs : MonoBehaviour
{
    [Header("Move Stats")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToJumpHeight = 0.4f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] [Range(0f,1f)] private float accelerationRate;
    [SerializeField] [Range(0f, 1f)] private float airAccelerationRate;
    [SerializeField] private AnimationCurve decelerationCurve;
    [SerializeField] [Range(0f, 1f)] private float decelerationRate;
    [SerializeField] [Range(0f, 1f)] private float airDecelerationRate;
    [SerializeField] private AnimationCurve changeDirectionCurve;
    [SerializeField] [Range(0f, 1f)] private float changeDirectionRate;
    [SerializeField] [Range(0f, 1f)] private float airChangeDirectionRate;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float rollDistance;
    [SerializeField] private float rollDuration;

    private float jumpCalledTime;
    private float lastGroundedTime;
    //gravity and jumpVelocity are calculated based on the jump height and time
    private float gravity;
    private float jumpVelocity;
    private float rollDirection;
   [SerializeField]  private float rollTimer =0f;

    private bool _isMoveInput;
    private bool _isJumpInput;
    private bool _isGrounded;
    private bool _isRollInput;
    private FacingDirection _facingDirection;
    private float lerpCurrent = 0f;
    MoveState _moveState;
    RollState _rollState;
    Vector3 velocity;
    Vector2 input;
    MoveController moveController;

    enum FacingDirection
    {
        Left = -1,
        Right = 1
    }

    enum MoveState
    {
        Stopped,
        Accelerating,
        Running,
        Decelerating,
        ChangingDirection,
        Rolling
    }

    enum RollState
    {
        StartRoll,
        Rolling,
        EndRoll,
        NotRolling
    }

    // Start is called before the first frame update
    void Start()
    {
        gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        jumpVelocity = timeToJumpHeight * Mathf.Abs(gravity);
        moveController = gameObject.GetComponent<MoveController>();
        _moveState = MoveState.Stopped;
        _rollState = RollState.NotRolling;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        HandleRollInput();
        HandleMoveInput();
        HandleJumpInput();
        SetHorizontalMove();
        CheckGrounded();
        ApplyGravity();
        Jump();
        moveController.Move(velocity * Time.deltaTime);
        //move works by taking in a displacement, firing raycasts in the directions of the displacement
        //then if the raycasts collide with anything the displacement is altered to be the distance from the player edge to the collider
        //then at the end of controller it uses transform.translate(displacement) with the edited displacement 
    }

    #region Jump Movement

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJumpInput = true;
            StartCoyoteTime();
        }
        else
        {
            _isJumpInput = false;
        }
    }

    void Jump()
    {
        if (_isJumpInput && (_isGrounded || (jumpCalledTime - lastGroundedTime < coyoteTime)))
        {
            velocity.y = jumpVelocity;
            jumpCalledTime = float.MaxValue;
        }
    }

    void ApplyGravity()
    {
        //this makes sure that the gravity is always properly applied
        //if its in an else of this if it really fucks up the jumping

        if (moveController.collisions.below || moveController.collisions.above)
        {
            velocity.y = 0;
        }
            velocity.y += gravity*Time.deltaTime;
    }

    void CheckGrounded()
    {
        if(moveController.CheckGrounded())
        {
            _isGrounded = true;
            lastGroundedTime = Time.time;
            jumpCalledTime = 0f;
        }
        else
        {
            _isGrounded = false;
        }
    }

    void StartCoyoteTime()
    {
        if (_isJumpInput&&jumpCalledTime!=float.MaxValue)
        {
            jumpCalledTime = Time.time;
        }
    }

    #endregion

    #region Horizontal Movement
    void HandleMoveInput()
    {
        if (input.x != 0)
        {
            _isMoveInput = true;
            _facingDirection = input.x < 0 ? FacingDirection.Left : FacingDirection.Right;
        }
        else
        {
            _isMoveInput = false;
        }
    }

    void SetHorizontalMove()
    {
        if (_isRollInput)
        {
            StartRoll();
        }
        if (_rollState != RollState.NotRolling)
        {
            switch (_rollState)
            {
                case RollState.Rolling:
                    Roll();
                    break;
                case RollState.EndRoll:
                    StopRoll();
                    break;
                case RollState.StartRoll:
                    break;
            }
            return;
        }
        if (_isMoveInput)
        {
            if (Mathf.Sign(velocity.x) != input.x && velocity.x!=0)
            {
                StartDirectionChange();
            }
            switch (_moveState)
            {
                case MoveState.Stopped:
                    //begins the movement, calls sets to accelerating
                    StartMoving();
                    break;
                case MoveState.Accelerating:
                    //accelerates to run speed, sets to moveState to run once at speed
                    Accelerate();
                    break;
                case MoveState.Decelerating:
                    //this will be called if the player starts decelerating and then wants to move again
                    StartMoving();
                    break;
                case MoveState.Running:
                    //continues moving at the current speed
                    Run();
                    break;
                case MoveState.ChangingDirection:
                    //changes the speed to the opposite one
                    ChangeDirection();
                    break;
            }
        }
        else
        {
            if (_moveState != MoveState.Stopped)
            {
                StopMoving();
                Decelerate();
            }
        }
    }

    void StartMoving()
    {
        lerpCurrent = 0f;
        _moveState = MoveState.Accelerating;
    }

    void StopMoving()
    {
        lerpCurrent = 0f;
        _moveState = MoveState.Decelerating;
    }

    void StartDirectionChange()
    {
        //resets the lerp to 0 each time we change direction
        lerpCurrent = 0f;
        _moveState = MoveState.ChangingDirection;
    }

    void Accelerate()
    {
        //uses a lerp which is then used to evaluate along an animation curve for the acceleration
        //once we get to the max speed change to running
        //checks if there is a collision below the player, and if so use the air timers
        float rate = (moveController.collisions.below ? accelerationRate : airAccelerationRate);
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, rate * Time.deltaTime);
        velocity.x = Mathf.Lerp(velocity.x, moveSpeed * input.x, accelerationCurve.Evaluate(lerpCurrent));
        if (Mathf.Abs(velocity.x)*input.x >= input.x * moveSpeed)
        {
            _moveState = MoveState.Running;
        }
    }

    void Run()
    {
        velocity.x = input.x * moveSpeed;
    }

    void Decelerate()
    {
        //same lerp method as accelerate
        //this time changes to stopped after getting low enough 
        //(I tried doing if(speed==0) but that was glitchy af
        float rate = moveController.collisions.below ? decelerationRate : airDecelerationRate;
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, rate * Time.deltaTime);
        velocity.x = Mathf.Lerp(velocity.x, 0f, decelerationCurve.Evaluate(lerpCurrent));
        if (Mathf.Abs(velocity.x) <=0.5f)
        {
            velocity.x = 0f;
            _moveState = MoveState.Stopped;
        }
    }

    void ChangeDirection()
    {
        //same lerp method as accelerate
        float rate = moveController.collisions.below ? changeDirectionRate : airChangeDirectionRate;
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, rate * Time.deltaTime);
        velocity.x = Mathf.Lerp(velocity.x, moveSpeed*input.x ,changeDirectionCurve.Evaluate(lerpCurrent));
        if (Mathf.Abs(velocity.x)*input.x == input.x * moveSpeed)
        {
            _moveState = MoveState.Running;
        }
    }

    void HandleRollInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isRollInput = true;
        }
        else
        {
            _isRollInput = false;
        }
    }

    void StartRoll()
    {
        //starts the roll timer and does the enums, could be state machine for animation purposes?
        //roll ovverides other movement
        //can only do while grounded for now, but we could change this later on 
        if (_isGrounded)
        {
            _moveState = MoveState.Rolling;
            rollTimer = 0f;
            _rollState = RollState.Rolling;
            rollDirection = (float)_facingDirection;
        }
    }

    void Roll()
    {
        //keeps rolling while the timer is on
        if (rollTimer <= rollDuration)
        {
            velocity.x = rollDistance * (int)rollDirection / rollDuration;
            rollTimer += Time.deltaTime;
        }
        else
        {
            _rollState = RollState.EndRoll;
        }
    }

    //TODO: Make roll stopping feel better
    void StopRoll()
    {
        //stops the player dead, this doesn't feel great tho going to have another look once the enemy AI is done
        _moveState = MoveState.Stopped;
        velocity.x = 0f;
        _rollState = RollState.NotRolling;
    }
    #endregion

}
