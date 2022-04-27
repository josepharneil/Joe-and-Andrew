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

        public void Jump()
        {
            // If we get a jump input, and we're in the air but we've reached our max aerial jumps, turn off the jump input
            if (_playerInputs.IsJumpInput() && !_playerInputs.IsGrounded() && !_isInCoyoteTime && _currentNumAerialJumps >= _maxNumAerialJumps)
            {
                _playerInputs.SetIsJumpInput(false);
            }
            
            bool isAerialJump = !_playerInputs.IsGrounded() && (_currentNumAerialJumps < _maxNumAerialJumps);
            bool isGroundAerialOrCoyoteJump = _playerInputs.IsJumpInput() && (_playerInputs.IsGrounded() || _isInCoyoteTime || isAerialJump);
            bool isBufferedJumpFromGround = _playerInputs.GetIsBufferedJumpInput() && _playerInputs.IsGrounded();
            
            if (isGroundAerialOrCoyoteJump || isBufferedJumpFromGround)
            {
                _playerInputs.Velocity.y = jumpVelocity;

                _playerInputs.SetIsBufferedJumpInput(false);
                _playerInputs.SetIsJumpInput(false);
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
        
        public void CalculateJumpApex()
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

        public void CheckCoyote()
        {
            if (_playerInputs.IsJumpInput() && !_playerInputs.IsGrounded() && !_hasJumped 
                && (_playerInputs.GetJumpInputTime() - _playerInputs.GetLastGroundedTime()) < coyoteTime )
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