using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SwordWeapon", menuName = "Weapons/Melee/Sword")]
    public class Sword : MeleeWeapon
    {
        [Header("Attack Position")]
        [SerializeField] private float attackRadius = 2f;

        #region UnityEvents
        private void OnEnable()
        {
            ForceHideAttackParticles();
        }
        
        public override void DrawGizmos(Vector2 attackerPosition, FacingDirection facingDirection, Transform attackHitBoxPosition)
        {
            if (!attackHitBoxPosition) return;
            
            Vector3 position = GetCirclePosition(attackerPosition, facingDirection == FacingDirection.Left ? 
                AttackDirection.Left : AttackDirection.Right,
                attackHitBoxPosition);
            Gizmos.DrawWireSphere(position, attackRadius);
        }
        #endregion

        #region Particles

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackParticle(AttackDirection attackDirection){}
        
        public override void ShowAttackHitParticle(){}
        
        #endregion
        
        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, Vector2 attackerPosition, AttackDirection attackDirection, Transform attackHitBoxPosition)
        {
            detectedObjects = new List<Collider2D>();
            
            Vector2 overlapCirclePosition = GetCirclePosition(attackerPosition, attackDirection, attackHitBoxPosition);
            Physics2D.OverlapCircle(overlapCirclePosition, attackRadius, contactFilter2D, detectedObjects);
        }

        private Vector2 GetCirclePosition(Vector2 attackerPosition, AttackDirection attackDirection, Transform attackHitBoxPosition)
        {
            Vector2 overlapCirclePosition;
            if (attackDirection == AttackDirection.Left)
            {
                Vector3 localPosition = attackHitBoxPosition.localPosition;
                overlapCirclePosition = attackerPosition + new Vector2(-localPosition.x, localPosition.y);
            }
            else
            {
                overlapCirclePosition = attackHitBoxPosition.position;
            }
            return overlapCirclePosition;
        }
    }
}