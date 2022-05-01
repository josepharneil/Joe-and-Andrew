using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class EntityParry : MonoBehaviour
    {
        // TODO Move this to player prob... or refactor to be more general...
        [SerializeField] private Player.PlayerController _playerController;
        [SerializeField] private LayerMask whatIsParryable;
        [SerializeField] private float parryRadius = 1f;
        [SerializeField] private float timeBetweenParries = 0.8f;
        private bool _canParry = true;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _showGizmos = false; 
#endif
        
        private float _parryTimer = 0f;

        private void Update()
        {
            if (!_canParry)
            {
                _parryTimer -= Time.deltaTime;
                if (_parryTimer <= 0.0f)
                {
                    _canParry = true;
                }
            }
        }

        public void CheckParry()
        {
            if (!_canParry)
            {
                return;
            }

            Debug.Log("Parry!");
            
            FacingDirection parryDirection = _playerController.PlayerMovement.FacingDirection;
            
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

            _canParry = false;
            _parryTimer = timeBetweenParries;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_showGizmos) return;
            if (_playerController.PlayerMovement.FacingDirection == FacingDirection.Left)
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
#endif
    }
}

