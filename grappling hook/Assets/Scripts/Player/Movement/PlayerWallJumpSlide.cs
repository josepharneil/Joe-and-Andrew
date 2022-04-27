using System;
using Entity;
using Physics;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerWallJumpSlide
    {
        [Header("Wall Jump")]
        [SerializeField] private float _verticalWallJump = 13f;
        [SerializeField] private float _horizontalWallJump = 13f;
        [SerializeField] private float _wallJumpInputDisableTime = 0.2f;
        [SerializeField] private float _wallJumpCoyoteDuration = 0.12f;
        [SerializeField] private int _maxNumberOfWallJumpsBeforeGrounding = 2;
        [SerializeField] private float _wallJumpSkinWidth = 0.25f;
        private float _lastWalledTime;
        private int _currentNumberOfWallJumps = 0;
        private bool _hasWallJumped;

        [Header("Wall Slide")]
        [SerializeField] private float _wallSlideGravityMultiplier = 0.3f;
        private bool _isWallSliding = false;
        
        [Header("Debug")]
        [SerializeField] private bool _debugDisableWallJump = false;
        [SerializeField] private bool _debugDisableWallSlide = false;

        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");

        #region Getters & Setters

        public bool IsWallSliding() => _isWallSliding;
        public float GetWallSlideGravityMultiplier() => _wallSlideGravityMultiplier;
        public bool HasWallJumped() => _hasWallJumped;
        public void SetHasWallJumped(bool hasWallJumped) => _hasWallJumped = hasWallJumped;
        #endregion

        #region WallJump

        public void ResetWallJumpCounter() => _currentNumberOfWallJumps = 0;

        public void UpdateWallJump(ref bool ref_isJumpInput, ref bool ref_isBufferedJumpInput, bool isGrounded, 
            ref bool ref_isInCoyoteTime, ref bool ref_isMoveInput, float jumpInputTime,
            BoxRayCollider2D boxRayCollider2D, ref Vector2 ref_playerVelocity, ref FacingDirection facingDirection,
            ref Vector2 ref_moveInput, PlayerInputs playerInputs, PlayerJump playerJump, PlayerAnimator playerAnimator)
        {
            if (_debugDisableWallJump) return;
            
            if (!isGrounded && !ref_isInCoyoteTime &&
                (_currentNumberOfWallJumps < _maxNumberOfWallJumpsBeforeGrounding))
            {
                // Check for wall to right / left OR check for wall jump coyote
                boxRayCollider2D.CheckHorizontalCollisions(out bool wallIsToLeft, out bool wallIsToRight, _wallJumpSkinWidth);

                bool isAgainstWall = wallIsToLeft || wallIsToRight;
                if(wallIsToLeft && wallIsToRight)
                {
                    Debug.LogError("WALL JUMPING: Wall to the left AND to the right: This implies bad level design? Not sure what to do here.", playerInputs);
                }
                if (isAgainstWall)
                {
                    _lastWalledTime = Time.time;
                }
                bool isInWallJumpCoyote = (Time.time - _lastWalledTime) < _wallJumpCoyoteDuration;
                
                bool jumpFromWall = ref_isJumpInput && (isAgainstWall || isInWallJumpCoyote);
                if (jumpFromWall)
                {
                    ref_playerVelocity.y = _verticalWallJump;
                    
                    if (wallIsToRight)
                    {
                        // Jump to left
                        ref_playerVelocity.x = _horizontalWallJump * -1f;
                        facingDirection = FacingDirection.Left;
                    }
                    else
                    {
                        // Jump to right
                        ref_playerVelocity.x = _horizontalWallJump;
                        facingDirection = FacingDirection.Right;
                    }
                    
                    ref_isBufferedJumpInput = false;
                    ref_isJumpInput = false;
                    ref_isInCoyoteTime = false;
                    ref_isMoveInput = false;
                    
                    ref_moveInput.x = 0f;

                    _hasWallJumped = true;
                    playerJump.SetHasJumped(true);

                    _currentNumberOfWallJumps++;

                    playerAnimator.SetTriggerJump();

                    playerInputs.GetPlayerSounds().PlayWallJumpSound();
                }
            }

            if (_hasWallJumped && (Time.time - jumpInputTime) > _wallJumpInputDisableTime)
            {
                // JA:29/03/22 Not sure if this is a good idea, but it fixes the instance where
                // you wall jump, and maintain the exact same input, thus no input read up
                ref_moveInput.x = Input.GetAxisRaw("Horizontal");
                ref_moveInput.y = Input.GetAxisRaw("Vertical");
                ref_isMoveInput = true;
                _hasWallJumped = false;
            }
        }

        #endregion

        #region WallSlide
        public void UpdateWallSlide(bool isMoveInput, bool isGrounded, FacingDirection facingDirection, bool isCollisionLeft, bool isCollisionRight)
        {
            if (_debugDisableWallSlide) return;
            
            bool collisionLeftRight = facingDirection == FacingDirection.Left ? isCollisionLeft : isCollisionRight;
            if (isMoveInput && !isGrounded && collisionLeftRight)
            {
                _isWallSliding = true;
            }
            else
            {
                _isWallSliding = false;
            }
        }
        #endregion
    }
}