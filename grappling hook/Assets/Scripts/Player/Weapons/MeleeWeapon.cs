using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Player
{
    public abstract class MeleeWeapon : MonoBehaviour
    {
        /// <summary>
        /// Damage that this weapon deals.
        /// </summary>
        public int WeaponDamage;
        
        /// <summary>
        /// Amount that this weapon knocks back the player.
        /// </summary>
        [SerializeField] protected float _knockbackAmountToPlayer;
        
        /// <summary>
        /// Amount that this weapon knocks back the target hit.
        /// </summary>
        public float KnockbackAmountToTarget;
        
        /// <summary>
        /// Draws gizmos.
        /// </summary>
        /// <param name="facingDirection"></param>
        public abstract void DrawGizmos(FacingDirection facingDirection);

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
        public abstract void ShowAttackHitParticle();
        #endregion

        /// <summary>
        /// Draws some kind of physics shape to detect a list of objects.
        /// </summary>
        public abstract void DetectAttackableObjects(out List<Collider2D> detectedObjects, ContactFilter2D contactFilter2D, AttackDirection attackDirection);
        
        /// <summary>
        /// Knockback on the player. Depends on the weapon, might be a really heavy weapon.
        /// </summary>
        public abstract void KnockbackPlayer(Vector2 firstEnemyHitPosition);
    }
}