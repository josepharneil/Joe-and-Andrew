using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
    public class LookDecision : Decision
    {
        private ChasePathing _chasePathing;
        private EnemyMovement _enemyMovement;
        
        public override bool Decide(StateController controller)
        {
            return Look(controller);
        }

        private bool Look(StateController controller)
        {
            if (_chasePathing == null)
            {
                _chasePathing = controller.gameObject.GetComponent<ChasePathing>();
                Debug.Assert(_chasePathing != null);
            }
            if (_enemyMovement == null)
            {
                _enemyMovement = controller.gameObject.GetComponent<EnemyMovement>();
                Debug.Assert(_chasePathing != null);
            }
            
            int facingDirection = (int)_enemyMovement.facingDirection;
            var gameObjectPosition = controller.gameObject.transform.position;
            LayerMask mask = _chasePathing.mask;
            RaycastHit2D hit = new RaycastHit2D();
            for (int i = 0; i < _chasePathing.sightHeight; i++)
            {
                Vector3 heightVector = new Vector3(0f, 0.5f*i, 0f);
                Debug.DrawRay(gameObjectPosition+heightVector, new Vector3(facingDirection * _chasePathing.sightRange, 0f, 0f));
                hit = Physics2D.CircleCast(gameObjectPosition+heightVector, _chasePathing.sightWidth, new Vector2(facingDirection, 0f), _chasePathing.sightRange, mask);
                if (hit)
                {
                    break;
                }
            }
            
            return hit && hit.collider.CompareTag("Player");
        }
    }
}
