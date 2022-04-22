using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity
{
    public class EntityHitData
    {
        // Health data
        public bool DealsDamage = true;
        public int DamageToHealth = 0;
        // Knockback
        public bool DealsKnockback = false;
        public float KnockbackStrength = 0;
        public Vector2 KnockbackOrigin = default;
        // Haze
        public bool DealsDaze = false;
    }
    
    /// <summary>
    /// Handles all hits to this entity, for example Damage, Knockback, Daze etc.
    /// Sits in between attacks and defense.
    /// </summary>
    public class EntityHitbox : MonoBehaviour
    {
        [SerializeField] private EntityHealth entityHealth;
        [SerializeField] private EntityBlock entityBlock;
        [SerializeField] private EntityKnockback entityKnockback;
        [SerializeField] private EntityDaze entityDaze;

        private bool _isEnabled = true;
        private bool _isHittable = true;
        [SerializeField] private float hitIFrameDuration = 0f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private EntityController entityController;

        [Header("Debug")]
        [SerializeField] private bool _isImmortal = false;

        private void Awake()
        {
            Debug.Assert(entityController, "Must have an entity controller to kill this enemy.", this);
        }

        private void OnEnable()
        {
            entityController.OnEnemyDead += OnEnemyDead;
        }

        private void OnDisable()
        {
            entityController.OnEnemyDead -= OnEnemyDead;
        }

        private void OnEnemyDead()
        {
            _isEnabled = false;
        }

        public bool Kill(EntityHitData hitData)
        {
            if (!_isHittable || !_isEnabled)
            {
                return false;
            }

            if (entityHealth && hitData.DealsDamage)
            {
                entityHealth.Damage(entityHealth.GetMaxHealth());
            }
            
            if (entityKnockback && hitData.DealsKnockback && !entityKnockback.IsBeingKnockedBack())
            {
                entityKnockback.StartKnockBack(hitData.KnockbackOrigin, hitData.KnockbackStrength);
            }

            return true;
        }
        
        public bool Hit(EntityHitData hitData)
        {
            if (!_isHittable || !_isEnabled || _isImmortal)
            {
                return false;
            }
            
            if (entityHealth && hitData.DealsDamage)
            {
                int actualDamageDealt = hitData.DamageToHealth;
                if (entityBlock && entityBlock.IsBlocking())
                {
                    entityBlock.ReduceDamage(ref actualDamageDealt);
                    entityBlock.BreakBlock();
                }
                entityHealth.Damage(actualDamageDealt);
            }

            if (entityKnockback && hitData.DealsKnockback && !entityKnockback.IsBeingKnockedBack())
            {
                entityKnockback.StartKnockBack(hitData.KnockbackOrigin, hitData.KnockbackStrength);
            }

            if (entityDaze && hitData.DealsDaze)
            {
                entityDaze.Daze();
            }

            ActivateIFrames();
            
            return true;
        }

        private async void ActivateIFrames()
        {
            // Debug
            if (spriteRenderer)
            {
                Color spriteRendererColor = spriteRenderer.color;
                spriteRendererColor.a = 0.5f;
                spriteRenderer.color = spriteRendererColor;
            }
            _isHittable = false;
            await Task.Delay(TimeSpan.FromSeconds(hitIFrameDuration));
            _isHittable = true;
            
            // Debug
            if (spriteRenderer)
            {
                Color spriteRendererColor = spriteRenderer.color;
                spriteRendererColor.a = 1f;
                spriteRenderer.color = spriteRendererColor;
            }
        }
    }
}