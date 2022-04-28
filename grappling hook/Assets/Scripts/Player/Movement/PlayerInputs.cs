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
        [Header("Player Components")]
        public PlayerMovement PlayerMovement;
        
        private bool _isMoveInput;
        private Vector2 _moveInput;
        private bool _isJumpInput;
        private float _jumpInputTime;
        private bool _isJumpEndedEarly;
        private bool _isBufferedJumpInput;

        public AttackDirection AttackDirection { get; private set; } 
        
        // Attacks
        [Header("Attacking")] 
        [NonSerialized] public bool IsAttacking;
        [NonSerialized] public bool IsInPreDamageAttackPhase = true;
        public PlayerEquipment CurrentPlayerEquipment;
        [SerializeField] private bool _attacksDrivenByAnimations = true;
        [SerializeField] private PlayerAttackDriver _playerAttackDriver;
        [SerializeField] private float _downAttackJumpVelocity = 15f;
        
        [Header("Parrying")] [SerializeField] private EntityParry entityParry;
        [Header("Blocking")] [SerializeField] private EntityBlock entityBlock;
        [Header("Knockback")] [SerializeField] private EntityKnockback entityKnockback;
        [Header("Daze")] [SerializeField] private EntityDaze entityDaze;
        
        [Header("Prototyping")] public PlayerCombatPrototyping playerCombatPrototyping;
        
        private void Awake()
        {
            PlayerMovement.Initialise(entityBlock);
        }

        // Start is called before the first frame update
        private void Start()
        {
            PlayerMovement.Start();
        }
        
        private void OnGUI()
        {
            _playerAttackDriver.ShowDebugGUI();
        }

        // Update is called once per frame
        private void Update()
        {
            // Input (could put this in an input function???)
            CheckBufferedJumpInput();
            
            // Movement
            PlayerMovement.Update(ref _isMoveInput, ref _moveInput, ref _isJumpInput, 
                ref _isBufferedJumpInput, ref _isJumpEndedEarly, _jumpInputTime, IsAttacking);

            // Attacks
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

        public void ResetMoveSpeed()
        {
            // TODO Need to change in Flow script...
            PlayerMovement.PlayerHorizontalMovement.ResetMoveSpeed();
        }
        
        public void MultiplyMoveSpeed(float multiple)
        {
            // TODO Need to change in Flow script...
            PlayerMovement.PlayerHorizontalMovement.MultiplyMoveSpeed(multiple);
        }
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

            if (context.canceled && (Time.time - _jumpInputTime < PlayerMovement.PlayerJump.GetEarlyCancelTime()))
            {
                _isJumpEndedEarly = true;
            }
        }
        
        public void DownAttackJump()
        {
            PlayerMovement.Velocity.y = _downAttackJumpVelocity;
        }

        private void CheckBufferedJumpInput()
        {
            // "When the character is already in the air pressing jump moments before the ground will trigger jump as soon as they land"
            // http://www.davetech.co.uk/gamedevplatformer
            if (_isJumpInput && !PlayerMovement.IsGrounded())
            {
                _isBufferedJumpInput = true;
            }
            
            // Remove buffered jump if it has been too long.
            if (_isBufferedJumpInput && (Time.time - _jumpInputTime) >PlayerMovement.PlayerJump.GetJumpBufferTime())
            {
                _isBufferedJumpInput = false;
            }
        }

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
            if (PlayerMovement.PlayerWallJumpSlide.HasWallJumped())
            {
                //used to stop the player moving for a short period after they have wall jumped
                //once it works it should let wall jumps be chained together
                _moveInput = Vector2.zero;
            }
            if (_moveInput.x != 0)
            {
                _isMoveInput = true;
                if (entityKnockback.IsBeingKnockedBack() ||
                    (IsAttacking && !playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
                {
                    return;
                }
                if (_moveInput.x < 0)
                {
                    PlayerMovement.FacingDirection = FacingDirection.Left;
                    PlayerMovement.PlayerAnimator.SetSpriteFlipX(true);
                }
                else if (_moveInput.x > 0)
                {                    
                    PlayerMovement.FacingDirection = FacingDirection.Right;
                    PlayerMovement.PlayerAnimator.SetSpriteFlipX(false);
                }
            }
            else
            {
                _isMoveInput = false;
            }
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadDashInput(InputAction.CallbackContext context)
        {
            if (context.started && !PlayerMovement.PlayerDash.IsDashOnCooldown() && !(entityDaze && entityDaze.isDazed))
            {
                PlayerMovement.PlayerDash.DashState = DashState.StartDash;
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
            else if (!PlayerMovement.IsGrounded() && (_moveInput.y < -verticalInputThreshold))
            {
                AttackDirection = AttackDirection.Down;
            }
            else
            {
                if (playerCombatPrototyping.data.canChangeDirectionsDuringAttack)
                {
                    if (_moveInput.x < 0)
                    {
                        PlayerMovement.FacingDirection = FacingDirection.Left;
                        PlayerMovement.PlayerAnimator.SetSpriteFlipX(true);
                    }
                    else if ( _moveInput.x > 0 )
                    {
                        PlayerMovement.FacingDirection = FacingDirection.Right;
                        PlayerMovement.PlayerAnimator.SetSpriteFlipX(false);
                    }
                }
                AttackDirection = (PlayerMovement.FacingDirection == FacingDirection.Left) ? AttackDirection.Left : AttackDirection.Right;
            }
            
            if (_attacksDrivenByAnimations)
            {
                PlayerMovement.PlayerAnimator.SetTriggerAttack();
            }
            else
            {
                _playerAttackDriver.StartAttack();
            }
        }

        private void CheckIfAttackIsCancellable()
        {
            // Cancellable attack phases
            if (!IsAttacking) return;

            // TODO There are only really two phases right now
            // the actual attack phase is only 1 frame right now.
            if (IsInPreDamageAttackPhase)
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
                if (PlayerMovement.PlayerDash.DashState == DashState.StartDash)
                {
                    IsAttacking = false;
                    PlayerMovement.PlayerAnimator.PlayState("Player_Idle");
                    // todo getting playercombat here is bad.
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }

            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
            {
                if (_isJumpInput)
                {
                    IsAttacking = false;
                    PlayerMovement.PlayerAnimator.PlayState("Player_Jump");
                    GetComponent<PlayerCombat>().ForceHideAttackParticles();
                }
            }
                
            if ((playerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
            {
                if (_isMoveInput)
                {
                    IsAttacking = false;
                    PlayerMovement.PlayerAnimator.PlayState("Player_Idle");
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
            if (!entityParry || IsAttacking)
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

            if (context.performed && !IsAttacking)
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

