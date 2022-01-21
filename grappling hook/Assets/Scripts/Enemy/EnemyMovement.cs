using Physics;
using UnityEngine;

// This is very similar to the player movement, which will let us make more complex enemies in future
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CustomCollider2D))]
public class EnemyMovement : MonoBehaviour
{
    public CustomCollider2D customCollider2D;
    
    public FacingDirection FacingDirection{ get; private set; }

    public void MoveAtSpeed(Vector2 speed)
    {
        Move(speed * Time.deltaTime);
    }
    
    public void Move(Vector2 displacement)
    {
        //sets the origins for all the raycasts
        customCollider2D.UpdateRaycastOrigins();
        //TODO: find a way of applying gravity to the enemies easily

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
        FacingDirection = (FacingDirection)Mathf.Sign(displacement.x);
        transform.Translate(displacement);
    }
}
