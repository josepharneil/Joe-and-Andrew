using UnityEditor;
using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
    public class LookDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetVisible = Look(controller);
            return targetVisible;
        }

        private bool Look(StateController controller)
        {
            ChasePathing chasePathing = controller.gameObject.GetComponent<ChasePathing>();
            int facingDirection = (int)controller.gameObject.GetComponent<EnemyMovement>().facingDirection;
            Debug.DrawRay(controller.gameObject.transform.position , new Vector3(facingDirection * chasePathing.sightRange,0f,0f) );
            RaycastHit2D hit = Physics2D.CircleCast(controller.gameObject.transform.position, chasePathing.sightWidth, new Vector2(facingDirection,0f),chasePathing.sightRange);
            if (hit && hit.collider.CompareTag("Player"))
            {
                return true;
            }
            return false;
        }
    }
}
