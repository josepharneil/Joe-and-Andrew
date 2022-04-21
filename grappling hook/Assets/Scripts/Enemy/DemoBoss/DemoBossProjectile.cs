using Entity;
using UnityEngine;

namespace Enemy
{
    public class DemoBossProjectile : Projectile
    {
        private Vector2 _normalisedDirection;
        
        public void Initialise(LayerMask targetLayerMask, Vector2 normalisedDirection)
        {
            base.Initialise(targetLayerMask);
            Debug.Assert(normalisedDirection == normalisedDirection.normalized, "This should be normalized", this);
            _normalisedDirection = normalisedDirection;
        }
        
        public override void UpdatePath(float speed)
        {
            transform.Translate(_normalisedDirection * speed * Time.deltaTime);
        }
    }
}