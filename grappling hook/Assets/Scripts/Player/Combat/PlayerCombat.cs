using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using JetBrains.Annotations;

namespace Player
{
    public enum AttackDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class PlayerCombat : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private LayerMask whatIsDamageable;
        [SerializeField] private PlayerInputs inputs;
        
        [Header("Attack infos")]
        [SerializeField] private Transform sideAttackHitBoxPosition;
        [SerializeField] private Transform aboveAttackHitBoxPosition;
        [SerializeField] private Transform belowAttackHitBoxPosition;
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackRadius;
        
        [Header("Shake")]
        [SerializeField] private CinemachineShake cinemachineShake;
        [SerializeField] private float shakeAmplitude = 3f;
        [SerializeField] private float shakeFrequency = 1f;
        [SerializeField] private float shakeDuration = 0.1f;

        [Header("Gamepad Vibration")]
        [SerializeField] private GamepadVibrator gamepadVibrator;
        
        [Header("Time Scale On Hit")]
        [SerializeField] private float slowTimeScaleDuration = 0.2f;
        [SerializeField] private float slowTimeScaleAmount = 1 / 20f;
        private float _slowTimeScaleTimer = 0f;

        [Header("Prototyping")]
        public PlayerCombatPrototyping playerCombatPrototyping;

        [Header("Weapon")]
        [SerializeField] private MeleeWeapon _currentMeleeWeapon;
        
        private AttackDirection ConvertAnimationEventInfo()
        {
            // No up or down right now.
            FacingDirection facingDirection = inputs.FacingDirection;
            if (facingDirection == FacingDirection.Left)
            {
                return AttackDirection.Left;
            }
            else
            {
                return AttackDirection.Right;
            }
        }

        /// <summary>
        /// Called by Animation Events.
        /// </summary>
        [UsedImplicitly] public void Attack(int attackIndex)//Number is unused right now.
        {
            AttackDirection attackDirection = ConvertAnimationEventInfo();
            
            _currentMeleeWeapon.ShowAttackParticle(attackDirection);
            
            ContactFilter2D contactFilter2D = new ContactFilter2D
            {
                layerMask = whatIsDamageable,
                useLayerMask = true,
                useTriggers = true
            };
            _currentMeleeWeapon.DetectAttackableObjects(out List<Collider2D> detectedObjects, contactFilter2D, attackDirection);

            
            if (TryHitDetectedObjects(detectedObjects, out Vector2? firstEnemyHitPosition))
            {
                ShakeCamera();

                if (firstEnemyHitPosition.HasValue)
                {
                    _currentMeleeWeapon.KnockbackPlayer(firstEnemyHitPosition.Value);
                }

                // Instantiate a hit particle here if we want only once per attack.
            }

            // At the end, we're now post damage.
            inputs.isInPreDamageAttackPhase = false;
        }
        
        private bool TryHitDetectedObjects(List<Collider2D> detectedObjects, out Vector2? knockbackPosition)
        {
            bool enemyHit = false;
            knockbackPosition = null;
            
            foreach (Collider2D coll in detectedObjects)
            {
                coll.gameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
                if (!entityHitbox) continue;
                
                enemyHit = HitEntity(entityHitbox);
                
                // If an enemy is hit and we haven't already set the knockback position.
                if (enemyHit && knockbackPosition == null)
                {
                    knockbackPosition = entityHitbox.transform.position;
                }
                
                // Instantiate a hit particle here if we want particles for EACH hit enemy
            }

            return enemyHit;
        }

        private bool HitEntity(EntityHitbox entityHitbox)
        {
            int damageDealt = CalculateDamageDealt();
            EntityHitData hitData = new EntityHitData
            {
                DealsDamage = true,
                DamageToHealth = damageDealt,

                DealsKnockback = _currentMeleeWeapon.KnockbackAmountToTarget != 0f,
                KnockbackOrigin = transform.position,
                KnockbackStrength = _currentMeleeWeapon.KnockbackAmountToTarget,

                DealsDaze = playerCombatPrototyping.data.doesPlayerDealDaze,
            };
            return entityHitbox.Hit(hitData);
        }

        private int CalculateDamageDealt()
        {
            // This could also factor in some base player attack value?
            
            int damageDealt = _currentMeleeWeapon.WeaponDamage;
            if (playerCombatPrototyping.data.doesAttackingParriedDealBonusDamage)
            {
                damageDealt *= playerCombatPrototyping.data.attackParriedBonusDamageAmount;
            }

            return damageDealt;
        }

        private void ShakeCamera()
        {
            cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
            Time.timeScale = slowTimeScaleAmount;
            _slowTimeScaleTimer = slowTimeScaleDuration;
        }
        
        private void Update()
        {
            if (_slowTimeScaleTimer > 0f)
            {
                _slowTimeScaleTimer -= Time.unscaledDeltaTime;
                if (_slowTimeScaleTimer < 0f)
                {
                    Time.timeScale = 1f;
                }
            }
        }

        public void ForceHideAttackParticles()
        {
            _currentMeleeWeapon.ForceHideAttackParticles();
        }

        private void OnDrawGizmos()
        {
            _currentMeleeWeapon.DrawGizmos(inputs.FacingDirection);
        }
    }
}
