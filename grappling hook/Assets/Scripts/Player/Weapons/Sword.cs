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
            // TODO Check this
            if (facingDirection == FacingDirection.Left)
            {
                var localPosition = attackHitBoxPosition.localPosition;
                Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y);
                Gizmos.DrawWireSphere(position, attackRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(attackHitBoxPosition.position, attackRadius);
            }
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
            
            Vector2 overlapCirclePosition;
            if (attackDirection == AttackDirection.Left)
            {
                var localPosition = attackHitBoxPosition.localPosition;
                overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y);
            }
            else
            {
                overlapCirclePosition = attackHitBoxPosition.position;
            }
            Physics2D.OverlapCircle(overlapCirclePosition, attackRadius, contactFilter2D, detectedObjects);
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