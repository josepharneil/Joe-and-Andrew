using System;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerAnimator
    {
        private static readonly int HorizontalSpeedID = Animator.StringToHash("horizontalSpeed");
        private static readonly int VerticalSpeedID = Animator.StringToHash("verticalSpeed");
        
        private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
        // private static readonly int AttackUpTriggerID = Animator.StringToHash("attackUpTrigger");
        // private static readonly int AttackDownTriggerID = Animator.StringToHash("attackDownTrigger");
        
        private static readonly int JumpTriggerID = Animator.StringToHash("jumpTrigger");
        private static readonly int GroundedTriggerID = Animator.StringToHash("groundedTrigger");
        
        [SerializeField] private Animator _animator;
        
        [Header("Debug")]
        [SerializeField] private bool _debugUseAnimations = true;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite defaultSquareSprite;

        public void SetSpriteFlipX(bool flipX) => spriteRenderer.flipX = flipX;
        
        public void SetGrounded(bool grounded) 
        {
            if (!_debugUseAnimations) return;
            _animator.SetBool(GroundedTriggerID, grounded);
        }
        
        public void SetHorizontalSpeed(float speed)
        {
            if (!_debugUseAnimations) return;
            _animator.SetFloat(HorizontalSpeedID, Mathf.Abs(speed));
        }
        public void SetVerticalSpeed(float speed)
        {
            if (!_debugUseAnimations) return;
            _animator.SetFloat(VerticalSpeedID, Mathf.Abs(speed));
        }

        public void SetTriggerAttack()
        {
            if (!_debugUseAnimations) return;
            _animator.SetTrigger(AttackTriggerID);
        }
        public void SetTriggerJump()
        {
            if (!_debugUseAnimations) return;
            _animator.SetTrigger(JumpTriggerID);
        }

        public void PlayState(string stateName)
        {
            if (!_debugUseAnimations) return;
            _animator.Play(stateName);
        }
        public void PlayState(int stateNameHash)
        {
            if (!_debugUseAnimations) return;
            _animator.Play(stateNameHash);
        }

        public void Start()
        {
            if (!_debugUseAnimations)
            {
                spriteRenderer.sprite = defaultSquareSprite;
            }
        }
    }
}