using System.Collections.Generic;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

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
        
        [Header("Shake")]
        [SerializeField] private CinemachineShake cinemachineShake;
        [SerializeField] private float shakeAmplitude = 3f;
        [SerializeField] private float shakeFrequency = 1f;
        [SerializeField] private float shakeDuration = 0.1f;

        [Header("Gamepad Vibration")]
        [SerializeField] private GamepadVibrator gamepadVibrator;
        
        [Header("Time Scale On Hit")]
        [SerializeField] private float _slowTimeScaleDuration = 0.15f;
        [SerializeField] private float _slowTimeScaleAmount = 0.01f;
        private float _slowTimeScaleTimer;

        [Header("Prototyping")]
        public PlayerCombatPrototyping playerCombatPrototyping;

        [Header("Knockback")]
        [SerializeField] private EntityKnockback _playerKnockback;
        
        [Header("Weapon")]
        public PlayerEquipment CurrentPlayerEquipment;
        
        private AttackDirection ConvertAnimationEventInfo()
        {
            // No up or down right now.
            FacingDirection facingDirection = inputs.FacingDirection;
            if (facingDirection == FacingDirection.Left)
            {
                return AttackDirection.Left;
            }

            return AttackDirection.Right;
        }

        /// <summary>
        /// Called by Animation Events.
        /// </summary>
        [UsedImplicitly] public void Attack(int attackIndex)//Number is unused right now.
        {
            AttackDirection attackDirection = ConvertAnimationEventInfo();
            
            CurrentPlayerEquipment.CurrentMeleeWeapon.ShowAttackParticle(attackDirection);
            
            ContactFilter2D contactFilter2D = new ContactFilter2D
            {
                layerMask = whatIsDamageable,
                useLayerMask = true,
                useTriggers = true
            };
            CurrentPlayerEquipment.CurrentMeleeWeapon.DetectAttackableObjects(out List<Collider2D> detectedObjects, contactFilter2D, transform.position, attackDirection);
            
            if (TryHitDetectedObjects(detectedObjects, out Vector2? firstEnemyHitPosition))
            {
                ShakeCamera();

                // Knockback player
                if (_playerKnockback && playerCombatPrototyping.data.doesPlayerGetKnockedBackByOwnAttack && firstEnemyHitPosition.HasValue && CurrentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToPlayer != 0f)
                {
                    _playerKnockback.StartKnockBack(firstEnemyHitPosition.Value, CurrentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToPlayer);
                }

                // Instantiate a hit particle here if we want only once per attack.
            }

            // At the end, we're now post damage.
            inputs.isInPreDamageAttackPhase = false;
        }
        
        private bool TryHitDetectedObjects(List<Collider2D> detectedObjects, out Vector2? enemyKnockbackPosition)
        {
            bool anyEnemyHit = false;
            enemyKnockbackPosition = null;
            
            foreach (Collider2D coll in detectedObjects)
            {
                coll.gameObject.TryGetComponent(out EntityHitbox entityHitbox);
                if (!entityHitbox) continue;
                
                bool enemyHit = HitEntity(entityHitbox);
                anyEnemyHit |= enemyHit;
                
                // If an enemy is hit and we haven't already set the knockback position.
                if (enemyHit)
                {
                    var hitboxTransform = entityHitbox.transform;
                    enemyKnockbackPosition ??= hitboxTransform.position;

                    // Instantiate a hit particle here if we want particles for EACH hit enemy
                    CurrentPlayerEquipment.CurrentMeleeWeapon.ShowAttackHitParticle(hitboxTransform);
                }
                
            }

            return anyEnemyHit;
        }

        private bool HitEntity(EntityHitbox entityHitbox)
        {
            int damageDealt = CalculateDamageDealt();
            EntityHitData hitData = new EntityHitData
            {
                DealsDamage = true,
                DamageToHealth = damageDealt,

                DealsKnockback = CurrentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToTarget != 0f,
                KnockbackOrigin = transform.position,
                KnockbackStrength = CurrentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToTarget,

                DealsDaze = playerCombatPrototyping.data.doesPlayerDealDaze,
            };
            return entityHitbox.Hit(hitData);
        }

        private int CalculateDamageDealt()
        {
            int damageDealt = CurrentPlayerEquipment.CurrentMeleeWeapon.WeaponDamage;
            
            // This could also factor in some base player attack value?
            
            if (playerCombatPrototyping.data.doesAttackingParriedDealBonusDamage)
            {
                damageDealt *= playerCombatPrototyping.data.attackParriedBonusDamageAmount;
            }

            return damageDealt;
        }

        private void ShakeCamera()
        {
            cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
            Time.timeScale = _slowTimeScaleAmount;
            _slowTimeScaleTimer = _slowTimeScaleDuration;
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
            CurrentPlayerEquipment.CurrentMeleeWeapon.ForceHideAttackParticles();
        }

        private void OnDrawGizmos()
        {
            if (!CurrentPlayerEquipment) return;
            if (!CurrentPlayerEquipment.CurrentMeleeWeapon) return;
            CurrentPlayerEquipment.CurrentMeleeWeapon.DrawGizmos(transform.position, ConvertAnimationEventInfo());
        }
    }
}
