using System;
using Entity;
using Physics;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerMovement
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
        
        [Header("Player Sounds")] [SerializeField] private PlayerSounds _playerSounds;
        
        [Header("Prototyping")] public PlayerCombatPrototyping playerCombatPrototyping;
        
        
        private bool _isMoveInput;
        private Vector2 _moveInput;
        private bool _isJumpInput;
        private float _jumpInputTime;
        private bool _isJumpEndedEarly;
        private bool _isBufferedJumpInput;
        
        
        [HideInInspector] public bool isAttacking;
        private FacingDirection _facingDirection;
        private Vector2 _velocity;

        private float _lastGroundedTime;
        private bool _isGrounded;


        public float GetLastGroundedTime() => _lastGroundedTime;
        public bool IsGrounded() => _isGrounded;

        public void Update()
        {
            UpdateMovement();
            UpdateWallSlide();
            UpdateGrounded();
            UpdateGravity();
            UpdateFallThroughPlatform();
            UpdateWallJump();
            UpdateJump();
            Move();
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
        
        private void UpdateWallSlide()
        {
            _playerWallJumpSlide.UpdateWallSlide(_isMoveInput, _isGrounded, _facingDirection,
                movementController.customCollider2D.CollisionLeft, movementController.customCollider2D.CollisionRight);
        }

        private void UpdateGravity()
        {
            _playerGravity.UpdateGravity(movementController.customCollider2D.CollisionBelow, 
                movementController.customCollider2D.CheckIfHittingCeiling(),
                _isJumpEndedEarly, ref _velocity, _playerJump, _playerWallJumpSlide);
        }
        
        private void UpdateGrounded()
        {
            _playerGravity.UpdateGravity(movementController.customCollider2D.CollisionBelow, 
                movementController.customCollider2D.CheckIfHittingCeiling(),
                _isJumpEndedEarly, ref _velocity, _playerJump, _playerWallJumpSlide);
            
        }

        public void UpdateGrounded(BoxRayCollider2D boxRayCollider2D, PlayerAnimator playerAnimator, 
            PlayerJump playerJump, PlayerWallJumpSlide playerWallJumpSlide, ref bool isMoveInput, ref Vector2 moveInput)
        {
            if(boxRayCollider2D.CheckIfGrounded())
            {
                playerAnimator.SetGrounded(true);
                _isGrounded = true;
                _lastGroundedTime = Time.time;
                playerJump.SetHasJumped(false);
                playerWallJumpSlide.ResetWallJumpCounter();
                playerJump.ResetCurrentNumAerialJumps();
                
                // Cancel wall jump blocking move inputs
                if (playerWallJumpSlide.HasWallJumped())
                {
                    // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
                    // you wall jump, and maintain the exact same input, thus no input read up
                    moveInput.x = Input.GetAxisRaw("Horizontal");
                    moveInput.y = Input.GetAxisRaw("Vertical");
                    isMoveInput = true;
                    playerWallJumpSlide.SetHasWallJumped(false);
                }
            }
            else
            {
                _isGrounded = false;
            }
        }
        
        private void UpdateFallThroughPlatform()
        {
            // TODO Check if we should be directly accessing Input here.. might be okay, but also might not be.
            // Can we not just use _moveInput.y < 0f ?
            _playerFallThroughPlatform.Update(movementController.customCollider2D, ref _isJumpInput, 
                Input.GetAxisRaw("Vertical") < 0f);
        }
        
        private void UpdateWallJump()
        {
            _playerWallJumpSlide.UpdateWallJump(ref _isJumpInput, ref _isBufferedJumpInput, 
                _isGrounded, ref _playerJump.GetIsInCoyoteTime(), ref _isMoveInput, _jumpInputTime, 
                movementController.customCollider2D, ref _velocity, facingDirection: ref _facingDirection, 
                ref _moveInput, _playerSounds, _playerJump, _playerAnimator);
        }

        private void UpdateJump()
        {
            float timeBetweenJumpInputAndLastGrounded = _jumpInputTime - _lastGroundedTime;
            _playerJump.Update(ref _isJumpInput, _isGrounded, ref _isBufferedJumpInput, 
                timeBetweenJumpInputAndLastGrounded, ref _velocity, movementController.customCollider2D.CollisionBelow, _playerAnimator, _playerSounds);
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
                    if (_isGrounded)
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
            
            UpdateFacingDirection();
            SetAnimatorSpeedFloats();
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
        
        private void SetAnimatorSpeedFloats()
        {
            _playerAnimator.SetHorizontalSpeed(Mathf.Abs(_velocity.x));
            _playerAnimator.SetVerticalSpeed(_velocity.y);
        }
    }
}