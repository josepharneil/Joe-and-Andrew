using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SwordWeapon", menuName = "Weapons/Melee/Sword")]
    public class Sword : MeleeWeapon
    {
        [Header("Attack Size")]
        [SerializeField] private float attackRadius = 2f;

        private ParticleSystem _particleOnHit;
        
        #region UnityEvents
        private void OnEnable()
        {
            ParticleOnHit.TryGetComponent(out _particleOnHit);
        }
        
        public override void DrawGizmos(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector3 position = GetCirclePosition(attackerPosition, attackDirection);
            Gizmos.DrawWireSphere(position, attackRadius);
        }
        #endregion

        #region Particles

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackParticle(AttackDirection attackDirection){}

        public override void ShowAttackHitParticle(Transform hitEntityTransform)
        {
            if (_particleOnHit)
            {
                ParticleSystem newParticleSystem = Instantiate(_particleOnHit, hitEntityTransform.position, Quaternion.identity, hitEntityTransform);
                newParticleSystem.Play();
            }
        }
        
        #endregion
        
        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, Vector2 attackerPosition, AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Vector2 overlapCirclePosition = GetCirclePosition(attackerPosition, attackDirection);
            Physics2D.OverlapCircle(overlapCirclePosition, attackRadius, contactFilter2D, detectedObjects);
        }

        public override void DrawLineRenderer(LineRenderer lineRenderer, AttackDirection attackDirection)
        {
            Vector3 circleOrigin = new Vector2(attackRadius, WeaponHeightOffset);
            if (attackDirection == AttackDirection.Left)
            {
                circleOrigin = new Vector3(-attackRadius, WeaponHeightOffset);
            }
            lineRenderer.DrawCircle(attackRadius, circleOrigin);
        }

        private Vector2 GetCirclePosition(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector2 overlapCirclePosition;
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    overlapCirclePosition = attackerPosition + new Vector2(0f, attackRadius);
                    break;
                case AttackDirection.Down:
                    overlapCirclePosition = attackerPosition + new Vector2(0f, -attackRadius);
                    break;
                case AttackDirection.Left:
                    overlapCirclePosition = attackerPosition + new Vector2(-attackRadius, WeaponHeightOffset);
                    break;
                case AttackDirection.Right:
                    overlapCirclePosition = attackerPosition + new Vector2(attackRadius, WeaponHeightOffset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }
            return overlapCirclePosition;
        }
    }
}