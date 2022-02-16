using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SpearWeapon", menuName = "Weapons/Melee/Spear")]
    public class Spear : MeleeWeapon
    {
        public override void DrawGizmos(Vector2 attackerPosition, FacingDirection facingDirection)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowAttackParticle(AttackDirection attackDirection)
        {
            throw new System.NotImplementedException();
        }

        public override void ForceHideAttackParticles()
        {
            throw new System.NotImplementedException();
        }

        public override void ShowAttackHitParticle()
        {
            throw new System.NotImplementedException();
        }

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, Vector2 attackerPosition,
            AttackDirection attackDirection)
        {
            throw new System.NotImplementedException();
        }
    }
}