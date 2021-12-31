using System;
using UnityEngine;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1

[RequireComponent(typeof(MoveController))]
public class PlayerInputs : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MoveController moveController;

    [Header("Move Stats")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToJumpHeight = 0.4f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] [Range(0f, 1f)] private float accelerationRate;
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

    private float _jumpCalledTime;
    private float _lastGroundedTime;
    //gravity and jumpVelocity are calculated based on the jump height and time
    private float _gravity;
    private float _jumpVelocity;
    private float _rollDirection;
    [SerializeField] private float rollTimer = 0f;

    private bool _isMoveInput;
    private bool _isJumpInput;
    private bool _isGrounded;
    private bool _isRollInput;
    [HideInInspector] public FacingDirection facingDirection;
    private float _lerpCurrent = 0f;
    private MoveState _moveState;
    private RollState _rollState;
    private Vector3 _velocity;
    private Vector2 _moveInput;
    
    // Attacks
    [Header("Attacking")] [SerializeField] private bool debugUseAnimations = true;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite defaultSquareSprite;
    //[SerializeField] private PlayerCombat playerCombat;
    [HideInInspector] public bool isAttacking;

    // Animation parameter IDs.
    private static readonly int SpeedID = Animator.StringToHash("speed");
    private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
    private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
    private static readonly int AttackUpTriggerID = Animator.StringToHash("attackUpTrigger");
    private static readonly int AttackDownTriggerID = Animator.StringToHash("attackDownTrigger");
    
    
    
    // Attack speed type
    private enum PrototypeAttackSpeedStyle
    {
        HollowKnight,
        Middling,
        DarkSouls,
    }

    // What can cancel attacks?
    [Flags] private enum PrototypeCancellables
    {
        None = 0,
        Movement = 1 << 0,
        Roll = 1 << 1,
        Jump = 1 << 2,
    }

    [Serializable] private struct PrototypeAttackCustomisation
    {
        [Tooltip("Is movement disabled by attacks?")]
        public bool movementDisabledByAttacks;
        
        [Tooltip("Attack speed style")]
        public PrototypeAttackSpeedStyle attackSpeedStyle;
        
        [Tooltip("What can cancel attacks?")]
        public PrototypeCancellables cancellables;
    }

    [Header("Prototype Customisation")]
    [SerializeField] private PrototypeAttackCustomisation prototypeAttackCustomisation;

    
    

    private enum MoveState
    {
        Stopped,
        Accelerating,
        Running,
        Decelerating,
        ChangingDirection,
        Rolling
    }

    private enum RollState
    {
        StartRoll,
        Rolling,
        EndRoll,
        NotRolling
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!debugUseAnimations)
        {
            spriteRenderer.sprite = defaultSquareSprite;
        }
        
        _gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        _jumpVelocity = timeToJumpHeight * Mathf.Abs(_gravity);
        _moveState = MoveState.Stopped;
        _rollState = RollState.NotRolling;
    }

    // Update is called once per frame
    private void Update()
    {
        if (debugUseAnimations)
        {
            // Attack
            if (isAttacking)
            {
                // Does attacking disable movement?
                if (prototypeAttackCustomisation.movementDisabledByAttacks)
                {
                    // TODO Not sure what to do here.
                    if (_isGrounded)
                    {
                        _velocity.x = 0f;
                    }
                    CheckGrounded();
                    ApplyGravity();
                    moveController.Move(_velocity * Time.deltaTime);
                }
            }
            else
            {
                ReadAttackInput();
            }
        }
        
        // Movement
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        HandleRollInput();
        HandleMoveInput();
        HandleJumpInput();
        SetHorizontalMove();
        CheckGrounded();
        ApplyGravity();
        Jump();

        if (!isAttacking || (isAttacking && !prototypeAttackCustomisation.movementDisabledByAttacks))
        {
            //move works by taking in a displacement, firing raycasts in the directions of the displacement
            //then if the raycasts collide with anything the displacement is altered to be the distance from the player edge to the collider
            //then at the end of controller it uses transform.translate(displacement) with the edited displacement 
            moveController.Move(_velocity * Time.deltaTime);
        }

        // Animation
        if (debugUseAnimations)
        {
            animator.SetFloat(SpeedID, Mathf.Abs(_velocity.x));

            if (isAttacking)
            {
                // What cancels attacks?
                if ((prototypeAttackCustomisation.cancellables & PrototypeCancellables.Roll) != PrototypeCancellables.None)
                {
                    if (_isRollInput)
                    {
                        isAttacking = false;
                        animator.Play("Player_Idle");
                        // todo getting playercombat here is bad.
                        GetComponent<PlayerCombat>().ForceHideSwipes();
                    }
                }

                if ((prototypeAttackCustomisation.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
                {
                    if (_isJumpInput)
                    {
                        isAttacking = false;
                        animator.Play("Player_Jump");
                        GetComponent<PlayerCombat>().ForceHideSwipes();
                    }
                }
                
                if ((prototypeAttackCustomisation.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
                {
                    if (_isMoveInput)
                    {
                        isAttacking = false;
                        animator.Play("Player_Idle");
                        GetComponent<PlayerCombat>().ForceHideSwipes();
                    }
                }
            }
        }
        spriteRenderer.flipX = facingDirection == FacingDirection.Left;
    }

    #region Jump Movement

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (debugUseAnimations)
            {
                animator.SetTrigger(JumpTriggerID);
            }
            _isJumpInput = true;
            StartCoyoteTime();
        }
        else
        {
            _isJumpInput = false;
        }
    }

    private void Jump()
    {
        if (_isJumpInput && (_isGrounded || (_jumpCalledTime - _lastGroundedTime < coyoteTime)))
        {
            _velocity.y = _jumpVelocity;
            _jumpCalledTime = float.MaxValue;
        }
    }

    private void ApplyGravity()
    {
        //this makes sure that the gravity is always properly applied
        //if its in an else of this if it really fucks up the jumping

        if (moveController.Collisions.Below || moveController.Collisions.Above)
        {
            _velocity.y = 0;
        }
        _velocity.y += _gravity * Time.deltaTime;
    }

    private void CheckGrounded()
    {
        if(moveController.CheckGrounded())
        {
            _isGrounded = true;
            _lastGroundedTime = Time.time;
            _jumpCalledTime = 0f;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void StartCoyoteTime()
    {
        if (_isJumpInput && _jumpCalledTime != float.MaxValue )
        {
            _jumpCalledTime = Time.time;
        }
    }

    #endregion

    #region Horizontal Movement
    private void HandleMoveInput()
    {
        if (_moveInput.x != 0)
        {
            _isMoveInput = true;
            facingDirection = _moveInput.x < 0 ? FacingDirection.Left : FacingDirection.Right;
        }
        else
        {
            _isMoveInput = false;
        }
    }

    private void SetHorizontalMove()
    {
        // Rolling
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
        
        // Moving
        if (_isMoveInput)
        {
            if ((int)Mathf.Sign(_velocity.x) != (int)_moveInput.x && _velocity.x != 0)
            {
                StartDirectionChange();
            }
            switch (_moveState)
            {
                case MoveState.Stopped:
                    //begins the movement, calls sets to accelerating
                    //todo Joe here, is this correct? Start moving in the stopped state?
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

    private void StartMoving()
    {
        _lerpCurrent = 0f;
        _moveState = MoveState.Accelerating;
    }

    private void StopMoving()
    {
        _lerpCurrent = 0f;
        _moveState = MoveState.Decelerating;
    }

    private void StartDirectionChange()
    {
        //resets the lerp to 0 each time we change direction
        _lerpCurrent = 0f;
        _moveState = MoveState.ChangingDirection;
    }

    private void Accelerate()
    {
        //uses a lerp which is then used to evaluate along an animation curve for the acceleration
        //once we get to the max speed change to running
        //checks if there is a collision below the player, and if so use the air timers
        float rate = (moveController.Collisions.Below ? accelerationRate : airAccelerationRate);
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, moveSpeed * _moveInput.x, accelerationCurve.Evaluate(_lerpCurrent));
        
        // TODO Can input.x just be deleted here? They look like they cancel to me.
        if (Mathf.Abs(_velocity.x) * _moveInput.x >= _moveInput.x * moveSpeed)
        {
            _moveState = MoveState.Running;
        }
    }

    private void Run()
    {
        _velocity.x = _moveInput.x * moveSpeed;
    }

    private void Decelerate()
    {
        //same lerp method as accelerate
        //this time changes to stopped after getting low enough 
        //(I tried doing if(speed==0) but that was glitchy af
        float rate = moveController.Collisions.Below ? decelerationRate : airDecelerationRate;
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, 0f, decelerationCurve.Evaluate(_lerpCurrent));
        if (Mathf.Abs(_velocity.x) <= 0.5f)
        {
            _velocity.x = 0f;
            _moveState = MoveState.Stopped;
        }
    }

    private void ChangeDirection()
    {
        //same lerp method as accelerate
        float rate = moveController.Collisions.Below ? changeDirectionRate : airChangeDirectionRate;
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, moveSpeed * _moveInput.x, changeDirectionCurve.Evaluate(_lerpCurrent));
        
        // TODO You need to be careful with floating point comparisons, eg:
        // 0.000001 == 0.000000 is false, but we might want them to be equal.
        // I'm not sure what the goal of the this bit of code is so I'm leaving for now.
        // But should prob be of the form:
        // if(Math.Abs(Mathf.Abs(_velocity.x) - moveSpeed) < TOLERANCE)
        // Where TOLERANCE is some constant small value.
        // You might want to set velocity.x = moveSpeed if this is true?
        
        // TODO P.S I changed this from:
        // if (Mathf.Abs(velocity.x)*input.x == input.x * moveSpeed)
        // Because they cancel. Delete this comment if that's all good.
        if (Mathf.Abs(_velocity.x) == moveSpeed)
        {
            _moveState = MoveState.Running;
        }
    }

    private void HandleRollInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isRollInput = true;
        }
        else
        {
            _isRollInput = false;
        }
    }

    private void StartRoll()
    {
        //starts the roll timer and does the enums, could be state machine for animation purposes?
        //roll overrides other movement
        //can only do while grounded for now, but we could change this later on 
        if (_isGrounded)
        {
            _moveState = MoveState.Rolling;
            rollTimer = 0f;
            _rollState = RollState.Rolling;
            _rollDirection = (float)facingDirection;
        }
    }

    private void Roll()
    {
        //keeps rolling while the timer is on
        if (rollTimer <= rollDuration)
        {
            _velocity.x = rollDistance * (int)_rollDirection / rollDuration;
            rollTimer += Time.deltaTime;
        }
        else
        {
            _rollState = RollState.EndRoll;
        }
    }

    //TODO: Make roll stopping feel better
    private void StopRoll()
    {
        //stops the player dead, this doesn't feel great tho going to have another look once the enemy AI is done
        _moveState = MoveState.Stopped;
        _velocity.x = 0f;
        _rollState = RollState.NotRolling;
    }
    #endregion

    #region Attacks

    private void ReadAttackInput()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        
        // TODO Switch which animations we're going to use.        
        // switch (prototypeCustomisation.attackSpeedStyle)
        // {
        //     case PrototypeAttackSpeedStyle.Middling:
        //         // Switch the animation we're going to use
        //         break;
        //     case PrototypeAttackSpeedStyle.DarkSouls:
        //         break;
        //     case PrototypeAttackSpeedStyle.HollowKnight:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
        
        if (Input.GetKey(KeyCode.W))
        {
            if (debugUseAnimations)
            {
                animator.SetTrigger(AttackUpTriggerID);
                isAttacking = true;
            }
        }
        else if (!_isGrounded && Input.GetKey(KeyCode.S))
        {
            if (debugUseAnimations)
            {
                animator.SetTrigger(AttackDownTriggerID);
                isAttacking = true;
            }
        }
        else
        {
            if (debugUseAnimations)
            {
                animator.SetTrigger(AttackTriggerID);
                isAttacking = true;
            }
        }
    }

    #endregion
}

public enum FacingDirection
{
    Left = -1,
    Right = 1
}
