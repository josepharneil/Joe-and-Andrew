using System;
using Entity;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Used on the actual fireball prefabs.
    /// </summary>
    public class PiranhaFireball : Projectile
    {
        [SerializeField] private EntityHealth _entityHealth;
        private Vector2 _normalizedDirection;
        public bool Destroyed { get; private set; } = false;

        public void Initialise(LayerMask targetLayerMask, Vector2 normalizedDirection)
        {
            base.Initialise(targetLayerMask);
            Debug.Assert(normalizedDirection == normalizedDirection.normalized, "This should be normalized", this);
            _normalizedDirection = normalizedDirection;
        }

        private void OnEnable()
        {
            _entityHealth.OnEntityDead += DestroyMe;
        }
        
        private void OnDisable()
        {
            _entityHealth.OnEntityDead += DestroyMe;
        }

        public override void UpdatePath(float speed)
        {
            transform.Translate(_normalizedDirection * speed * Time.deltaTime);
        }
        
        private void DestroyMe()
        {
            Destroyed = true;
        }
    }
}