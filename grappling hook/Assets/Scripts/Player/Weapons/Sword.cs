using System.Collections;
using System.Collections.Generic;
using Bolt;
using Entity;
using UnityEngine;

namespace Player
{
    public class Sword : MeleeWeapon
    {
        [Header("Attack Swing Particles")]
        [SerializeField] private float swipeShowTime = 1f;
        [SerializeField] private SpriteRenderer upSwipe;
        [SerializeField] private SpriteRenderer downSwipe;
        [SerializeField] private SpriteRenderer leftSwipe;
        [SerializeField] private SpriteRenderer rightSwipe;
        
        [Header("Attack Position")]
        [SerializeField] private Transform sideAttackHitBoxPosition;
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
                var localPosition = sideAttackHitBoxPosition.localPosition;
                Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y);
                Gizmos.DrawWireSphere(position, attackRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(sideAttackHitBoxPosition.position, attackRadius);
            }
        }
        #endregion

        #region Particles
        public override void ForceHideAttackParticles()
        {
            downSwipe.enabled = false;
            upSwipe.enabled = false;
            rightSwipe.enabled = false;
            leftSwipe.enabled = false;
        }

        public override void ShowAttackParticle(AttackDirection attackDirection)
        {
            IEnumerator ShowSwipeCoroutine()
            {
                SpriteRenderer swipe = upSwipe;
                switch (attackDirection)
                {
                    case AttackDirection.Up:
                        swipe = upSwipe;
                        break;
                    case AttackDirection.Down:
                        swipe = downSwipe;
                        break;
                    case AttackDirection.Left:
                        swipe = leftSwipe;
                        break;
                    case AttackDirection.Right:
                        swipe = rightSwipe;
                        break;
                }
            
                swipe.enabled = true;
            
                yield return new WaitForSeconds(swipeShowTime / GetComponent<PlayerCombat>().playerCombatPrototyping.data.attackSpeed);

                swipe.enabled = false;
            }
            
            StartCoroutine(ShowSwipeCoroutine());
        }
        
        public override void ShowAttackHitParticle(){}
        
        #endregion
        
        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Transform attackPosition = sideAttackHitBoxPosition;

            Vector2 overlapCirclePosition;
            if (attackDirection == AttackDirection.Left)
            {
                var localPosition = attackPosition.localPosition;
                overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y);
            }
            else
            {
                overlapCirclePosition = attackPosition.position;
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