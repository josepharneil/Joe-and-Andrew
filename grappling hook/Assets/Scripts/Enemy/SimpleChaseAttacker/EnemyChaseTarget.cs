using Entity;
using UnityEngine;
using Utilities;

namespace Enemy
{
    public class EnemyChaseTarget : MonoBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private Transform chaseTarget;
        [SerializeField] private float chaseSpeed = 5f;
        
        public void UpdateChaseTarget()
        {
            if (transform.position.IsLeftOf(chaseTarget.position))
            {
                movementController.MoveAtSpeed(new Vector2(chaseSpeed,Physics2D.gravity.y));
            }
            if (transform.position.IsRightOf(chaseTarget.position))
            {
                movementController.MoveAtSpeed(new Vector2(-chaseSpeed,Physics2D.gravity.y));
            }
        }

    }
}