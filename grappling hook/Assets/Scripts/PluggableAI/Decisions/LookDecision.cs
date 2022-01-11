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

            Debug.DrawRay(gameObjectPosition , new Vector3(facingDirection * _chasePathing.sightRange,0f,0f) );
            RaycastHit2D hit = Physics2D.Raycast(gameObjectPosition, new Vector2(facingDirection, 0f), _chasePathing.sightRange,mask);
            //Physics2D.CircleCast(gameObjectPosition, _chasePathing.sightWidth, new Vector2(facingDirection,0f),_chasePathing.sightRange);
            
            return hit && hit.collider.CompareTag("Player");
        }
    }
}
