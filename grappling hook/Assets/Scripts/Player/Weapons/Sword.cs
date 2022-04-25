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

        #region UnityEvents
        public override void DrawGizmos(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Gizmos.DrawWireSphere(attackerPosition + GetCircleCentre(attackDirection), attackRadius);
        }
        #endregion

        #region Particles

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackParticle(AttackDirection attackDirection){}

        // public override void ShowAttackHitParticle(Transform hitEntityTransform){}
        
        #endregion
        
        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, Vector2 attackerPosition, AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Vector2 overlapCirclePosition = GetCircleCentre(attackDirection);
            Physics2D.OverlapCircle(overlapCirclePosition + attackerPosition, attackRadius, contactFilter2D, detectedObjects);
        }

        public override void DrawLineRenderer(LineRenderer lineRenderer, AttackDirection attackDirection)
        {
            lineRenderer.DrawCircle(attackRadius, GetCircleCentre(attackDirection));
        }

        private Vector2 GetCircleCentre(AttackDirection attackDirection)
        {
            Vector2 circleCentre;
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    circleCentre = new Vector2(0f, attackRadius + VerticalAttackWeaponHeightOffset);
                    break;
                case AttackDirection.Down:
                    circleCentre = new Vector2(0f, -attackRadius + VerticalAttackWeaponHeightOffset);
                    break;
                case AttackDirection.Left:
                    circleCentre = new Vector2(-attackRadius, HorizontalAttackWeaponHeightOffset);
                    break;
                case AttackDirection.Right:
                    circleCentre = new Vector2(attackRadius, HorizontalAttackWeaponHeightOffset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }
            return circleCentre;
        }
    }
}