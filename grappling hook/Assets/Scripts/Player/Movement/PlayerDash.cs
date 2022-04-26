using System;
using UnityEngine;

namespace Player
{
    public enum DashState
    {
        NotDashing,
        StartDash,
        Dashing,
        EndDash,
    }
    
    [Serializable] public class PlayerDash
    {
        [Header("Stats")]
        [SerializeField] private float dashDistance = 2f;
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashCoolDownDuration = 0.8f;

        [Header("Options")]
        [SerializeField] private bool _dashIsDirectional = false;
        [SerializeField] private bool _dashIsAttack = false;
        [SerializeField] private float _dashDamageRadius = 1f;
        [SerializeField] private int _dashDamage = 5;
        [SerializeField] private bool _debugDashFall = false;

        private Vector2 _dashDirection;
        private float _dashDurationTimer = 0f;
        private float _dashCoolDownTimer = 0f;
        [HideInInspector] public DashState DashState = DashState.NotDashing;

        private PlayerInputs _playerInputs;

        public void Initialise(PlayerInputs playerInputs)
        {
            _playerInputs = playerInputs;
        }

        public void Start()
        {
            DashState = DashState.NotDashing;
        }
        
        public bool UpdateDash()
        {
            bool isInADashState = false;
            
            switch (DashState)
            {
                case DashState.StartDash:
                    StartDash();
                    isInADashState = true;
                    break;
                case DashState.Dashing:
                    Dashing();
                    isInADashState = true;
                    break;
                case DashState.EndDash:
                    StopDash();
                    isInADashState = true;
                    break;
                case DashState.NotDashing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isInADashState;
        }
        
        private void StartDash()
        {
            _dashDurationTimer = 0f;
            _playerInputs.SetMoveState(MoveStateOLD.Rolling);
            DashState = DashState.Dashing;
            if (_dashIsDirectional)
            {
                Vector2 moveInput = _playerInputs.GetMoveInput();
                if (moveInput.x == 0 && moveInput.y == 0)
                {
                    _dashDirection.x = (int)_playerInputs.FacingDirection;
                    _dashDirection.y = 0f;
                }
                else
                {
                    _dashDirection = moveInput.normalized;
                }
            }
            else
            {
                _dashDirection.x = (int)_playerInputs.FacingDirection;
                _dashDirection.y = 0f;
            }
        }

        private void Dashing()
        {
            // Keeps dashing while the timer is on
            float dashDuration = dashDistance / dashSpeed;
            if (_dashDurationTimer <= dashDuration)
            {
                _playerInputs.Velocity = _dashDirection * dashSpeed;
                if (_debugDashFall)
                {
                    _playerInputs.Velocity.y = 0;
                }
                _dashDurationTimer += Time.deltaTime;
            }
            else
            {
                DashState = DashState.EndDash;
            }
        }

        private void StopDash()
        {
            _playerInputs.SetMoveState(MoveStateOLD.Decelerating);
            DashState = DashState.NotDashing;
            _dashCoolDownTimer = Time.time;
        }
        
        public bool IsDashOnCooldown()
        {
            return Time.time - _dashCoolDownTimer <= dashCoolDownDuration;
        }
    }
}