using System;
using Physics;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// If other entities touch this entity, damage them.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityDamageOnTouch : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        
        [Header("Components")]
        [SerializeField] private LayerMask damagesWhat;
        
        [Header("Customisation")]
        [SerializeField] private int damage = 5;
        [SerializeField] private float knockbackSpeed = 2f;

        [SerializeField] private CustomCollider2D customCollider2D;

        private void OnEnable()
        {
            customCollider2D.OnCollisionEnter += OnCustomCollisionEnter;
        }

        private void OnDisable()
        {
            customCollider2D.OnCollisionEnter -= OnCustomCollisionEnter;
        }

        private void OnCustomCollisionEnter(GameObject collidedGameObject)
        {
            if (!isEnabled)
            {
                return;
            }
            if ((collidedGameObject.layer & damagesWhat) != 0) return;

            collidedGameObject.TryGetComponent<EntityHealth>(out EntityHealth entityHealth);
            if (entityHealth)
            {
                Debug.Log(damage);
                entityHealth.Damage(damage);
            }

            collidedGameObject.TryGetComponent<EntityKnockback>(out EntityKnockback entityKnockback);
            if (entityKnockback)
            {
                entityKnockback.Knockback(
                    collidedGameObject.transform.position - transform.position, 
                    knockbackSpeed);
            }
        }
    }
}

