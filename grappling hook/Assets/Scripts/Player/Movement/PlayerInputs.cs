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
        [Header("Movement")] public PlayerMovement PlayerMovement;
        [Header("Attacking")] public PlayerAttacks PlayerAttacks;
        
        private bool _isMoveInput;
        private Vector2 _moveInput;
        private bool _isJumpInput;
        private float _jumpInputTime;
        private bool _isJumpEndedEarly;
        private bool _isBufferedJumpInput;

        [Header("Entity Components")] 
        [SerializeField] private EntityParry entityParry;
        [SerializeField] private EntityBlock entityBlock;
        [SerializeField] private EntityKnockback entityKnockback;
        [SerializeField] private EntityDaze entityDaze;
        
        private void Awake()
        {
            PlayerMovement.Initialise(entityBlock);
            PlayerAttacks.Initialise(PlayerMovement);
        }

        // Start is called before the first frame update
        private void Start()
        {
            PlayerMovement.Start();
        }
        
        private void OnGUI()
        {
            PlayerAttacks.ShowGUI();
        }

        // Update is called once per frame
        private void Update()
        {
            // Input (could put this in an input function???)
            CheckBufferedJumpInput();
            
            // Movement
            PlayerMovement.Update(ref _isMoveInput, ref _moveInput, ref _isJumpInput, 
                ref _isBufferedJumpInput, ref _isJumpEndedEarly, _jumpInputTime, PlayerAttacks.IsAttacking);

            // Attacks
            PlayerAttacks.Update(PlayerMovement.PlayerDash.DashState, PlayerMovement.PlayerAnimator, _isMoveInput, _isJumpInput);
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
                    (PlayerAttacks.IsAttacking && !PlayerAttacks.PlayerCombatPrototyping.data.canChangeDirectionsDuringAttack))
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
            
            PlayerAttacks.StartAttack(PlayerMovement.PlayerAnimator, PlayerMovement.IsGrounded(), _moveInput, ref PlayerMovement.FacingDirection);
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadParryInput(InputAction.CallbackContext context)
        {
            if (!entityParry || PlayerAttacks.IsAttacking)
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

            if (context.performed && !PlayerAttacks.IsAttacking)
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

