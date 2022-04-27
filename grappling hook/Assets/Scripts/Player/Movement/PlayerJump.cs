using System;
using Entity;
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
        private bool _isInCoyoteTime; // do we need this? TODO check
        private bool _hasJumped;
        
        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
        private static readonly int GroundedTriggerID = Animator.StringToHash("groundedTrigger");

        public float GetFallSpeed() => _fallSpeed;
        public void ResetCurrentNumAerialJumps() => _currentNumAerialJumps = 0;
        public void SetIsInCoyoteTime(bool isInCoyoteTime) => _isInCoyoteTime = isInCoyoteTime;
        public bool GetIsInCoyoteTime() => _isInCoyoteTime;
        public void SetHasJumped() => _hasJumped = true;
        public float GetEarlyJumpMultiplier() => earlyJumpMultiplier;
        public float GetEarlyCancelTime() => earlyJumpCancelTime;
        public float GetJumpBufferTime() => _jumpBufferTime;
        
        
        private PlayerInputs _playerInputs;
        private MovementController _movementController;

        public void Initialise(PlayerInputs playerInputs, MovementController movementController)
        {
            _playerInputs = playerInputs;
            _movementController = movementController;
        }

        public void Update(bool isJumpInput, bool isGrounded, bool isBufferedJumpInput, float timeBetweenJumpInputAndLastGrounded)
        {
            CheckCoyote(isJumpInput, isGrounded, timeBetweenJumpInputAndLastGrounded);
            CalculateJumpApex();
            Jump(isJumpInput, isGrounded, isBufferedJumpInput);
        }

        private void Jump(bool isJumpInput, bool isGrounded, bool isBufferedJumpInput)
        {
            // If we get a jump input, and we're in the air but we've reached our max aerial jumps, turn off the jump input
            if (isJumpInput && !isGrounded && !_isInCoyoteTime && _currentNumAerialJumps >= _maxNumAerialJumps)
            {
                _playerInputs.TurnOffJumpInput();
            }
            
            bool isAerialJump = !isGrounded && (_currentNumAerialJumps < _maxNumAerialJumps);
            bool isGroundAerialOrCoyoteJump = isJumpInput && (isGrounded || _isInCoyoteTime || isAerialJump);
            bool isBufferedJumpFromGround = isBufferedJumpInput && isGrounded;
            
            if (isGroundAerialOrCoyoteJump || isBufferedJumpFromGround)
            {
                _playerInputs.Velocity.y = jumpVelocity;

                _playerInputs.TurnOffBufferedJumpInput();
                _playerInputs.TurnOffJumpInput();
                _isInCoyoteTime = false;

                if (isAerialJump)
                {
                    _currentNumAerialJumps++;
                }
                
                _hasJumped = true;
                
                if (_playerInputs.GetDebugUseAnimations())
                {
                    _playerInputs.GetAnimator().SetTrigger(JumpTriggerID);
                    _playerInputs.GetAnimator().SetBool(GroundedTriggerID, false);
                }

                if (_playerInputs.GetDebugUseSounds())
                {
                    _playerInputs.GetPlayerSounds().PlayJumpSound();
                }
            }
        }
        
        private void CalculateJumpApex()
        {
            if (!_movementController.customCollider2D.CollisionBelow)
            {
                //sets the apexPoint based on how large the y velocity is
                _apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(_playerInputs.Velocity.y));
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