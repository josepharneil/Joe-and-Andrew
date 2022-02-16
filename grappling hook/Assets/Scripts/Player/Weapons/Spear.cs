using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    public class Spear : MeleeWeapon
    {
        public override void DrawGizmos(FacingDirection facingDirection)
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

        public override void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D,
            AttackDirection attackDirection)
        {
            throw new System.NotImplementedException();
        }

        public override void KnockbackPlayer(Vector2 firstEnemyHitPosition)
        {
            throw new System.NotImplementedException();
        }
    }
}