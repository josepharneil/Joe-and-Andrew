using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Transform aboveAttackHitBoxPosition;
        [SerializeField] private Transform belowAttackHitBoxPosition;
        [SerializeField] private float attackRadius;
        
        [Header("Knockback")]
        [SerializeField] private EntityKnockback playerKnockbackable;
        
        private void OnEnable()
        {
            ForceHideAttackParticles();
        }
        
        public override void ForceHideAttackParticles()
        {
            downSwipe.enabled = false;
            upSwipe.enabled = false;
            rightSwipe.enabled = false;
            leftSwipe.enabled = false;
        }

        public override void DrawGizmos(FacingDirection facingDirection)
        {
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

            Gizmos.DrawWireSphere(aboveAttackHitBoxPosition.position, attackRadius);
            Gizmos.DrawWireSphere(belowAttackHitBoxPosition.position, attackRadius);
        }

        public override void ShowAttackParticle(AttackDirection attackDirection)
        {
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    StartCoroutine(ShowSwipe(AttackDirection.Up));
                    break;
                case AttackDirection.Down:
                    StartCoroutine(ShowSwipe(AttackDirection.Down));
                    break;
                case AttackDirection.Left:
                    StartCoroutine(ShowSwipe(AttackDirection.Left));
                    break;
                case AttackDirection.Right:
                    StartCoroutine(ShowSwipe(AttackDirection.Right));
                    break;
            }
        }

        public override void ShowAttackHitParticle(){}

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Transform attackPosition = sideAttackHitBoxPosition;
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    attackPosition = aboveAttackHitBoxPosition;
                    break;
                case AttackDirection.Down:
                    attackPosition = belowAttackHitBoxPosition;
                    break;
                case AttackDirection.Left:
                    break;
                case AttackDirection.Right:
                    break;
            }
            
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

        public override bool TryHitDetectedObjects(List<Collider2D> detectedObjects, out bool firstEnemyHitPositionIsSet,
            out Vector2 firstEnemyHitPosition)
        {
            bool enemyHit = false;
            firstEnemyHitPositionIsSet = false;
            firstEnemyHitPosition = Vector2.zero;
            
            foreach (Collider2D coll in detectedObjects)
            {
                coll.gameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
                if (!entityHitbox) continue;
                
                PlayerCombatPrototyping playerCombatPrototyping = GetComponent<PlayerCombat>().playerCombatPrototyping;
                
                int damageDealt = _weaponDamage;
                if (playerCombatPrototyping.data.doesAttackingParriedDealBonusDamage)
                {
                    damageDealt *= playerCombatPrototyping.data.attackParriedBonusDamageAmount;
                }

                EntityHitData hitData = new EntityHitData
                {
                    DealsDamage = true,
                    DamageToHealth = damageDealt,
                    
                    DealsKnockback = playerCombatPrototyping.data.doesPlayerDealKnockback,
                    KnockbackOrigin = transform.position,
                    KnockbackStrength = playerCombatPrototyping.data.knockbackStrength,

                    DealsDaze = playerCombatPrototyping.data.doesPlayerDealDaze,
                };
                enemyHit = entityHitbox.Hit(hitData);

                if (enemyHit && !firstEnemyHitPositionIsSet)
                {
                    firstEnemyHitPosition = entityHitbox.transform.position;
                    firstEnemyHitPositionIsSet = true;
                }
                
                // Instantiate a hit particle here if we want particles for EACH hit enemy
            }

            return enemyHit;
        }

        public override void KnockbackPlayer(bool shouldKnockbackPlayer, Vector2 firstEnemyHitPosition)
        {
            PlayerCombatPrototyping playerCombatPrototyping = GetComponent<PlayerCombat>().playerCombatPrototyping;
            if (playerKnockbackable && shouldKnockbackPlayer)
            {
                playerKnockbackable.StartKnockBack(firstEnemyHitPosition, _knockbackAmount);
            }
        }
        
        private IEnumerator ShowSwipe(AttackDirection attackDirection)
        {
            SpriteRenderer swipe = upSwipe;
            switch (attackDirection)
            {
                case AttackDirection.Up:
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
    }
}