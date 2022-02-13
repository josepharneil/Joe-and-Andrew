using Entity;
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
}