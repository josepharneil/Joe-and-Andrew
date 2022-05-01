using System;
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

    [Serializable] public class PlayerCombat
    {
        [Header("Setup")]
        [SerializeField] private LayerMask whatIsDamageable;

        [Header("Shake")] 
        [SerializeField] private CameraShakeData _cameraShakeData; 

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
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _lineRenderDuration = 0.2f;
        private float _lineRendererStartTime = 0f;
        private bool _isShowingLineRenderer = false;
        private AttackDirection _lineRendererAttackDirection;

        [Header("Debug")]
        [SerializeField] private bool _showGizmos = false;
        [SerializeField] private bool _showLineRenderer = false;

        [Header("Components")]
        [SerializeField] private PlayerFlow flow;

        [NonSerialized] public Transform PlayerTransform;
        [NonSerialized] private PlayerAttacks _playerAttacks;
        [NonSerialized] private PlayerEquipment _currentPlayerEquipment;

        public void Initialise(Transform playerTransform, PlayerAttacks playerAttacks, PlayerEquipment currentPlayerEquipment)
        {
            PlayerTransform = playerTransform;
            _playerAttacks = playerAttacks;
            _currentPlayerEquipment = currentPlayerEquipment;
        }

        public void Update()
        {
            if (_slowTimeScaleTimer > 0f)
            {
                _slowTimeScaleTimer -= Time.unscaledDeltaTime;
                if (_slowTimeScaleTimer < 0f)
                {
                    Time.timeScale = 1f;
                }
            }

            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            if (!_isShowingLineRenderer) return;

            float timeSinceLineRendererStart = Time.time - _lineRendererStartTime;
            if (timeSinceLineRendererStart > _lineRenderDuration)
            {
                _currentPlayerEquipment.CurrentMeleeWeapon.HideLineRenderer(_lineRenderer);
                _isShowingLineRenderer = false;
            }
        }

        /// <summary>
        /// Called by Animation Events.
        /// </summary>
        [UsedImplicitly] public void Attack(int attackIndex)//Number is unused right now.
        {
            AttackDirection attackDirection = _playerAttacks.AttackDirection;
            
            _currentPlayerEquipment.CurrentMeleeWeapon.ShowAttackParticle(attackDirection);
            
            ContactFilter2D contactFilter2D = new ContactFilter2D
            {
                layerMask = whatIsDamageable,
                useLayerMask = true,
                useTriggers = true
            };
            _currentPlayerEquipment.CurrentMeleeWeapon.DetectAttackableObjects(out List<Collider2D> detectedObjects, contactFilter2D, PlayerTransform.position, attackDirection);

            StartShowLineRendererForSeconds(attackDirection);

            bool objectHit = TryHitDetectedObjects(detectedObjects, out Vector2? firstEnemyHitPosition);
            if (objectHit)
            {
                flow.BeginFlow();
                ShakeCamera();

                // Knockback player
                if (attackDirection == AttackDirection.Down)
                {
                    _playerAttacks.DownwardsAttackJump();
                }
                else
                {
                    if (_playerKnockback && playerCombatPrototyping.data.doesPlayerGetKnockedBackByOwnAttack && firstEnemyHitPosition.HasValue && _currentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToPlayer != 0f)
                    {
                        _playerKnockback.StartKnockBack(firstEnemyHitPosition.Value, _currentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToPlayer);
                    }
                }

                // Instantiate a hit particle here if we want only once per attack.
            }

            // At the end, we're now post damage.
            _playerAttacks.IsInPreDamageAttackPhase = false;
        }

        private void StartShowLineRendererForSeconds(AttackDirection attackDirection)
        {
            if (!_showLineRenderer) return;
            if (!_lineRenderer) return;
            if (!_currentPlayerEquipment) return;
            if (!_currentPlayerEquipment.CurrentMeleeWeapon) return;

            _isShowingLineRenderer = true;
            _lineRendererAttackDirection = attackDirection;
            _currentPlayerEquipment.CurrentMeleeWeapon.DrawLineRenderer(_lineRenderer, _lineRendererAttackDirection);
            _lineRendererStartTime = Time.time;
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
                    Transform hitboxTransform = entityHitbox.transform;
                    enemyKnockbackPosition ??= hitboxTransform.position;

                    // Instantiate a hit particle here if we want particles for EACH hit enemy
                    _currentPlayerEquipment.CurrentMeleeWeapon.ShowAttackHitParticle(hitboxTransform);
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

                DealsKnockback = _currentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToTarget != 0f,
                KnockbackOrigin = PlayerTransform.position,
                KnockbackStrength = _currentPlayerEquipment.CurrentMeleeWeapon.KnockbackAmountToTarget,

                DealsDaze = playerCombatPrototyping.data.doesPlayerDealDaze,
            };
            return entityHitbox.Hit(hitData);
        }

        private int CalculateDamageDealt()
        {
            int damageDealt = _currentPlayerEquipment.CurrentMeleeWeapon.WeaponDamage;
            
            // This could also factor in some base player attack value?
            
            if (playerCombatPrototyping.data.doesAttackingParriedDealBonusDamage)
            {
                damageDealt *= playerCombatPrototyping.data.attackParriedBonusDamageAmount;
            }

            return damageDealt;
        }

        private void ShakeCamera()
        {
            CameraManager.Instance.Shake.ShakeCamera(_cameraShakeData);
            Time.timeScale = _slowTimeScaleAmount;
            _slowTimeScaleTimer = _slowTimeScaleDuration;
        }

        public void ForceHideAttackParticles()
        {
            _currentPlayerEquipment.CurrentMeleeWeapon.ForceHideAttackParticles();
        }

#if UNITY_EDITOR
        public void DrawGizmosSelected()
        {
            if (!_showGizmos) return;
            if (!_currentPlayerEquipment) return;
            if (!_currentPlayerEquipment.CurrentMeleeWeapon) return;
            _currentPlayerEquipment.CurrentMeleeWeapon.DrawGizmos(PlayerTransform.position, _playerAttacks.AttackDirection);
        }
#endif
    }
}
