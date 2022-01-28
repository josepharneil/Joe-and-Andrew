using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyCheckCanSeeTarget : MonoBehaviour
    {
        [SerializeField] private float maxSightDistance = 5f;
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask groundLayer;

        public bool CheckCanSeeTarget()
        {
            Vector2 thisPosition = (Vector2)transform.position;
            Vector2 targetPosition = target.position;

            return (targetPosition - thisPosition).sqrMagnitude < maxSightDistance * maxSightDistance;
        }

        private bool RaycastChecker()
        {
            Debug.Assert(target != null, "Target shouldn't be null.", this);
            if (target == null)
            {
                return false;
            }

            ContactFilter2D contactFilter2D = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = targetLayer | groundLayer,
                useTriggers = true
            };
            var thisPosition = transform.position;
            List<RaycastHit2D> raycastHits = new List<RaycastHit2D>
            {
                Capacity = 8
            };
            int numHits = Physics2D.Raycast(thisPosition, target.position - thisPosition,
                contactFilter2D, raycastHits, maxSightDistance);
            if (numHits == 0)
            {
                return false;
            }
            print(raycastHits[0].transform.gameObject.layer + targetLayer);
            // If the very first raycast hit is the target, return true!
            return raycastHits[0].transform.gameObject.layer == targetLayer;

        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;

            var thisPosition = transform.position;
            var targetPosition = target.position;
            Gizmos.DrawRay(thisPosition, (targetPosition - thisPosition).normalized * maxSightDistance);
        }
    }
}