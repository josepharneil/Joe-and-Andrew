using System;
using Entity;
using JetBrains.Annotations;
using Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1
// more shameless thieving https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs

namespace Player
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(CustomCollider2D))]
    public class PlayerInputs : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;

        [Header("Jump Stats")]
        [SerializeField] private float jumpVelocity = 15f;
        [SerializeField] private float coyoteTime;
        [SerializeField] private float minFallSpeed = 80f;
        [SerializeField] private float maxFallSpeed = 120f;
        [SerializeField] private float jumpApexThreshold = 5f;
        [SerializeField] private float fallClamp = -40f;
        [SerializeField] private float earlyJumpMultiplier = 3f;
        [SerializeField] private float earlyJumpCancelTime = 0.1f;
        [SerializeField] private bool _debugRollFall = false;
        private float _apexPoint; //This becomes 1 at the end of the jump
        private float _fallSpeed;

        [Header("Wall Jump / Sliding Stats")]
        [SerializeField] private bool _debugDisableWallJumpSlide = false;
        [SerializeField] private float _verticalWallJump;
        [SerializeField] private float _horizontalWallJump;
        private bool _isWallSliding = false;
        [SerializeField] private float _wallSideGravityMultiplier;
        private float _wallJumpCounter=0f;
        
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
        [SerializeField] private float rollSpeed = 20f;
        [SerializeField] private float rollCoolDown;

        private float _jumpCalledTime;
        private float _lastGroundedTime;
        private float _rollDirection;
        private float _rollDurationTimer = 0f;
        private float _rollCoolDownTimer = 0f;
        private float _fallThroughPlatformTimer = 0f;

        private bool _isMoveInput;
        private bool _isJumpInput;
        private bool _isJumpEndedEarly = false;
        private bool _isInCoyoteTime;
        private bool _hasJumped;
        private bool _isGrounded;
        private bool _isRollInput;
        private bool _hasWallJumped;
        private bool _hasFallenThroughPlatform;
        public FacingDirection FacingDirection { get; private set; }
        private float _lerpCurrent = 0f;
        [SerializeField] private MoveState moveState;
        private RollState _rollState;
        [NonSerialized] public Vector2 Velocity;
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
        
        [Header("Knockback")]
        [SerializeField] private EntityKnockback entityKnockback;

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
            
            moveState = MoveState.Stopped;
            _rollState = RollState.NotRolling;
        }

        // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html
        // Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
        // You would usually use this to perform an action after a value changes in the Inspector; for example, making sure that data stays within a certain range.
        // NOTE @JA I've never heard of this, but looks perfect for updating values during game time :) 
        private void OnValidate()
        {
        }

        private void OnDrawGizmosSelected()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            // Attack movement
            if (debugUseAnimations && isAttacking && playerCombatPrototyping.data.movementDisabledByAttacks)
            {
                // TODO @JA Not sure what to do here.
                if (_isGrounded)
                {
                    Velocity.x = 0f;
                }
                CheckGrounded();
                CalculateGravity();
                movementController.Move(Velocity);
            }
            
            // Movement
            SetHorizontalMove();
            if (!_debugDisableWallJumpSlide)
            {
                CheckWallSlide();
            }
            CheckGrounded();
            CheckCoyote();
            CalculateJumpApex();
            CalculateGravity();
            DropThroughPlatform();
            if (!_debugDisableWallJumpSlide)
            {
                WallJump();
            }
            Jump();

            if (!isAttacking || (isAttacking && !playerCombatPrototyping.data.movementDisabledByAttacks))
            {
                //move works by taking in a displacement, firing raycasts in the directions of the displacement
                //then if the raycasts collide with anything the displacement is altered to be the distance from the player edge to the collider
                //then at the end of controller it uses transform.translate(displacement) with the edited displacement 
                movementController.Move(Velocity);
            }

            if (!isAttacking || (isAttacking && playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
            {
                if (_moveInput.x < 0)
                {
                    FacingDirection = FacingDirection.Left;
                    spriteRenderer.flipX = true;
                }
                else if( _moveInput.x > 0)
                {                    
                    FacingDirection = FacingDirection.Right;
                    spriteRenderer.flipX = false;
                }
            }

            // Animation
            if (debugUseAnimations)
            {
                animator.SetFloat(SpeedID, Mathf.Abs(Velocity.x));
            }

            CheckIfAttackIsCancellable();
        }

        #region Gravity and Fall calculations
        //Taken from Tarodevs GitHub: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
        private void CalculateGravity()
        {
            if (movementController.customCollider2D.GetCollisionBelow())
            {
                if (Velocity.y < 0)
                {
                    Velocity.y = 0;
                }
            }
            else
            {
                if (movementController.customCollider2D.CheckIfHittingCeiling())
                {
                    _isJumpEndedEarly = true;
                }
                
                // Checks if the player has ended a jump early, and if so increase the gravity
                if (_isJumpEndedEarly)
                {
                    Velocity.y -= _fallSpeed * earlyJumpMultiplier * Time.deltaTime;
                }
                else if (_isWallSliding && Velocity.y < 0) 
                {
                    Velocity.y -= _fallSpeed * _wallSideGravityMultiplier * Time.deltaTime;
                }
                else
                {
                    Velocity.y -= _fallSpeed * Time.deltaTime;
                }
                
                // Makes the player actually fall
                // Clamps the y velocity to a certain value
                if (Velocity.y < fallClamp) Velocity.y = fallClamp;
            }
        }

        #endregion
        
        #region Jump Movement

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadJumpInput(InputAction.CallbackContext context)
        {
            if (entityDaze && entityDaze.isDazed) return;
            if (context.started)
            {
                _isJumpInput = true;
                _isJumpEndedEarly = false;
                StartCoyoteTime();
                if (debugUseAnimations)
                {
                    animator.SetTrigger(JumpTriggerID);
                }
            }
            if (context.canceled && (Time.time -_jumpCalledTime<earlyJumpCancelTime))
            {
                _isJumpEndedEarly = true;
            }
        }

        private void ActionCancelled(InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
        }

        private void Jump()
        {
            if (_isJumpInput && (_isGrounded || _isInCoyoteTime))
            {
                if(!_isGrounded && _isInCoyoteTime) Debug.Log("Coyote jump");
                Velocity.y = jumpVelocity;
                _isJumpInput = false;
                _isInCoyoteTime = false;
                _hasJumped = true;
            }
        }

        private void CalculateJumpApex()
        {
            if (!movementController.customCollider2D.GetCollisionBelow())
            {
                //sets the apexPoint based on how large the y velocity is
                _apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                //uses the apexPoint to lerp between the min and max fallspeeds (our new gravity replacement)
                _fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0f;
            }
        }

        private void CheckGrounded()
        {
            if(movementController.customCollider2D.CheckIfGrounded())
            {
                _isGrounded = true;
                _lastGroundedTime = Time.time;
                _hasJumped = false;

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
            if (_isJumpInput)
            {
                _jumpCalledTime = Time.time;
            }
        }

        private void CheckCoyote()
        {
            if (_isJumpInput && !_isGrounded && !_hasJumped && (_jumpCalledTime - _lastGroundedTime) < coyoteTime )
            {
                _isInCoyoteTime = true;
            }
            else
            {
                _isInCoyoteTime = false;
            }
        }

        private void WallJump()
        {
            //checks if grounded and jumpinput
            //then checks if there are any collisions to the left and right
            //if so, applies the wall jump vectors in the correct direction
            //currently this also disables movement inputs for a short time with a counter
            //not sure if there is a better way of doing this
            if (!_isGrounded && _isJumpInput && !_isInCoyoteTime)
            {
                CustomCollider2D customCollider2D = movementController.customCollider2D;
                if (customCollider2D.GetCollisionLeft() || customCollider2D.GetCollisionRight())
                {
                    _isJumpInput = false;
                    Velocity.y = _verticalWallJump;
                    Velocity.x = _horizontalWallJump * -1f * (int)FacingDirection;
                    _hasWallJumped = true;
                    FacingDirection = (FacingDirection == FacingDirection.Left) ? FacingDirection.Right : FacingDirection.Left;
                }
                else
                {
                    _isJumpInput = false;
                }
            }
            if (_hasWallJumped)
            {
                _wallJumpCounter++;
                Debug.Log(_wallJumpCounter);
            }
            //AK:9/2/21 this number (150) is too high atm, uaing it for testing  
            if (_wallJumpCounter >= 15)
            {
                _hasWallJumped = false;
                _wallJumpCounter = 0;
            }
        }

        private void DropThroughPlatform()
        {
            CustomCollider2D customCollider2D = movementController.customCollider2D;
            if (_hasFallenThroughPlatform &&_fallThroughPlatformTimer<20)
            {
                _fallThroughPlatformTimer += 1;
            }
            else
            {
                _hasFallenThroughPlatform = false;
                _fallThroughPlatformTimer = 0;
                customCollider2D.SetFallThroughPlatform(false);
            }

            // TODO This should be called within some kind of read input function if we're reading the axis... I think
            if (_isJumpInput && customCollider2D.CheckIfOneWayPlatform() && Input.GetAxisRaw("Vertical") < 0f)
            {
                _isJumpInput = false;
                customCollider2D.SetFallThroughPlatform(true);
                _hasFallenThroughPlatform = true;
            }
        }

        #endregion

        #region WallSlide
        private void CheckWallSlide()
        {
            bool collisionLeftRight = FacingDirection == FacingDirection.Left ?
                movementController.customCollider2D.GetCollisionLeft() : movementController.customCollider2D.GetCollisionRight();
            if (_isMoveInput && !_isGrounded &&collisionLeftRight)
            {
                 _isWallSliding = true;
            }
            else
            {
                _isWallSliding = false;
            }
        }
        #endregion

        #region Horizontal Movement
        /// <summary>
        /// called by Unity PlayerInput event
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadMoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            if (entityDaze && entityDaze.isDazed)
            {
                _moveInput = Vector2.zero;
            }
            if (_hasWallJumped)
            {
                //used to stop the player moving for a short period after they have wall jumped
                //once it works it should let wall jumps be chained together
                _moveInput = Vector2.zero;
            }
            if (_moveInput.x != 0)
            {
                _isMoveInput = true;
                if (entityKnockback.IsBeingKnockedBack() ||
                    (isAttacking && !playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
                {
                    return;
                }
                if (_moveInput.x < 0)
                {
                    FacingDirection = FacingDirection.Left;
                    spriteRenderer.flipX = true;
                }
                else if (_moveInput.x > 0)
                {                    
                    FacingDirection = FacingDirection.Right;
                    spriteRenderer.flipX = false;
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
                if ((int)Mathf.Sign(Velocity.x) != (int)_moveInput.x && Velocity.x != 0)
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
            Velocity.x = Mathf.Lerp(Velocity.x, moveSpeed * _moveInput.x, accelerationCurve.Evaluate(_lerpCurrent));
            
            // TODO Can input.x just be deleted here? They look like they cancel to me
            // AK: yes its gone now!
            if (moveSpeed - Mathf.Abs(Velocity.x) <=  accelerationTolerance)
            {
                moveState = MoveState.Running;
            }
        }

        private void Run()
        {
            float blockMoveSpeedModifier = 1.0f;
            if (entityBlock && entityBlock.IsBlocking())
            {
                blockMoveSpeedModifier = entityBlock.blockSpeedModifier;
            }
            Velocity.x = _moveInput.x * moveSpeed * blockMoveSpeedModifier;
        }

        private void Decelerate()
        {
            //same lerp method as accelerate
            //this time changes to stopped after getting low enough 
            //(I tried doing if(speed==0) but that was glitchy af
            float rate = movementController.customCollider2D.GetCollisionBelow() ? decelerationRate : airDecelerationRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            Velocity.x = Mathf.Lerp(Velocity.x, 0f, decelerationCurve.Evaluate(_lerpCurrent));
            if (Mathf.Abs(Velocity.x) <= decelerationTolerance)
            {
                Velocity.x = 0f;
                moveState = MoveState.Stopped;
            }
        }

        private void ChangeDirection()
        {
            //same lerp method as accelerate
            float rate = movementController.customCollider2D.GetCollisionBelow() ? changeDirectionRate : airChangeDirectionRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            Velocity.x = Mathf.Lerp(Velocity.x, moveSpeed * _moveInput.x, changeDirectionCurve.Evaluate(_lerpCurrent));

            if ((Mathf.Abs(Velocity.x) - moveSpeed) < changeDirectionTolerance) 
            {
                moveState = MoveState.Running;
            }
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadRollInput(InputAction.CallbackContext context)
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
                _rollDirection = (float)FacingDirection;
            }
        }

        private void Roll()
        {
            // Keeps rolling while the timer is on
            float rollDuration = rollDistance / rollSpeed;
            if (_rollDurationTimer <= rollDuration)
            {
                Velocity.x = (int)_rollDirection * rollSpeed;
                if (_debugRollFall)
                {
                    Velocity.y = 0;
                }
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
        [UsedImplicitly] public void ReadAttackInput(InputAction.CallbackContext context)
        {
            if (!debugUseAnimations || (!entityBlock || entityBlock.IsBlocking()) && entityBlock)
            {
                return;
            }

            if (!context.started)
            {
                return;
            }
            
            // Disabling this because we don't have up / down right now.
#if false
            isAttacking = true;
            const float upwardsInputThreshold = 0.5f;
            const float downwardsInputThreshold = -upwardsInputThreshold;
            if (_moveInput.y > upwardsInputThreshold)
            {
                animator.SetTrigger(AttackUpTriggerID);
            }
            else if (!_isGrounded && _moveInput.y < downwardsInputThreshold)
            {
                animator.SetTrigger(AttackDownTriggerID);
            }
            else
            {
                if (playerCombatPrototyping.data.canChangeDirectionsDuringAttack)
                {
                    if (_moveInput.x < 0)
                    {
                        FacingDirection = FacingDirection.Left;
                        spriteRenderer.flipX = true;
                    }
                    else if ( _moveInput.x > 0 )
                    {
                        FacingDirection = FacingDirection.Right;
                        spriteRenderer.flipX = false;
                    }
                }
                animator.SetTrigger(AttackTriggerID);
            }
#else
            if (playerCombatPrototyping.data.canChangeDirectionsDuringAttack)
            {
                if (_moveInput.x < 0)
                {
                    FacingDirection = FacingDirection.Left;
                    spriteRenderer.flipX = true;
                }
                else if ( _moveInput.x > 0 )
                {
                    FacingDirection = FacingDirection.Right;
                    spriteRenderer.flipX = false;
                }
            }
            animator.SetTrigger(AttackTriggerID);
#endif

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
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }

            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
            {
                if (_isJumpInput)
                {
                    isAttacking = false;
                    animator.Play("Player_Jump");
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }
                
            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
            {
                if (_isMoveInput)
                {
                    isAttacking = false;
                    animator.Play("Player_Idle");
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }
        }
        
        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadParryInput(InputAction.CallbackContext context)
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
        [UsedImplicitly] public void ReadBlockInput(InputAction.CallbackContext context)
        {
            if (!entityBlock)
            {
                return;
            }

            if (context.performed && !isAttacking)
            {
                // For now, don't need to worry about whether you're mid parry /attack
                entityBlock.SetBlocking(true);
            }

            if (context.canceled)
            {
                entityBlock.SetBlocking(false);
            }
        }

        #endregion
    }
}

