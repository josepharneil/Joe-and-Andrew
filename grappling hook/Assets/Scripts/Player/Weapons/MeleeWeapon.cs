using System.Collections.Generic;
using Entity;
using Ludiq;
using UnityEngine;

namespace Player
{
    public abstract class MeleeWeapon : ScriptableObject
    {
        /// <summary>
        /// Damage that this weapon deals.
        /// </summary>
        [Header("Damage")]
        public int WeaponDamage = 5;

        /// <summary>
        /// Speed of the weapon.
        /// </summary>
        public float WeaponSpeed = 1f;

        /// <summary>
        /// Height offset of the weapon.
        /// </summary>
        [Header("Weapon Position")]
        public float WeaponHeightOffset = 0f;
        
        /// <summary>
        /// Amount that this weapon knocks back the player.
        /// </summary>
        [Header("Knockback")]
        public float KnockbackAmountToPlayer = 8f;

        /// <summary>
        /// Amount that this weapon knocks back the target hit.
        /// </summary>
        public float KnockbackAmountToTarget = 15f;

        [Header("Particles")] 
        [SerializeField] protected GameObject ParticleOnHit;

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        public abstract void DrawGizmos(Vector2 attackerPosition, AttackDirection facingDirection);

        #region Particles
        /// <summary>
        /// Shows particles of the attack, whether an enemy is hit or not.
        /// </summary>
        /// <param name="attackDirection"></param>
        public abstract void ShowAttackParticle(AttackDirection attackDirection);
        
        /// <summary>
        /// Force hide the particles for attack cancels.
        /// </summary>
        public abstract void ForceHideAttackParticles();

        /// <summary>
        /// Shows particles of an attack *hit* on an enemy.
        /// </summary>
        public abstract void ShowAttackHitParticle(Transform hitEntityTransform);
        #endregion

        /// <summary>
        /// Draws some kind of physics shape to detect a list of objects.
        /// </summary>
        public abstract void DetectAttackableObjects(out List<Collider2D> detectedObjects, 
            ContactFilter2D contactFilter2D, 
            Vector2 attackerPosition, AttackDirection attackDirection);
    }
}