using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SpearWeapon", menuName = "Weapons/Melee/Spear")]
    public class Spear : MeleeWeapon
    {
        [Header("Attack Size")]
        [SerializeField] private float _attackLength = 2f;
        [SerializeField] private float _attackWidth = 0.5f;
        
        #region Particles
        public override void ShowAttackParticle(AttackDirection attackDirection){}

        public override void ForceHideAttackParticles(){}

        public override void ShowAttackHitParticle(){}
        #endregion

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, Vector2 attackerPosition,
            AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            
            Vector2 overlapBoxCentre = GetRectCentre(attackerPosition, attackDirection);
            Physics2D.OverlapBox(overlapBoxCentre, new Vector2(_attackLength, _attackWidth), 0f, contactFilter2D, detectedObjects);
        }
        
        public override void DrawGizmos(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector2 overlapBoxCentre = GetRectCentre(attackerPosition, attackDirection);
            Gizmos.DrawWireCube(overlapBoxCentre, new Vector2(_attackLength, _attackWidth));
        }
        
        private Vector2 GetRectCentre(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            Vector2 overlapCirclePosition;
            if (attackDirection == AttackDirection.Left)
            {
                overlapCirclePosition = attackerPosition + new Vector2(-_attackLength / 2f, WeaponHeightOffset);
            }
            else
            {
                overlapCirclePosition = attackerPosition + new Vector2(_attackLength / 2f, WeaponHeightOffset);
            }
            return overlapCirclePosition;
        }
    }
}