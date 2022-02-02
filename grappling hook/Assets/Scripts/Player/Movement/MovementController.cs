using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(Physics.CustomCollider2D))]
    public class MovementController : MonoBehaviour
    {
        [Header("Components")]
        public Physics.CustomCollider2D customCollider2D;

        public void Move(Vector2 velocity)
        {
            Translate(velocity * Time.deltaTime);
        }
        
        private void Translate(Vector2 displacement)
        {
            //sets the origins for all the raycasts
            customCollider2D.UpdateRaycastOrigins();

            // Always update collisions, even if 0 displacement.
            // Found a problem where if we didn't, and we were on something that moved, once if moved away
            // the player didn't fall downwards.
            customCollider2D.ResetCollisionState();
            customCollider2D.CheckHorizontalCollisions(ref displacement);
            customCollider2D.ResetCollisionState();
            customCollider2D.CheckVerticalCollisions(ref displacement);

            transform.Translate(displacement);
        }
    }
    
    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
}
