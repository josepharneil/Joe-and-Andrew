using Entity;
using Physics;
using UnityEngine;
using UnityEngine.InputSystem;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CustomCollider2D))]
public class PlayerInputs : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MovementController movementController;

    [Header("Jump Stats")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToJumpHeight = 0.4f;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float minFallSpeed = 80f;
    [SerializeField] private float maxFallSpeed = 120f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float fallClamp = -40f;
    private float _apexPoint; //This becomes 1 at the end of the jump
    private float _fallSpeed;
    private float _jumpVelocity; //todo this is only ever assigned?? never used
    private float _gravity;
    //note: gravity is about to be replaced by fall speed

    [Header("Ground Move Stats")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] [Range(0f, 1f)] private float accelerationRate;
    [SerializeField] private float accelerationTolerance;
    [SerializeField] private AnimationCurve decelerationCurve;
    [SerializeField] [Range(0f, 1f)] private float decelerationRate;
    [SerializeField] private float decelerationTolerance;
    [SerializeField] private AnimationCurve changeDirectionCurve;
    [SerializeField] [Range(0f, 1f)] private float changeDirectionRate;
    [SerializeField] private float changeDirectionTolerance;

    [Header("Air Move Stats")]
    [SerializeField] [Range(0f, 1f)] private float airAccelerationRate;
    [SerializeField] [Range(0f, 1f)] private float airDecelerationRate;
    [SerializeField] [Range(0f, 1f)] private float airChangeDirectionRate;

    [Header("Roll Stats")]
    [SerializeField] private float rollDistance;
    [SerializeField] private float rollDuration;
    [SerializeField] private float rollCoolDown;

    private float _jumpCalledTime;
    private float _lastGroundedTime;
    //gravity and jumpVelocity are calculated based on the jump height and time
    private float _rollDirection;
    private float _rollDurationTimer = 0f;
    private float _rollCoolDownTimer = 0f;

    private bool _isMoveInput;
    private bool _isJumpInput;
    private bool _isGrounded;
    private bool _isRollInput;
    [HideInInspector] public FacingDirection facingDirection;
    private float _lerpCurrent = 0f;
    [SerializeField] private MoveState moveState;
    private RollState _rollState;
    private Vector3 _velocity;
    private Vector2 _moveInput;
    
    // Attacks
    [Header("Attacking")] 
    [SerializeField] private bool debugUseAnimations = true;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSquareSprite;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isInPreDamageAttackPhase = true;

    [Header("Parrying")]
    [SerializeField] private EntityParry entityParry;

    [Header("Blocking")] 
    [SerializeField] private EntityBlock entityBlock;
    
    // Animation parameter IDs.
    private static readonly int SpeedID = Animator.StringToHash("speed");
    private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
    private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
    private static readonly int AttackUpTriggerID = Animator.StringToHash("attackUpTrigger");
    private static readonly int AttackDownTriggerID = Animator.StringToHash("attackDownTrigger");
    
    [Header("Prototyping")]
    public PlayerCombatPrototyping playerCombatPrototyping;
    [SerializeField] private EntityDaze entityDaze;

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
        
        //AK 17/1/22 These are going to be altered for a more flexible gravity calculation, don't use for now
        _gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        _jumpVelocity = timeToJumpHeight * Mathf.Abs(_gravity);
        moveState = MoveState.Stopped;
        _rollState = RollState.NotRolling;
    }

    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html
    // Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
    // You would usually use this to perform an action after a value changes in the Inspector; for example, making sure that data stays within a certain range.
    // NOTE @JA I've never heard of this, but looks perfect for updating values during game time :) 
    private void OnValidate()
    {
        _gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        _jumpVelocity = timeToJumpHeight * Mathf.Abs(_gravity);
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
                if (playerCombatPrototyping.data.movementDisabledByAttacks)
                {
                    // TODO Not sure what to do here.
                    if (_isGrounded)
                    {
                        _velocity.x = 0f;
                    }
                    CheckGrounded();
                    ApplyGravity();
                    movementController.MoveAtSpeed(_velocity);
                }
            }
        }
        
        // Movement
        // ReadMoveInput();
        SetHorizontalMove();
        CheckGrounded();
        CalculateJumpApex();
        CalculateGravity();
    //    ApplyGravity();
        Jump();

        if (!isAttacking || (isAttacking && !playerCombatPrototyping.data.movementDisabledByAttacks))
        {
            //move works by taking in a displacement, firing raycasts in the directions of the displacement
            //then if the raycasts collide with anything the displacement is altered to be the distance from the player edge to the collider
            //then at the end of controller it uses transform.translate(displacement) with the edited displacement 
            movementController.MoveAtSpeed(_velocity);
            if (_moveInput.x < 0)
            {
                facingDirection = FacingDirection.Left;
                spriteRenderer.flipX = true;
            }
            else if( _moveInput.x > 0)
            {                    
                facingDirection = FacingDirection.Right;
                spriteRenderer.flipX = false;
            }
        }

        // Animation
        if (debugUseAnimations)
        {
            animator.SetFloat(SpeedID, Mathf.Abs(_velocity.x));
        }

        CheckIfAttackIsCancellable();
    }

    #region Gravity and Fall calculations
    //Taken from Tarodevs GitHub: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
    private void CalculateGravity()
    {
        if (movementController.customCollider2D.GetCollisionBelow())
        {
            if (_velocity.y < 0) _velocity.y = 0;
        }
        else
        {
            //TODO add in the early jump letgo Code

            //Makes the player actually fall
            _velocity.y -= _fallSpeed * Time.deltaTime;

            //Clamps the y velocity to a certain value
            if (_velocity.y < fallClamp) _velocity.y = fallClamp;
        }
    }

    #endregion
    
    #region Jump Movement

    /// <summary>
    /// Called by PlayerInput Unity Event.
    /// </summary>
    public void ReadJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && !(entityDaze && entityDaze.isDazed))
        {
            if (debugUseAnimations)
            {
                animator.SetTrigger(JumpTriggerID);
            }
            _isJumpInput = true;
            StartCoyoteTime();
        }
    }

    private void Jump()
    {
        if (_isJumpInput && (_isGrounded || (_jumpCalledTime - _lastGroundedTime < coyoteTime)))
        {
            _velocity.y = jumpHeight;
            _jumpCalledTime = float.MaxValue;
            _isJumpInput = false;
        }
    }

    private void CalculateJumpApex()
    {
        if (!movementController.customCollider2D.GetCollisionBelow())
        {
            //sets the apexPoint based on how large the y velocity is
            _apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(_velocity.y));
            //uses the apexPoint to lerp between the min and max fallspeeds (our new gravity replacement)
            _fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0f;
        }
    }


    private void ApplyGravity()
    {
        //this makes sure that the gravity is always properly applied
        //if its in an else of this if it really fucks up the jumping
        //AK 17/1/22: Gravity has now been replaced with fall speed

        if (movementController.customCollider2D.GetCollisionBelow() || movementController.customCollider2D.GetCollisionAbove() || _rollState != RollState.NotRolling)
        {
            _velocity.y = 0;
        }
        _velocity.y -= _fallSpeed * Time.deltaTime;
    }

    private void CheckGrounded()
    {
        if(movementController.customCollider2D.CheckIfGrounded())
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
        // Note for the future, because this is setting an exact value... it should be fine?? Hopefully...
        // This is a little bit of a "magic number" fix though.
        if (_isJumpInput && _jumpCalledTime != float.MaxValue )
        {
            _jumpCalledTime = Time.time;
        }
    }

    #endregion

    #region Horizontal Movement
    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        if (entityDaze && entityDaze.isDazed)
        {
            _moveInput = Vector2.zero;
        }
        if (_moveInput.x != 0)
        {
            _isMoveInput = true;
            if (!isAttacking || isAttacking && playerCombatPrototyping.data.canChangeDirectionsDuringAttack)
            {
                if (_moveInput.x < 0)
                {
                    facingDirection = FacingDirection.Left;
                    spriteRenderer.flipX = true;
                }
                else
                {                    
                    facingDirection = FacingDirection.Right;
                    spriteRenderer.flipX = false;
                }
            }
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
            _isRollInput = false;
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
            switch (moveState)
            {
                case MoveState.Stopped:
                    //begins the movement, calls sets to accelerating
                    //todo Joe here, is this correct? Start moving in the stopped state?
                    //AK: yeah this is correct, this is for if the player has stopped and an input comes in to start moving
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
            if (moveState != MoveState.Stopped)
            {
                StopMoving();
                Decelerate();
            }
        }
    }

    private void StartMoving()
    {
        _lerpCurrent = 0f;
        moveState = MoveState.Accelerating;
    }

    private void StopMoving()
    {
        _lerpCurrent = 0f;
        moveState = MoveState.Decelerating;
    }

    private void StartDirectionChange()
    {
        //resets the lerp to 0 each time we change direction
        _lerpCurrent = 0f;
        moveState = MoveState.ChangingDirection;
    }

    private void Accelerate()
    {
        //uses a lerp which is then used to evaluate along an animation curve for the acceleration
        //once we get to the max speed change to running
        //checks if there is a collision below the player, and if so use the air timers
        float rate = (movementController.customCollider2D.GetCollisionBelow() ? accelerationRate : airAccelerationRate);
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, moveSpeed * _moveInput.x, accelerationCurve.Evaluate(_lerpCurrent));
        
        // TODO Can input.x just be deleted here? They look like they cancel to me
        // AK: yes its gone now!
        if (moveSpeed - Mathf.Abs(_velocity.x) <=  accelerationTolerance)
        {
            moveState = MoveState.Running;
        }
    }

    private void Run()
    {
        float blockMoveSpeedModifier = 1.0f;
        if (entityBlock && entityBlock.isBlocking)
        {
            blockMoveSpeedModifier = entityBlock.blockSpeedModifier;
        }
        _velocity.x = _moveInput.x * moveSpeed * blockMoveSpeedModifier;
    }

    private void Decelerate()
    {
        //same lerp method as accelerate
        //this time changes to stopped after getting low enough 
        //(I tried doing if(speed==0) but that was glitchy af
        float rate = movementController.customCollider2D.GetCollisionBelow() ? decelerationRate : airDecelerationRate;
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, 0f, decelerationCurve.Evaluate(_lerpCurrent));
        if (Mathf.Abs(_velocity.x) <= decelerationTolerance)
        {
            _velocity.x = 0f;
            moveState = MoveState.Stopped;
        }
    }

    private void ChangeDirection()
    {
        //same lerp method as accelerate
        float rate = movementController.customCollider2D.GetCollisionBelow() ? changeDirectionRate : airChangeDirectionRate;
        _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
        _velocity.x = Mathf.Lerp(_velocity.x, moveSpeed * _moveInput.x, changeDirectionCurve.Evaluate(_lerpCurrent));

        if ((Mathf.Abs(_velocity.x) - moveSpeed) < changeDirectionTolerance) 
        {
            moveState = MoveState.Running;
        }
    }

    /// <summary>
    /// Called by PlayerInput Unity Event.
    /// </summary>
    public void ReadRollInput(InputAction.CallbackContext context)
    {
        // TODO maybe should just directly call here.
        if (context.started && !(entityDaze && entityDaze.isDazed))
        {
            _isRollInput = true;
        }
    }

    private void StartRoll()
    {
        //starts the roll timer and does the enums, could be state machine for animation purposes?
        //roll overrides other movement
        if (_isRollInput && (Time.time - _rollCoolDownTimer > rollCoolDown)&&_rollState!=RollState.Rolling) 
        {
            moveState = MoveState.Rolling;
            _rollDurationTimer = 0f;
            _rollState = RollState.Rolling;
            _rollDirection = (float)facingDirection;
        }
    }

    private void Roll()
    {
        //keeps rolling while the timer is on
        if (_rollDurationTimer <= rollDuration)
        {
            _velocity.x = (int)_rollDirection * (rollDistance / rollDuration);
            _rollDurationTimer += Time.deltaTime;
        }
        else
        {
            _rollState = RollState.EndRoll;
        }
    }

    private void StopRoll()
    {
        moveState = MoveState.Decelerating;
        _rollState = RollState.NotRolling;
        _rollCoolDownTimer = Time.time;
    }
    #endregion

    #region Combat
    
    /// <summary>
    /// Called by PlayerInput Unity Event.
    /// </summary>
    /// <param name="context"></param>
    public void ReadAttackInput(InputAction.CallbackContext context)
    {
        if (!debugUseAnimations || (!entityBlock || entityBlock.isBlocking) && entityBlock)
        {
            return;
        }

        if (!context.started)
        {
            return;
        }

        const float upwardsInputThreshold = 0.5f;
        const float downwardsInputThreshold = -upwardsInputThreshold;
        if (_moveInput.y > upwardsInputThreshold)
        {
            animator.SetTrigger(AttackUpTriggerID);
            isAttacking = true;
        }
        else if (!_isGrounded && _moveInput.y < downwardsInputThreshold)
        {
            animator.SetTrigger(AttackDownTriggerID);
            isAttacking = true;
        }
        else
        {
            if (_moveInput.x < 0)
            {
                facingDirection = FacingDirection.Left;
                spriteRenderer.flipX = true;
            }
            else if ( _moveInput.x > 0 )
            {
                facingDirection = FacingDirection.Right;
                spriteRenderer.flipX = false;
            }
            animator.SetTrigger(AttackTriggerID);
            isAttacking = true;
        }
    }

    private void CheckIfAttackIsCancellable()
    {
        // Cancellable attack phases
        if (!isAttacking) return;

        // TODO There are only really two phases right now
        // the actual attack phase is only 1 frame right now.
        if (isInPreDamageAttackPhase)
        {
            // What phases are cancellable?
            if ((playerCombatPrototyping.data.cancellableAttackPhases &
                 PrototypeAttackPhases.PreDamage) == PrototypeAttackPhases.None)
            {
                return;
            }
        }
        else // Post damage
        {
            if ((playerCombatPrototyping.data.cancellableAttackPhases &
                 PrototypeAttackPhases.PostDamage) == PrototypeAttackPhases.None)
            {
                return;
            }
        }
        
        // What cancels attacks?
        if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Roll) != PrototypeCancellables.None)
        {
            if (_isRollInput)
            {
                isAttacking = false;
                animator.Play("Player_Idle");
                // todo getting playercombat here is bad.
                GetComponent<PlayerCombat>().ForceHideSwipes();
            }
        }

        if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
        {
            if (_isJumpInput)
            {
                isAttacking = false;
                animator.Play("Player_Jump");
                GetComponent<PlayerCombat>().ForceHideSwipes();
            }
        }
            
        if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
        {
            if (_isMoveInput)
            {
                isAttacking = false;
                animator.Play("Player_Idle");
                GetComponent<PlayerCombat>().ForceHideSwipes();
            }
        }
    }
    
    /// <summary>
    /// Called by PlayerInput Unity Event.
    /// </summary>
    /// <param name="context"></param>
    public void ReadParryInput(InputAction.CallbackContext context)
    {
        if (!entityParry || isAttacking)
        {
            return;
        }

        // For now, parrying is instant and you can still move during it.
        if (!context.started)
        {
            return;
        }
        
        entityParry.CheckParry();
    }

    /// <summary>
    /// Called by PlayerInput Unity Event.
    /// </summary>
    /// <param name="context"></param>
    public void ReadBlockInput(InputAction.CallbackContext context)
    {
        if (!entityBlock)
        {
            return;
        }

        if (context.performed && !isAttacking)
        {
            // For now, don't need to worry about whether you're mid parry /attack
            entityBlock.isBlocking = true;
        }

        if (context.canceled)
        {
            entityBlock.isBlocking = false;
        }
    }

    #endregion
}

