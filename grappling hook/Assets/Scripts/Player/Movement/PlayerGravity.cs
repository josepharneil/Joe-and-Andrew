using System;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerGravity
    {
        [SerializeField] private float _fallClamp = -35f;
        
        // Taken from Tarodevs GitHub: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/blob/main/Scripts/PlayerController.cs
        public void UpdateGravity(bool isCollisionBelow, bool isHittingCeiling, bool isJumpEndedEarly, 
            ref Vector2 ref_playerVelocity, PlayerJump playerJump, PlayerWallJumpSlide playerWallJumpSlide)
        {
            if (isCollisionBelow)
            {
                if (ref_playerVelocity.y < 0)
                {
                    ref_playerVelocity.y = 0;
                }
            }
            else
            {
                if (isHittingCeiling)
                {
                    isJumpEndedEarly = true;
                }
                
                // Checks if the player has ended a jump early, and if so increase the gravity
                if (isJumpEndedEarly)
                {
                    ref_playerVelocity.y -= playerJump.GetFallSpeed() * playerJump.GetEarlyJumpMultiplier() * Time.deltaTime;
                }
                else if (playerWallJumpSlide.IsWallSliding() && ref_playerVelocity.y < 0) 
                {
                    ref_playerVelocity.y -= playerJump.GetFallSpeed() * playerWallJumpSlide.GetWallSlideGravityMultiplier() * Time.deltaTime;
                }
                else
                {
                    ref_playerVelocity.y -= playerJump.GetFallSpeed() * Time.deltaTime;
                }
                
                // Makes the player actually fall
                // Clamps the y velocity to a certain value
                if (ref_playerVelocity.y < _fallClamp)
                {
                    ref_playerVelocity.y = _fallClamp;
                }
            }
        }
    }
}