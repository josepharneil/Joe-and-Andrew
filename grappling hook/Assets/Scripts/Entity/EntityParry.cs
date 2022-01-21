using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class EntityParry : MonoBehaviour
    {
        [SerializeField] private PlayerInputs inputs;
        [SerializeField] private LayerMask whatIsParryable;
        [SerializeField] private float parryRadius = 1f;
        [SerializeField] private float timeBetweenParries = 0.8f;
        private bool canParry = true;
        
        private float _parryTimer = 0f;

        private void Update()
        {
            if (!canParry)
            {
                _parryTimer -= Time.deltaTime;
                if (_parryTimer <= 0.0f)
                {
                    canParry = true;
                }
            }
        }

        public void CheckParry()
        {
            if (!canParry)
            {
                return;
            }

            Debug.Log("Parry!");
            
            FacingDirection parryDirection = inputs.facingDirection;
            
            Vector2 overlapCirclePosition;
            if (parryDirection == FacingDirection.Left)
            {
                overlapCirclePosition = (Vector2)transform.position + new Vector2(-1, 0 );
            }
            else
            {
                overlapCirclePosition = (Vector2)transform.position + new Vector2(1, 0 );
            }
            ContactFilter2D contactFilter2D = new ContactFilter2D
            {
                layerMask = whatIsParryable,
                useLayerMask = true
            };
            List<Collider2D> detectedObjects = new List<Collider2D>();
            Physics2D.OverlapCircle(overlapCirclePosition, parryRadius, contactFilter2D, detectedObjects);

            foreach (Collider2D coll in detectedObjects)
            {
                EntityParryable entityParryable = coll.gameObject.GetComponent<EntityParryable>();
                if (!entityParryable || !entityParryable.isCurrentlyParryable) continue;
                entityParryable.Parry();
                break;

                // Instantiate a hit particle here if we want
            }

            canParry = false;
            _parryTimer = timeBetweenParries;
        }

        private void OnDrawGizmos()
        {
            if (inputs.facingDirection == FacingDirection.Left)
            {
                Vector3 position = transform.position + new Vector3(-1, 0);
                Gizmos.DrawWireSphere(position, parryRadius);
            }
            else
            {
                Vector3 position = transform.position + new Vector3(1, 0);
                Gizmos.DrawWireSphere(position, parryRadius);
            }
        }
    }
}

