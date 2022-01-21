using Physics;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MoveController : MonoBehaviour
{
    [Header("Components")]
    public CustomCollider2D customCollider2D;

    public void Move(Vector2 displacement)
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
