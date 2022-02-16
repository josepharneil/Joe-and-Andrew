using System.Collections;
using System.Collections.Generic;
using Bolt;
using Entity;
using UnityEngine;

namespace Player
{
    public class Sword : MeleeWeapon
    {
        [Header("Attack Position")]
        [SerializeField] private Transform attackHitBoxPosition;
        [SerializeField] private float attackRadius = 2f;
        
        [Header("Knockback")]
        [SerializeField] private EntityKnockback playerKnockbackComponent;

        #region UnityEvents
        private void OnEnable()
        {
            ForceHideAttackParticles();
        }
        
        public override void DrawGizmos(FacingDirection facingDirection)
        {
            if (!attackHitBoxPosition) return;
            
            Vector3 position = GetCirclePosition(facingDirection == FacingDirection.Left ? AttackDirection.Left : AttackDirection.Right);
            Gizmos.DrawWireSphere(position, attackRadius);
        }
        #endregion

        #region Particles

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackParticle(AttackDirection attackDirection){}
        
        public override void ShowAttackHitParticle(){}
        
        #endregion
        
        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Vector2 overlapCirclePosition = GetCirclePosition(attackDirection);
            Physics2D.OverlapCircle(overlapCirclePosition, attackRadius, contactFilter2D, detectedObjects);
        }

        private Vector2 GetCirclePosition(AttackDirection attackDirection)
        {
            Vector2 overlapCirclePosition;
            if (attackDirection == AttackDirection.Left)
            {
                Vector3 localPosition = attackHitBoxPosition.localPosition;
                overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y);
            }
            else
            {
                overlapCirclePosition = (Vector2)attackHitBoxPosition.position;
            }
            return overlapCirclePosition;
        }

        public override void KnockbackPlayer(Vector2 firstEnemyHitPosition)
        {
            if (playerKnockbackComponent)
            {
                playerKnockbackComponent.StartKnockBack(firstEnemyHitPosition, _knockbackAmountToPlayer);
            }
        }
    }
}