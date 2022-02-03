using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class EnemyChaseTarget : MonoBehaviour
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private Transform chaseTarget;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [UsedImplicitly] public void UpdateChaseTarget()
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