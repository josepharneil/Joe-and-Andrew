using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SpearWeapon", menuName = "Weapons/Melee/Spear")]
    public class Spear : MeleeWeapon
    {
        [Header("Attack Size")]
        [SerializeField] private float _attackLength = 3f;
        [SerializeField] private float _attackWidth = 1.2f;

        #region UnityEvents
        #endregion
        
        #region Particles
        public override void ShowAttackParticle(AttackDirection attackDirection){}

        public override void ForceHideAttackParticles(){}

        // public override void ShowAttackHitParticle(Transform hitEntityTransform){}
        #endregion

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, Vector2 attackerPosition,
            AttackDirection attackDirection)
        {
            detectedObjects = new List<Collider2D>();
            GetRectInfo(attackDirection, out Vector2 centre, out float rectWidth, out float rectHeight);
            Physics2D.OverlapBox(attackerPosition + centre, new Vector2(rectWidth, rectHeight), 0f, contactFilter2D, detectedObjects);
        }

        private void GetRectInfo(AttackDirection attackDirection, out Vector2 centre, out float rectWidth, out float rectHeight)
        {
            switch (attackDirection)
            {
                case AttackDirection.Up:
                    rectHeight = _attackLength;
                    rectWidth = _attackWidth;
                    centre = new Vector2(0f, VerticalAttackWeaponHeightOffset + _attackLength / 2f);
                    break;
                case AttackDirection.Down:
                    rectHeight = _attackLength;
                    rectWidth = _attackWidth;
                    centre = new Vector2(0f, VerticalAttackWeaponHeightOffset - _attackLength / 2f);
                    break;
                case AttackDirection.Left:
                    rectWidth = _attackLength;
                    rectHeight = _attackWidth;
                    centre = new Vector2(-_attackLength / 2f, VerticalAttackWeaponHeightOffset);
                    break;
                case AttackDirection.Right:
                    rectWidth = _attackLength;
                    rectHeight = _attackWidth;
                    centre = new Vector2(_attackLength / 2f, VerticalAttackWeaponHeightOffset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackDirection), attackDirection, null);
            }
        }

        public override void DrawLineRenderer(LineRenderer lineRenderer, AttackDirection attackDirection)
        {
            GetRectInfo(attackDirection, out Vector2 centre, out float rectWidth, out float rectHeight);
            lineRenderer.DrawRectangle(rectWidth, rectHeight, (Vector3)centre);
        }

        public override void DrawGizmos(Vector2 attackerPosition, AttackDirection attackDirection)
        {
            GetRectInfo(attackDirection, out Vector2 centre, out float rectWidth, out float rectHeight);
            Gizmos.DrawWireCube(attackerPosition + centre, new Vector2(rectWidth, rectHeight));
        }
    }
}