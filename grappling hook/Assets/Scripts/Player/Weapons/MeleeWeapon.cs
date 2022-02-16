using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    public abstract class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] protected int _weaponDamage;
        [SerializeField] protected float _knockbackAmount;

        public abstract void DrawGizmos(FacingDirection facingDirection);
        
        public abstract void ShowAttackParticle(AttackDirection attackDirection);

        public abstract void ShowAttackHitParticle();

        public abstract void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, AttackDirection attackDirection);

        public abstract bool TryHitDetectedObjects(List<Collider2D> detectedObjects,
            out bool firstEnemyHitPositionIsSet,
            out Vector2 firstEnemyHitPosition);

        public abstract void KnockbackPlayer(bool shouldKnockbackPlayer, Vector2 firstEnemyHitPosition);

        public abstract void ForceHideAttackParticles();
    }
}