using System;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Used on the actual fireball prefabs.
    /// </summary>
    public class PiranhaFireball : Projectile
    {
        private Vector2 _normalizedDirection;

        public void Initialise(LayerMask targetLayerMask, Vector2 normalizedDirection)
        {
            base.Initialise(targetLayerMask);
            Debug.Assert(normalizedDirection == normalizedDirection.normalized, "This should be normalized", this);
            _normalizedDirection = normalizedDirection;
        }

        public override void UpdatePath(float speed)
        {
            transform.Translate(_normalizedDirection * speed * Time.deltaTime);
        }
    }
    
    public abstract class Projectile : MonoBehaviour
    {
        [NonSerialized] private LayerMask _targetLayerMask;
        [NonSerialized] private bool _hasHitTarget = false;
        [NonSerialized] public float LifespanTimer = 0f;

        public bool HasHitTarget()
        {
            return _hasHitTarget;
        }
        
        protected void Initialise(LayerMask targetLayerMask)
        {
            Debug.Assert(targetLayerMask.value != 0, "This won't hit anything", this);
            _targetLayerMask = targetLayerMask;
        }

        /// <summary>
        /// Updates the path of this projectile.
        /// </summary>
        /// <param name="speed"></param>
        public abstract void UpdatePath(float speed);

        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject.layer == _targetLayerMask)
            {
                _hasHitTarget = true;
            }
        }
    }
}