using System;
using Entity;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerMovement
    {
        [Header("Components")]
        [SerializeField] private MovementController movementController;
        
        [Header("Player Components")]
        public PlayerDash PlayerDash;
        public PlayerHorizontalMovement PlayerHorizontalMovement;
        public PlayerJump PlayerJump;
        public PlayerFallThroughPlatform PlayerFallThroughPlatform;
        public PlayerWallJumpSlide PlayerWallJumpSlide;
        public PlayerGravity PlayerGravity;
        
        private PlayerAnimator _playerAnimator;
        private PlayerSounds _playerSounds;
        
        [NonSerialized] public FacingDirection FacingDirection;
        [NonSerialized] public Vector2 Velocity;

        private float _lastGroundedTime;
        private bool _isGrounded;

        #region Getters & Setters
        public float GetLastGroundedTime() => _lastGroundedTime;
        public bool IsGrounded() => _isGrounded;
        public FacingDirection GetFacingDirection() => FacingDirection;
        
        #endregion

        public void Initialise(PlayerAnimator playerAnimator, EntityBlock entityBlock, PlayerSounds playerSounds)
        {
            PlayerHorizontalMovement.Initialise(entityBlock);
            _playerSounds = playerSounds;
            _playerAnimator = playerAnimator;
        }

        public void Start()
        {
            PlayerDash.Start();
            PlayerHorizontalMovement.Start();
        }

        public void Update(ref bool isMoveInput, ref Vector2 moveInput, ref bool isJumpInput, 
            ref bool isBufferedJumpInput, ref bool isJumpEndedEarly, float jumpInputTime,
            bool isAttacking, PlayerCombatPrototyping playerCombatPrototyping)
        {
            UpdateMovement(isMoveInput, moveInput);
            UpdateWallSlide(isMoveInput);
            UpdateGrounded(ref isMoveInput, ref moveInput);
            UpdateGravity(ref isJumpEndedEarly);
            UpdateFallThroughPlatform(ref isJumpInput);
            UpdateWallJump(ref isMoveInput, ref moveInput, ref isJumpInput, ref isBufferedJumpInput, jumpInputTime);
            UpdateJump(ref isJumpInput, ref isBufferedJumpInput, jumpInputTime);
            Move(ref isMoveInput, ref moveInput, isJumpEndedEarly, isAttacking, playerCombatPrototyping);
            UpdateFacingDirection(moveInput, isAttacking, playerCombatPrototyping);
            SetAnimatorSpeedFloats();
        }
        
        private void UpdateMovement(bool isMoveInput, Vector2 moveInput)
        {
            // Note to self: could do a switch on movestate here instead?
            if (PlayerDash.UpdateDash(moveInput, FacingDirection, ref PlayerHorizontalMovement.MoveState, ref Velocity))
            {
                return;
            }
            
            PlayerHorizontalMovement.Update(isMoveInput, moveInput, ref Velocity, movementController.customCollider2D.CollisionBelow);
        }
        
        private void UpdateWallSlide(bool isMoveInput)
        {
            PlayerWallJumpSlide.UpdateWallSlide(isMoveInput, _isGrounded, FacingDirection,
                movementController.customCollider2D.CollisionLeft, movementController.customCollider2D.CollisionRight);
        }
        
        private void UpdateGrounded(ref bool isMoveInput, ref Vector2 moveInput)
        {
            if(movementController.customCollider2D.CheckIfGrounded())
            {
                _playerAnimator.SetGrounded(true);
                _isGrounded = true;
                _lastGroundedTime = Time.time;
                PlayerJump.SetHasJumped(false);
                PlayerWallJumpSlide.ResetWallJumpCounter();
                PlayerJump.ResetCurrentNumAerialJumps();
                
                // Cancel wall jump blocking move inputs
                if (PlayerWallJumpSlide.HasWallJumped())
                {
                    // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
                    // you wall jump, and maintain the exact same input, thus no input read up
                    moveInput.x = Input.GetAxisRaw("Horizontal");
                    moveInput.y = Input.GetAxisRaw("Vertical");
                    isMoveInput = true;
                    PlayerWallJumpSlide.SetHasWallJumped(false);
                }
            }
            else
            {
                _isGrounded = false;
            }
        }
        
        private void UpdateGravity(ref bool isJumpEndedEarly)
        {
            PlayerGravity.UpdateGravity(movementController.customCollider2D.CollisionBelow, 
                movementController.customCollider2D.CheckIfHittingCeiling(),
                ref isJumpEndedEarly, ref Velocity, PlayerJump, PlayerWallJumpSlide);
        }
        
        private void UpdateFallThroughPlatform(ref bool isJumpInput)
        {
            // TODO Check if we should be directly accessing Input here.. might be okay, but also might not be.
            // Can we not just use _moveInput.y < 0f ?
            PlayerFallThroughPlatform.Update(movementController.customCollider2D, ref isJumpInput, 
                Input.GetAxisRaw("Vertical") < 0f);
        }
        
        private void UpdateWallJump(ref bool isMoveInput, ref Vector2 moveInput, ref bool isJumpInput, ref bool isBufferedJumpInput, float jumpInputTime)
        {
            PlayerWallJumpSlide.UpdateWallJump(ref isJumpInput, ref isBufferedJumpInput, 
                _isGrounded, ref PlayerJump.GetIsInCoyoteTime(), ref isMoveInput, jumpInputTime, 
                movementController.customCollider2D, ref Velocity, facingDirection: ref FacingDirection, 
                ref moveInput, _playerSounds, PlayerJump, _playerAnimator);
        }

        private void UpdateJump(ref bool isJumpInput, ref bool isBufferedJumpInput, float jumpInputTime)
        {
            float timeBetweenJumpInputAndLastGrounded = jumpInputTime - _lastGroundedTime;
            PlayerJump.Update(ref isJumpInput, _isGrounded, ref isBufferedJumpInput, 
                timeBetweenJumpInputAndLastGrounded, ref Velocity, movementController.customCollider2D.CollisionBelow, _playerAnimator, _playerSounds);
        }
        
        private void Move(ref bool isMoveInput, ref Vector2 moveInput, bool isJumpEndedEarly, bool isAttacking, PlayerCombatPrototyping playerCombatPrototyping)
        {
            if (isAttacking && playerCombatPrototyping.data.movementDisabledByAttacks)
            {
                // TODO @JA Not sure what to do here.
                if (_isGrounded)
                {
                    Velocity.x = 0f;
                }

                UpdateGrounded(ref isMoveInput, ref moveInput);
                UpdateGravity(ref isJumpEndedEarly);
                movementController.Move(Velocity);
            }
            // Either we're not attacking; or we are attacking but movement isn't disabled by attacking
            else
            {
                movementController.Move(Velocity);
            }
        }
        
        private void UpdateFacingDirection(Vector2 moveInput, bool isAttacking, PlayerCombatPrototyping playerCombatPrototyping)
        {
            if (!isAttacking || (isAttacking && playerCombatPrototyping.data.canChangeDirectionsDuringAttack))
            {
                if (moveInput.x < 0)
                {
                    FacingDirection = FacingDirection.Left;
                    _playerAnimator.SetSpriteFlipX(true);
                }
                else if (moveInput.x > 0)
                {
                    FacingDirection = FacingDirection.Right;
                    _playerAnimator.SetSpriteFlipX(false);
                }
            }
        }
        
        private void SetAnimatorSpeedFloats()
        {
            _playerAnimator.SetHorizontalSpeed(Mathf.Abs(Velocity.x));
            _playerAnimator.SetVerticalSpeed(Velocity.y);
        }
    }
}