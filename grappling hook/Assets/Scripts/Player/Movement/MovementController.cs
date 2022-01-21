using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(Physics.CustomCollider2D))]
    public class MovementController : MonoBehaviour
    {
        [Header("Components")]
        public Physics.CustomCollider2D customCollider2D;

        public void MoveAtSpeed(Vector2 velocity)
        {
            Move(velocity * Time.deltaTime);
        }
        
        private void Move(Vector2 displacement)
        {
            //sets the origins for all the raycasts
            customCollider2D.UpdateRaycastOrigins();

            //only have the set to be a bit more efficient, but we could just call them each frame.
            if (displacement.x != 0)
            {
                customCollider2D.ResetCollisionState();
                customCollider2D.CheckHorizontalCollisions(ref displacement);
            }
            if (displacement.y != 0)
            {
                customCollider2D.ResetCollisionState();
                customCollider2D.CheckVerticalCollisions(ref displacement);
            }

            transform.Translate(displacement);
        }
    }
    
    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
}
