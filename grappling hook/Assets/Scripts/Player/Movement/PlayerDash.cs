using System;
using Entity;
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
        
        [Header("Debug")]
        [SerializeField] private bool _debugDashFall = false;

        private Vector2 _dashDirection;
        private float _dashDurationTimer = 0f;
        private float _dashCoolDownTimer = 0f;
        [HideInInspector] public DashState DashState = DashState.NotDashing;
        
        public void Start()
        {
            DashState = DashState.NotDashing;
        }
        
        public bool UpdateDash(Vector2 moveInput, FacingDirection facingDirection, ref MoveState ref_moveState, ref Vector2 ref_velocity)
        {
            bool isInADashState = false;
            
            switch (DashState)
            {
                case DashState.StartDash:
                    StartDash(moveInput, facingDirection, out ref_moveState);
                    isInADashState = true;
                    break;
                case DashState.Dashing:
                    Dashing(ref ref_velocity);
                    isInADashState = true;
                    break;
                case DashState.EndDash:
                    StopDash(out ref_moveState);
                    isInADashState = true;
                    break;
                case DashState.NotDashing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return isInADashState;
        }
        
        private void StartDash(Vector2 moveInput, FacingDirection facingDirection, out MoveState out_MoveState)
        {
            _dashDurationTimer = 0f;
            out_MoveState = MoveState.Dashing;
            DashState = DashState.Dashing;
            if (_dashIsDirectional)
            {
                if (moveInput.x == 0 && moveInput.y == 0)
                {
                    _dashDirection.x = (int)facingDirection;
                    _dashDirection.y = 0f;
                }
                else
                {
                    _dashDirection = moveInput.normalized;
                }
            }
            else
            {
                _dashDirection.x = (int)facingDirection;
                _dashDirection.y = 0f;
            }
        }

        private void Dashing(ref Vector2 ref_velocity)
        {
            // Keeps dashing while the timer is on
            float dashDuration = dashDistance / dashSpeed;
            if (_dashDurationTimer <= dashDuration)
            {
                ref_velocity = _dashDirection * dashSpeed;
                if (_debugDashFall)
                {
                    ref_velocity.y = 0;
                }
                _dashDurationTimer += Time.deltaTime;
            }
            else
            {
                DashState = DashState.EndDash;
            }
        }

        private void StopDash(out MoveState out_moveState)
        {
            out_moveState = MoveState.Decelerating;
            DashState = DashState.NotDashing;
            _dashCoolDownTimer = Time.time;
        }
        
        public bool IsDashOnCooldown()
        {
            return Time.time - _dashCoolDownTimer <= dashCoolDownDuration;
        }
    }
}