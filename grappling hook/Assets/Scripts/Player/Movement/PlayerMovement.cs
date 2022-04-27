using System;
using Physics;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerMovement
    {
        // [SerializeField] private PlayerDash _playerDash;
        // [SerializeField] private PlayerHorizontalMovement _playerHorizontalMovement;

        private float _lastGroundedTime;
        private bool _isGrounded;

        public float GetLastGroundedTime() => _lastGroundedTime;
        public void SetLastGroundedTime(float lastGroundedTime) => _lastGroundedTime = lastGroundedTime;
        public bool IsGrounded() => _isGrounded;
        public void SetIsGrounded(bool isGrounded) => _isGrounded = isGrounded;
        
        public void Update()
        {
            
        }
        
        // private void UpdateMovement()
        // {
        //     // Note to self: could do a switch on movestate here instead?
        //     if (_playerDash.UpdateDash(_moveInput, _facingDirection, ref _playerHorizontalMovement.MoveState, ref _velocity))
        //     {
        //         return;
        //     }
        //     
        //     _playerHorizontalMovement.Update(_isMoveInput, _moveInput, ref _velocity, movementController.customCollider2D.CollisionBelow);
        // }

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
    }
}