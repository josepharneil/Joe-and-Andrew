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
        
        [Header("Player Components")]
        [SerializeField] private PlayerDash _playerDash;
        [SerializeField] private PlayerHorizontalMovement _playerHorizontalMovement;
        [SerializeField] private PlayerJump _playerJump;
        [SerializeField] private PlayerFallThroughPlatform _playerFallThroughPlatform;
        [SerializeField] private PlayerWallJumpSlide _playerWallJumpSlide;
        [SerializeField] private PlayerGravity _playerGravity;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerMovement _playerMovement;

        private bool _isMoveInput;
        private bool _isJumpInput;
        private float _jumpInputTime;
        private bool _isJumpEndedEarly;
        private bool _isBufferedJumpInput;

        private FacingDirection _facingDirection;
        public AttackDirection AttackDirection { get; private set; } 
        private Vector2 _velocity;
        private Vector2 _moveInput;
        
        // Attacks
        [Header("Attacking")] 
        [HideInInspector] public bool isAttacking;
        [HideInInspector] public bool isInPreDamageAttackPhase = true;
        public PlayerEquipment CurrentPlayerEquipment;
        [SerializeField] private bool _attacksDrivenByAnimations = true;
        [SerializeField] private PlayerAttackDriver _playerAttackDriver;
        [SerializeField] private float _downAttackJumpVelocity = 15f;

        public FacingDirection GetFacingDirection() => _facingDirection;
        public bool GetDebugUseSounds() => _debugUseSounds;
        public PlayerSounds GetPlayerSounds() => _playerSounds;
        public ref Vector2 GetVelocity() => ref _velocity;
        
        [Header("Parrying")] [SerializeField] private EntityParry entityParry;
        [Header("Blocking")] [SerializeField] private EntityBlock entityBlock;
        [Header("Knockback")] [SerializeField] private EntityKnockback entityKnockback;
        [Header("Daze")] [SerializeField] private EntityDaze entityDaze;
        
        [Header("Prototyping")]
        public PlayerCombatPrototyping playerCombatPrototyping;

        [Header("Player Sounds")] 
        [SerializeField] private PlayerSounds _playerSounds;
        [SerializeField] private bool _debugUseSounds = true;
        
        private void Awake()
        {
            _playerHorizontalMovement.Initialise(entityBlock);
            _playerJump.Initialise(this);
        }

        // Start is called before the first frame update
        private void Start()
        {
            _playerAnimator.Start();
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
            UpdateWallSlide();
            UpdateGrounded();
            CheckBufferedJumpInput();
            UpdateGravity();
            UpdateFallThroughPlatform();
            UpdateWallJump();
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
            _playerAnimator.SetHorizontalSpeed(Mathf.Abs(_velocity.x));
            _playerAnimator.SetVerticalSpeed(_velocity.y);
        }

        private void Move()
        {
            if (!playerCombatPrototyping.data.movementDisabledByAttacks)
            {
                movementController.Move(_velocity);
            }
            else
            {                
                if (isAttacking)
                {
                    // TODO @JA Not sure what to do here.
                    if (_playerMovement.IsGrounded())
                    {
                        _velocity.x = 0f;
                    }

                    UpdateGrounded();
                    UpdateGravity();
                    movementController.Move(_velocity);
                }
                else
                {
                    movementController.Move(_velocity);
                }
            }
        }

        private void UpdateFacingDirection()
        {
            if (!isAttacking || (isAttacking && playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
            {
                if (_moveInput.x < 0)
                {
                    _facingDirection = FacingDirection.Left;
                    _playerAnimator.SetSpriteFlipX(true);
                }
                else if (_moveInput.x > 0)
                {
                    _facingDirection = FacingDirection.Right;
                    _playerAnimator.SetSpriteFlipX(false);
                }
            }
        }

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
        private void UpdateGravity()
        {
            _playerGravity.UpdateGravity(movementController.customCollider2D.CollisionBelow, 
                movementController.customCollider2D.CheckIfHittingCeiling(),
                _isJumpEndedEarly, ref _velocity, _playerJump, _playerWallJumpSlide);
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
            float timeBetweenJumpInputAndLastGrounded = _jumpInputTime - _playerMovement.GetLastGroundedTime();
            _playerJump.Update(ref _isJumpInput, _playerMovement.IsGrounded(), ref _isBufferedJumpInput, 
                timeBetweenJumpInputAndLastGrounded, ref _velocity, movementController.customCollider2D.CollisionBelow, _playerAnimator);
        }
        
        public void DownAttackJump()
        {
            _velocity.y = _downAttackJumpVelocity;
        }

        private void UpdateGrounded()
        {
            _playerMovement.UpdateGrounded(movementController.customCollider2D, _playerAnimator, _playerJump, _playerWallJumpSlide, ref _isMoveInput, ref _moveInput);
        }

        private void CheckBufferedJumpInput()
        {
            // "When the character is already in the air pressing jump moments before the ground will trigger jump as soon as they land"
            // http://www.davetech.co.uk/gamedevplatformer
            if (_isJumpInput && !_playerMovement.IsGrounded())
            {
                _isBufferedJumpInput = true;
            }
            
            // Remove buffered jump if it has been too long.
            if (_isBufferedJumpInput && (Time.time - _jumpInputTime) > _playerJump.GetJumpBufferTime())
            {
                _isBufferedJumpInput = false;
            }
        }
        
        private void UpdateWallJump()
        {
            _playerWallJumpSlide.UpdateWallJump(ref _isJumpInput, ref _isBufferedJumpInput, 
                _playerMovement.IsGrounded(), ref _playerJump.GetIsInCoyoteTime(), ref _isMoveInput, _jumpInputTime, 
                movementController.customCollider2D, ref _velocity, facingDirection: ref _facingDirection, 
                ref _moveInput, this, _playerJump, _playerAnimator);
        }
        
        private void UpdateFallThroughPlatform()
        {
            // TODO Check if we should be directly accessing Input here.. might be okay, but also might not be.
            // Can we not just use _moveInput.y < 0f ?
            _playerFallThroughPlatform.Update(movementController.customCollider2D, ref _isJumpInput, 
                Input.GetAxisRaw("Vertical") < 0f);
        }

        #endregion

        #region WallSlide
        private void UpdateWallSlide()
        {
            _playerWallJumpSlide.UpdateWallSlide(_isMoveInput, _playerMovement.IsGrounded(), _facingDirection,
                movementController.customCollider2D.CollisionLeft, movementController.customCollider2D.CollisionRight);
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
            if (_playerWallJumpSlide.HasWallJumped())
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
                    _facingDirection = FacingDirection.Left;
                    _playerAnimator.SetSpriteFlipX(true);
                }
                else if (_moveInput.x > 0)
                {                    
                    _facingDirection = FacingDirection.Right;
                    _playerAnimator.SetSpriteFlipX(false);
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
            if (_playerDash.UpdateDash(_moveInput, _facingDirection, ref _playerHorizontalMovement.MoveState, ref _velocity))
            {
                return;
            }
            
            _playerHorizontalMovement.Update(_isMoveInput, _moveInput, ref _velocity, movementController.customCollider2D.CollisionBelow);
        }
        
        #endregion

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadDashInput(InputAction.CallbackContext context)
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
            if ((!entityBlock || entityBlock.IsBlocking()) && entityBlock)
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
            else if (!_playerMovement.IsGrounded() && (_moveInput.y < -verticalInputThreshold))
            {
                AttackDirection = AttackDirection.Down;
            }
            else
            {
                if (playerCombatPrototyping.data.canChangeDirectionsDuringAttack)
                {
                    if (_moveInput.x < 0)
                    {
                        _facingDirection = FacingDirection.Left;
                        _playerAnimator.SetSpriteFlipX(true);
                    }
                    else if ( _moveInput.x > 0 )
                    {
                        _facingDirection = FacingDirection.Right;
                        _playerAnimator.SetSpriteFlipX(false);
                    }
                }
                AttackDirection = _facingDirection == FacingDirection.Left ? AttackDirection.Left : AttackDirection.Right;
            }
            
            if (_attacksDrivenByAnimations)
            {
                _playerAnimator.SetTriggerAttack();
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
                    _playerAnimator.PlayState("Player_Idle");
                    // todo getting playercombat here is bad.
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }

            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
            {
                if (_isJumpInput)
                {
                    isAttacking = false;
                    _playerAnimator.PlayState("Player_Jump");
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }
                
            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
            {
                if (_isMoveInput)
                {
                    isAttacking = false;
                    _playerAnimator.PlayState("Player_Idle");
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

