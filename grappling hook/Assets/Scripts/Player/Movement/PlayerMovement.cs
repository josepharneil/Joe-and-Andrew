using Physics;
using UnityEngine;

namespace Player
{
    public class PlayerMovement
    {
        public void Update()
        {
            
        }

        // private void UpdateGrounded(BoxRayCollider2D boxRayCollider2D, )
        // {
        //     if(boxRayCollider2D.CheckIfGrounded())
        //     {
        //         animator.SetBool(GroundedTriggerID, true);
        //         _isGrounded = true;
        //         _lastGroundedTime = Time.time;
        //         _playerJump.SetHasJumped(false);
        //         _playerWallJumpSlide.ResetWallJumpCounter();
        //         _playerJump.ResetCurrentNumAerialJumps();
        //         
        //         // Cancel wall jump blocking move inputs
        //         if (_playerWallJumpSlide.HasWallJumped())
        //         {
        //             // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
        //             // you wall jump, and maintain the exact same input, thus no input read up
        //             _moveInput.x = Input.GetAxisRaw("Horizontal");
        //             _moveInput.y = Input.GetAxisRaw("Vertical");
        //             _isMoveInput = true;
        //             _playerWallJumpSlide.SetHasWallJumped(false);
        //         }
        //     }
        //     else
        //     {
        //         _isGrounded = false;
        //     }
        // }
    }
}