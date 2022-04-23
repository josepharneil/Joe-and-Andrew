using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SpearWeapon", menuName = "Weapons/Melee/Spear")]
    public class Spear : MeleeWeapon
    {
        [Header("Attack Size")]
        [SerializeField] private float _attackLength = 2f;
        [SerializeField] private float _attackWidth = 0.5f;
        private ParticleSystem _particleOnHit;

        #region UnityEvents
        private void OnEnable()
        {
            ParticleOnHit.TryGetComponent(out _particleOnHit);
        }
        #endregion
        
        #region Particles
        public override void ShowAttackParticle(AttackDirection attackDirection){}

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackHitParticle(Transform hitEntityTransform)
        {
            if (_particleOnHit)
            {
                ParticleSystem newParticleSystem = Instantiate(_particleOnHit, hitEntityTransform.position, Quaternion.identity, hitEntityTransform);
                newParticleSystem.Play();
            }
        }
        #endregion

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, Vector2 attackerPosition,
            AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();

            float boxHeight;
            float boxWidth;
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    boxWidth = _attackWidth;
                    boxHeight = _attackLength;
                    break;
                case AttackDirection.Down:
                    boxWidth = _attackWidth;
                    boxHeight = _attackLength;
                    break;
                case AttackDirection.Left:
                    boxWidth = _attackLength;
                    boxHeight = _attackWidth;
                    break;
                case AttackDirection.Right:
                    boxWidth = _attackLength;
                    boxHeight = _attackWidth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }
            
            Vector2 overlapBoxCentre = GetRectCentre(attackerPosition, attackDirection);
            Physics2D.OverlapBox(overlapBoxCentre, new Vector2(boxWidth, boxHeight), 0f, contactFilter2D, detectedObjects);
        }

        public override void DrawLineRenderer(LineRenderer lineRenderer, AttackDirection attackDirection)
        {
            float rectHeight;
            float rectWidth;
            Vector3 origin;
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    rectHeight = -_attackLength;
                    rectWidth = _attackWidth;
                    origin = new Vector3(0, 0f, 0);
                    break;
                case AttackDirection.Down:
                    rectHeight = _attackLength;
                    rectWidth = _attackWidth;
                    origin = new Vector3(0, 0f, 0);
                    break;
                case AttackDirection.Left:
                    rectHeight = _attackWidth;
                    rectWidth = -_attackLength;
                    origin = new Vector3(0, WeaponHeightOffset, 0);
                    break;
                case AttackDirection.Right:
                    rectHeight = _attackWidth;
                    rectWidth = _attackLength;
                    origin = new Vector3(0, WeaponHeightOffset, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }

            lineRenderer.DrawRectangle(rectWidth, rectHeight, origin);
        }

        public override void DrawGizmos(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector2 overlapBoxCentre = GetRectCentre(attackerPosition, attackDirection);
            Gizmos.DrawWireCube(overlapBoxCentre, new Vector2(_attackLength, _attackWidth));
        }
        
        private Vector2 GetRectCentre(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector2 overlapRectPosition;

            switch (attackDirection)
            {
                case AttackDirection.Up:
                    overlapRectPosition = attackerPosition + new Vector2(0f, _attackWidth / 2f);
                    break;
                case AttackDirection.Down:
                    overlapRectPosition = attackerPosition + new Vector2(0f, -_attackWidth / 2f);
                    break;
                case AttackDirection.Left:
                    overlapRectPosition = attackerPosition + new Vector2(-_attackLength / 2f, WeaponHeightOffset);
                    break;
                case AttackDirection.Right:
                    overlapRectPosition = attackerPosition + new Vector2(_attackLength / 2f, WeaponHeightOffset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }
            return overlapRectPosition;
        }
    }
}