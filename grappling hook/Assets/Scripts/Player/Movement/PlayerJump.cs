using System;
using Entity;
using Physics;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerJump
    {        
        [Header("Jump Stats")]
        [SerializeField] private float jumpVelocity = 15f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] [Tooltip("How long the jump buffer will last")] private float _jumpBufferTime;
        [SerializeField] private float minFallSpeed = 35f;
        [SerializeField] private float maxFallSpeed = 40f;
        [SerializeField] private float jumpApexThreshold = 10f;
        [SerializeField] private float earlyJumpMultiplier = 2f;
        [SerializeField] private float earlyJumpCancelTime = 0.15f;
        [SerializeField] private int _maxNumAerialJumps = 1;
        
        private int _currentNumAerialJumps = 0;
        private float _apexPoint; //This becomes 1 at the end of the jump
        private float _fallSpeed;
        private bool _isInCoyoteTime;
        private bool _hasJumped;

        private PlayerInputs _playerInputs;// want to remove this eventually
        
        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
        private static readonly int GroundedTriggerID = Animator.StringToHash("groundedTrigger");
        
        #region Getters & Setters
        public float GetFallSpeed() => _fallSpeed;
        public void ResetCurrentNumAerialJumps() => _currentNumAerialJumps = 0;
        public void SetIsInCoyoteTime(bool isInCoyoteTime) => _isInCoyoteTime = isInCoyoteTime;
        public ref bool GetIsInCoyoteTime() => ref _isInCoyoteTime;
        public void SetHasJumped(bool hasJumped) => _hasJumped = hasJumped;
        public float GetEarlyJumpMultiplier() => earlyJumpMultiplier;
        public float GetEarlyCancelTime() => earlyJumpCancelTime;
        public float GetJumpBufferTime() => _jumpBufferTime;
        #endregion

        public void Initialise(PlayerInputs playerInputs)
        {
            _playerInputs = playerInputs;
            
        }

        public void Update(ref bool ref_isJumpInput, bool isGrounded, ref bool ref_isBufferedJumpInput, 
            float timeBetweenJumpInputAndLastGrounded, ref Vector2 ref_playerVelocity, bool isCollisionBelow, PlayerAnimator playerAnimator)
        {
            CheckCoyote(ref_isJumpInput, isGrounded, timeBetweenJumpInputAndLastGrounded);
            CalculateJumpApex(ref ref_playerVelocity, isCollisionBelow);
            Jump(ref ref_isJumpInput, isGrounded, ref ref_isBufferedJumpInput, ref ref_playerVelocity, playerAnimator);
        }

        private void Jump(ref bool ref_isJumpInput, bool isGrounded, ref bool ref_isBufferedJumpInput, ref Vector2 ref_playerVelocity, PlayerAnimator playerAnimator)
        {
            // If we get a jump input, and we're in the air but we've reached our max aerial jumps, turn off the jump input
            if (ref_isJumpInput && !isGrounded && !_isInCoyoteTime && _currentNumAerialJumps >= _maxNumAerialJumps)
            {
                ref_isJumpInput = false;
            }
            
            bool isAerialJump = !isGrounded && (_currentNumAerialJumps < _maxNumAerialJumps);
            bool isGroundAerialOrCoyoteJump = ref_isJumpInput && (isGrounded || _isInCoyoteTime || isAerialJump);
            bool isBufferedJumpFromGround = ref_isBufferedJumpInput && isGrounded;
            
            if (isGroundAerialOrCoyoteJump || isBufferedJumpFromGround)
            {
                ref_playerVelocity.y = jumpVelocity;

                ref_isBufferedJumpInput = false;
                ref_isJumpInput = false;
                _isInCoyoteTime = false;

                if (isAerialJump)
                {
                    _currentNumAerialJumps++;
                }
                
                _hasJumped = true;
                
                playerAnimator.SetTriggerJump();
                playerAnimator.SetGrounded(false);

                if (_playerInputs.GetDebugUseSounds())
                {
                    _playerInputs.GetPlayerSounds().PlayJumpSound();
                }
            }
        }
        
        private void CalculateJumpApex(ref Vector2 ref_playerVelocity, bool isCollisionBelow)
        {
            if (!isCollisionBelow)
            {
                //sets the apexPoint based on how large the y velocity is
                _apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(ref_playerVelocity.y));
                //uses the apexPoint to lerp between the min and max fallspeeds (our new gravity replacement)
                _fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0f;
            }
        }

        private void CheckCoyote(bool isJumpInput, bool isGrounded, float timeBetweenJumpInputAndLastGrounded)
        {
            if (isJumpInput && !isGrounded && !_hasJumped 
                && timeBetweenJumpInputAndLastGrounded < coyoteTime )
            {
                _isInCoyoteTime = true;
            }
            else
            {
                _isInCoyoteTime = false;
            }
        }
    }
}