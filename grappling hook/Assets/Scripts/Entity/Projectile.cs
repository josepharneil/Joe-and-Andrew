using System;
using UnityEngine;

namespace Entity
{
    public abstract class Projectile : MonoBehaviour
    {
        [NonSerialized] private LayerMask _targetLayerMask;
        [NonSerialized] public float LifespanTimer = 0f;
        public bool HasHitTarget { get; private set; } = false;

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
                HasHitTarget = true;
            }
        }
    }
}