using System;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerAnimator
    {
        private static readonly int HorizontalSpeedID = Animator.StringToHash("horizontalSpeed");
        private static readonly int VerticalSpeedID = Animator.StringToHash("verticalSpeed");
        
        private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
        private static readonly int AttackUpTriggerID = Animator.StringToHash("attackUpTrigger");
        private static readonly int AttackDownTriggerID = Animator.StringToHash("attackDownTrigger");
        
        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
        private static readonly int GroundedTriggerID = Animator.StringToHash("groundedTrigger");
        
        [SerializeField] private Animator _animator;
        
        public void SetGrounded(bool grounded) => _animator.SetBool(GroundedTriggerID, grounded);
        
        public void SetHorizontalSpeed(float speed) => _animator.SetFloat(HorizontalSpeedID, Mathf.Abs(speed));
        public void SetVerticalSpeed(float speed) => _animator.SetFloat(VerticalSpeedID, Mathf.Abs(speed));

        public void SetTriggerAttack() => _animator.SetTrigger(AttackTriggerID);
        public void SetTriggerJump() => _animator.SetTrigger(JumpTriggerID);

        public void PlayState(string stateName) => _animator.Play(stateName);
        public void PlayState(int stateNameHash) => _animator.Play(stateNameHash);
    }
}