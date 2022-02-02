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
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void UpdateChaseTarget()
        {
            if (transform.position.IsLeftOf(chaseTarget.position))
            {
                movementController.Move(new Vector2(chaseSpeed,Physics2D.gravity.y));
                spriteRenderer.flipX = false;
            }
            if (transform.position.IsRightOf(chaseTarget.position))
            {
                movementController.Move(new Vector2(-chaseSpeed,Physics2D.gravity.y));
                spriteRenderer.flipX = true;
            }
        }
    }
}