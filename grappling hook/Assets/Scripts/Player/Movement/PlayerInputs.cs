using System;
using Audio;
using Entity;
using JetBrains.Annotations;
using Physics;
using UnityEngine;
using UnityEngine.InputSystem;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1
// more shameless thieving https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs

namespace Player
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(BoxRayCollider2D))]
    public class PlayerInputs : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;

        [Header("Fall Stats")]
        [SerializeField] private float _fallClamp = -35f;

        [Header("Wall Jump / Sliding Stats")]
        [SerializeField] private bool _debugDisableWallJumpSlide = false;
        [SerializeField] private float _verticalWallJump = 13f;
        [SerializeField] private float _horizontalWallJump = 13f;
        [SerializeField] private float _wallJumpInputDisableTime = 0.2f;
        [SerializeField] private float _wallJumpCoyoteDuration = 0.12f;
        private float _lastWalledTime;
        [SerializeField] private int _maxNumberOfWallJumpsBeforeGrounding = 2;
        [SerializeField] private float _wallJumpSkinWidth = 0.25f;
        private int _currentNumberOfWallJumps = 0;
        private bool _isWallSliding = false;
        [SerializeField] private float _wallSideGravityMultiplier = 0.3f;
        
        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private PlayerHorizontalMovement _playerHorizontalMovement;
        [SerializeField] private PlayerJump _playerJump;

        private float _jumpInputTime;
        private float _lastGroundedTime;
        public void TurnOffJumpInput() => _isJumpInput = false;
        public void TurnOffBufferedJumpInput() => _isBufferedJumpInput = false;
        
        private float _fallThroughPlatformTimer = 0f;

        private bool _isMoveInput;
        private bool _isJumpInput;
        
        private bool _isJumpEndedEarly = false;
        // private bool _isInCoyoteTime; // do we need this? TODO check
        private bool _isBufferedJumpInput;

        // private bool _hasJumped;
        private bool _isGrounded;
        private bool _hasWallJumped;
        private bool _hasFallenThroughPlatform;
        public FacingDirection FacingDirection { get; private set; }
        public AttackDirection AttackDirection { get; private set; } 
        [NonSerialized] public Vector2 Velocity;
        private Vector2 _moveInput;
        
        // Attacks
        [Header("Attacking")] 
        [SerializeField] private bool _debugUseAnimations = true;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite defaultSquareSprite;
        [HideInInspector] public bool isAttacking;
        [HideInInspector] public bool isInPreDamageAttackPhase = true;
        public PlayerEquipment CurrentPlayerEquipment;
        [SerializeField] private bool _attacksDrivenByAnimations = true;
        [SerializeField] private PlayerAttackDriver _playerAttackDriver;
        [SerializeField] private float _downAttackJumpVelocity = 15f;
        
        public bool GetDebugUseAnimations() => _debugUseAnimations;
        public bool GetDebugUseSounds() => _debugUseSounds;
        public Animator GetAnimator() => animator;
        public PlayerSounds GetPlayerSounds() => _playerSounds;
        

        [Header("Parrying")]
        [SerializeField] private EntityParry entityParry;

        [Header("Blocking")] 
        [SerializeField] private EntityBlock entityBlock;
        
        [Header("Knockback")]
        [SerializeField] private EntityKnockback entityKnockback;

        [Header("Daze")]
        [SerializeField] private EntityDaze entityDaze;
        
        // Animation parameter IDs.
        private static readonly int HorizontalSpeedID = Animator.StringToHash("horizontalSpeed");
        private static readonly int VerticalSpeedID = Animator.StringToHash("verticalSpeed");
        private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
        private static readonly int AttackUpTriggerID = Animator.StringToHash("attackUpTrigger");
        private static readonly int AttackDownTriggerID = Animator.StringToHash("attackDownTrigger");
        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
        private static readonly int GroundedTriggerID = Animator.StringToHash("groundedTrigger");

        [Header("Prototyping")]
        public PlayerCombatPrototyping playerCombatPrototyping;

        [Header("Player Sounds")] 
        [SerializeField] private PlayerSounds _playerSounds;
        [SerializeField] private bool _debugUseSounds = true;

        public void SetMoveState(MoveState moveState)
        {
            _playerHorizontalMovement.MoveState = moveState;
        }

        public Vector2 GetMoveInput()
        {
            return _moveInput;
        }

        public bool GetIsMoveInput()
        {
            return _isMoveInput;
        }

        private void Awake()
        {
            _playerDash.Initialise(this);
            _playerHorizontalMovement.Initialise(this, movementController, entityBlock);
            _playerJump.Initialise(this, movementController);
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (!_debugUseAnimations)
            {
                spriteRenderer.sprite = defaultSquareSprite;
            }

            _playerDash.Start();
            _playerHorizontalMovement.Start();
        }
        
        private void OnGUI()
        {
            _playerAttackDriver.ShowDebugGUI();
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateMovement();
            CheckWallSlide();
            CheckGrounded();
            CheckBufferedJumpInput();
            CalculateGravity();
            DropThroughPlatform();
            WallJump();
            UpdateJump();
            Move();
            UpdateFacingDirection();
            SetAnimatorSpeedFloats();
            UpdateAttackDriver();
            CheckIfAttackIsCancellable();
        }

        private void UpdateAttackDriver()
        {
            if (!_attacksDrivenByAnimations)
            {
                _playerAttackDriver.UpdateAttack();
            }
        }

        private void SetAnimatorSpeedFloats()
        {
            // Animation
            if (!_debugUseAnimations) return;
            
            animator.SetFloat(HorizontalSpeedID, Mathf.Abs(Velocity.x));
            animator.SetFloat(VerticalSpeedID, Velocity.y);
        }

        private void Move()
        {
            if (!playerCombatPrototyping.data.movementDisabledByAttacks)
            {
                movementController.Move(Velocity);
            }
            else
            {                
                if (isAttacking)
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
                else
                {
                    movementController.Move(Velocity);
                }
            }
        }

        private void UpdateFacingDirection()
        {
            if (!isAttacking || (isAttacking && playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
            {
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
        }

        //AK 23/4 added for use when changing the the speeds from flow
        public void ResetMoveSpeed()
        {
            _playerHorizontalMovement.ResetMoveSpeed();
        }

        public void MultiplyMoveSpeed(float multiple)
        {
            _playerHorizontalMovement.MultiplyMoveSpeed(multiple);
        }

        #region Gravity and Fall calculations
        //Taken from Tarodevs GitHub: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
        private void CalculateGravity()
        {
            if (movementController.customCollider2D.CollisionBelow)
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
                    Velocity.y -= _playerJump.GetFallSpeed() * _playerJump.GetEarlyJumpMultiplier() * Time.deltaTime;
                }
                else if (_isWallSliding && Velocity.y < 0) 
                {
                    Velocity.y -= _playerJump.GetFallSpeed() * _wallSideGravityMultiplier * Time.deltaTime;
                }
                else
                {
                    Velocity.y -= _playerJump.GetFallSpeed() * Time.deltaTime;
                }
                
                // Makes the player actually fall
                // Clamps the y velocity to a certain value
                if (Velocity.y < _fallClamp) Velocity.y = _fallClamp;
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
                // Note for the future, because this is setting an exact value... it should be fine?? Hopefully...
                // This is a little bit of a "magic number" fix though.
                _jumpInputTime = Time.time;
            }

            if (context.canceled && (Time.time - _jumpInputTime < _playerJump.GetEarlyCancelTime()))
            {
                _isJumpEndedEarly = true;
            }
        }

        private void UpdateJump()
        {
            float timeBetweenJumpInputAndLastGrounded = _jumpInputTime - _lastGroundedTime;
            _playerJump.Update(_isJumpInput, _isGrounded, _isBufferedJumpInput, timeBetweenJumpInputAndLastGrounded);
        }
        
        public void DownAttackJump()
        {
            Velocity.y = _downAttackJumpVelocity;
        }

        private void CheckGrounded()
        {
            if(movementController.customCollider2D.CheckIfGrounded())
            {
                animator.SetBool(GroundedTriggerID, true);
                _isGrounded = true;
                _lastGroundedTime = Time.time;
                _playerJump.SetHasJumped(false);
                _currentNumberOfWallJumps = 0;
                _playerJump.ResetCurrentNumAerialJumps();
                
                // Cancel wall jump blocking move inputs
                if (_hasWallJumped)
                {
                    // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
                    // you wall jump, and maintain the exact same input, thus no input read up
                    _moveInput.x = Input.GetAxisRaw("Horizontal");
                    _moveInput.y = Input.GetAxisRaw("Vertical");
                    _isMoveInput = true;
                    _hasWallJumped = false;
                }
            }
            else
            {
                _isGrounded = false;
            }
        }

        private void CheckBufferedJumpInput()
        {
            // "When the character is already in the air pressing jump moments before the ground will trigger jump as soon as they land"
            // http://www.davetech.co.uk/gamedevplatformer
            if (_isJumpInput && !_isGrounded)
            {
                _isBufferedJumpInput = true;
            }
            
            // Remove buffered jump if it has been too long.
            if (_isBufferedJumpInput && (Time.time - _jumpInputTime) > _playerJump.GetJumpBufferTime())
            {
                _isBufferedJumpInput = false;
            }
        }
        
        private void WallJump()
        {
            if (_debugDisableWallJumpSlide) return;
            
            if (!_isGrounded && !_playerJump.GetIsInCoyoteTime() &&
                (_currentNumberOfWallJumps < _maxNumberOfWallJumpsBeforeGrounding))
            {
                // Check for wall to right / left OR check for wall jump coyote
                BoxRayCollider2D customCollider2D = movementController.customCollider2D;
                customCollider2D.CheckHorizontalCollisions(out bool wallIsToLeft, out bool wallIsToRight, _wallJumpSkinWidth);

                bool isAgainstWall = wallIsToLeft || wallIsToRight;
                if(wallIsToLeft && wallIsToRight)
                {
                    Debug.LogError("WALL JUMPING: Wall to the left AND to the right: This implies bad level design? Not sure what to do here.", this);
                }
                if (isAgainstWall)
                {
                    _lastWalledTime = Time.time;
                }
                bool isInWallJumpCoyote = (Time.time - _lastWalledTime) < _wallJumpCoyoteDuration;
                
                bool jumpFromWall = _isJumpInput && (isAgainstWall || isInWallJumpCoyote);
                if (jumpFromWall)
                {
                    Velocity.y = _verticalWallJump;
                    
                    if (wallIsToRight)
                    {
                        // Jump to left
                        Velocity.x = _horizontalWallJump * -1f;
                        FacingDirection = FacingDirection.Left;
                    }
                    else
                    {
                        // Jump to right
                        Velocity.x = _horizontalWallJump;
                        FacingDirection = FacingDirection.Right;
                    }
                    
                    _isBufferedJumpInput = false;
                    _isJumpInput = false;
                    _playerJump.SetIsInCoyoteTime(false);
                    _isMoveInput = false;
                    
                    _moveInput.x = 0f;

                    _hasWallJumped = true;
                    _playerJump.SetHasJumped(true);

                    _currentNumberOfWallJumps++;

                    if (_debugUseAnimations)
                    {
                        animator.SetTrigger(JumpTriggerID);
                    }
                    
                    if (_debugUseSounds)
                    {
                        _playerSounds.PlayWallJumpSound();
                    }
                }
            }

            if (_hasWallJumped && (Time.time - _jumpInputTime) > _wallJumpInputDisableTime)
            {
                // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
                // you wall jump, and maintain the exact same input, thus no input read up
                _moveInput.x = Input.GetAxisRaw("Horizontal");
                _moveInput.y = Input.GetAxisRaw("Vertical");
                _isMoveInput = true;
                _hasWallJumped = false;
            }
        }
        
        private void DropThroughPlatform()
        {
            BoxRayCollider2D customCollider2D = movementController.customCollider2D;
            // JA:29/03/22 Not sure if you should use frame counting for this instead of a timer...???
            if (_hasFallenThroughPlatform && _fallThroughPlatformTimer < 20)
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
            if (_debugDisableWallJumpSlide) return;
            
            bool collisionLeftRight = FacingDirection == FacingDirection.Left ?
                movementController.customCollider2D.CollisionLeft : movementController.customCollider2D.CollisionRight;
            if (_isMoveInput && !_isGrounded && collisionLeftRight)
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

        private void UpdateMovement()
        {
            // Note to self: could do a switch on movestate here instead?
            if (_playerDash.UpdateDash())
            {
                return;
            }
            
            _playerHorizontalMovement.Update();
        }
        
        #endregion

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadRollInput(InputAction.CallbackContext context)
        {
            if (context.started && !_playerDash.IsDashOnCooldown() && !(entityDaze && entityDaze.isDazed))
            {
                _playerDash.DashState = DashState.StartDash;
            }
        }

        #region Combat
        
        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadAttackInput(InputAction.CallbackContext context)
        {
            if (!_debugUseAnimations || (!entityBlock || entityBlock.IsBlocking()) && entityBlock)
            {
                return;
            }

            if (!context.started)
            {
                return;
            }
            
            // Disabling this because we don't have up / down right now.

            const float verticalInputThreshold = 0.5f;

            if (_moveInput.y > verticalInputThreshold)
            {
                AttackDirection = AttackDirection.Up;
            }
            else if (!_isGrounded && (_moveInput.y < -verticalInputThreshold))
            {
                AttackDirection = AttackDirection.Down;
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
                AttackDirection = FacingDirection == FacingDirection.Left ? AttackDirection.Left : AttackDirection.Right;
            }
            
            if (_attacksDrivenByAnimations)
            {
                animator.SetTrigger(AttackTriggerID);
            }
            else
            {
                _playerAttackDriver.StartAttack();
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
            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Dash) != PrototypeCancellables.None)
            {
                if (_playerDash.DashState == DashState.StartDash)
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

        public PlayerAttackDriver GetPlayerAttackDriver()
        {
            return _playerAttackDriver;
        }

        #endregion
    }
}

