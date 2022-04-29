using System;
using Entity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

// thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1
// more shameless thieving https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs

namespace Player
{
    public class PlayerInputs : MonoBehaviour
    {
        [NonSerialized] public bool IsMoveInput;
        [NonSerialized] public Vector2 MoveInput;
        [NonSerialized] public bool IsJumpInput;
        [NonSerialized] public float JumpInputTime;
        [NonSerialized] public bool IsJumpEndedEarly;
        [NonSerialized] public bool IsBufferedJumpInput;

        [NonSerialized] private PlayerController _playerController;
        [NonSerialized] private PlayerAnimator _playerAnimator;
        
        [NonSerialized] private EntityParry _entityParry;
        [NonSerialized] private EntityBlock _entityBlock;
        [NonSerialized] private EntityKnockback _entityKnockback;
        [NonSerialized] private EntityDaze _entityDaze;

        public void Initialise(PlayerController playerController, PlayerAnimator playerAnimator, EntityParry entityParry, EntityBlock entityBlock, EntityKnockback entityKnockback, EntityDaze entityDaze)
        {
            _playerController = playerController;
            _playerAnimator = playerAnimator;
            _entityParry = entityParry;
            _entityBlock = entityBlock;
            _entityKnockback = entityKnockback;
            _entityDaze = entityDaze;
        }

        public void Update()
        {
            CheckBufferedJumpInput();
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadJumpInput(InputAction.CallbackContext context)
        {
            if (_entityDaze && _entityDaze.isDazed) return;
            if (context.started)
            {
                IsJumpInput = true;
                IsJumpEndedEarly = false;
                // Note for the future, because this is setting an exact value... it should be fine?? Hopefully...
                // This is a little bit of a "magic number" fix though.
                JumpInputTime = Time.time;
            }

            if (context.canceled && (Time.time - JumpInputTime < _playerController.PlayerMovement.PlayerJump.GetEarlyCancelTime()))
            {
                IsJumpEndedEarly = true;
            }
        }

        private void CheckBufferedJumpInput()
        {
            // "When the character is already in the air pressing jump moments before the ground will trigger jump as soon as they land"
            // http://www.davetech.co.uk/gamedevplatformer
            if (IsJumpInput && !_playerController.PlayerMovement.IsGrounded())
            {
                IsBufferedJumpInput = true;
            }
            
            // Remove buffered jump if it has been too long.
            if (IsBufferedJumpInput && (Time.time - JumpInputTime) >
                _playerController.PlayerMovement.PlayerJump.GetJumpBufferTime())
            {
                IsBufferedJumpInput = false;
            }
        }

        /// <summary>
        /// called by Unity PlayerInput event
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            if (_entityDaze && _entityDaze.isDazed)
            {
                MoveInput = Vector2.zero;
            }
            if (_playerController.PlayerMovement.PlayerWallJumpSlide.HasWallJumped())
            {
                //used to stop the player moving for a short period after they have wall jumped
                //once it works it should let wall jumps be chained together
                MoveInput = Vector2.zero;
            }
            if (MoveInput.x != 0)
            {
                IsMoveInput = true;
                if (_entityKnockback.IsBeingKnockedBack() ||
                    (_playerController.PlayerAttacks.IsAttacking && !_playerController.PlayerAttacks.PlayerCombatPrototyping.data.canChangeDirectionsDuringAttack))
                {
                    return;
                }
                if (MoveInput.x < 0)
                {
                    _playerController.PlayerMovement.FacingDirection = FacingDirection.Left;
                    _playerAnimator.SetSpriteFlipX(true);
                }
                else if (MoveInput.x > 0)
                {                    
                    _playerController.PlayerMovement.FacingDirection = FacingDirection.Right;
                    _playerAnimator.SetSpriteFlipX(false);
                }
            }
            else
            {
                IsMoveInput = false;
            }
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        [UsedImplicitly] public void ReadDashInput(InputAction.CallbackContext context)
        {
            if (context.started && !_playerController.PlayerMovement.PlayerDash.IsDashOnCooldown() && !(_entityDaze && _entityDaze.isDazed))
            {
                _playerController.PlayerMovement.PlayerDash.DashState = DashState.StartDash;
            }
        }

        #region Combat
        
        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadAttackInput(InputAction.CallbackContext context)
        {
            if ((!_entityBlock || _entityBlock.IsBlocking()) && _entityBlock)
            {
                return;
            }

            if (!context.started)
            {
                return;
            }
            
            _playerController.PlayerAttacks.StartAttack(_playerController.PlayerMovement.IsGrounded(), MoveInput, ref _playerController.PlayerMovement.FacingDirection);
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadParryInput(InputAction.CallbackContext context)
        {
            if (!_entityParry || _playerController.PlayerAttacks.IsAttacking)
            {
                return;
            }

            // For now, parrying is instant and you can still move during it.
            if (!context.started)
            {
                return;
            }
            
            _entityParry.CheckParry();
        }

        /// <summary>
        /// Called by PlayerInput Unity Event.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly] public void ReadBlockInput(InputAction.CallbackContext context)
        {
            if (!_entityBlock)
            {
                return;
            }

            if (context.performed && !_playerController.PlayerAttacks.IsAttacking)
            {
                // For now, don't need to worry about whether you're mid parry /attack
                _entityBlock.SetBlocking(true);
            }

            if (context.canceled)
            {
                _entityBlock.SetBlocking(false);
            }
        }

        #endregion
    }
}

