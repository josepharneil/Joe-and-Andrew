using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //this is very similar to the player movement, which will let us make more complex eneimes in future

    [Header("Ray Setup")]
    [SerializeField] const float skinWidth = 0.15f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D enemyCollider;
    RayCastOrigins raycastOrigins;
    public CollisionInfo collisions;


    struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    #region setup
    public struct CollisionInfo
    {
        //stoers the collision info, this gets updated each time we call move
        //won't be using this that much for the basic enemy
        public bool above, below;
        public bool left, right;
        public void ResetAll()
        {
            above = below = false;
            left = right = false;
        }
        public void ResetHorizontal()
        {
            left = right = false;
        }

        public void ResetVertical()
        {
            above = below = false;
        }

    }

    void CalculateRaySpacing()
    {
        //claculates the spacing of the rays, based on how many rays were set at the start
        Bounds bounds = enemyCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(skinWidth * -2);
        //limits the minimum number of rays to 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        //calculates the ray spacing
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    #endregion
    void Start()
    {
        enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        UpdateRaycastOrigins();
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = enemyCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(skinWidth * -2);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    void VerticalCollisions(ref Vector2 displacement)
    {
        //gets the direction and values of the y displacement
        float directionY = Mathf.Sign(displacement.y);
        float rayLength = Mathf.Abs(displacement.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + displacement.x);
            //adding the displacement x means that the ray is cast from the point where the object will be after the x displacement
            //do this because we call the y displacement after x
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                //same as horizontal collisions, this sets the amount of displacement to be the max
                //that we can get to without a collision
                displacement.y = (hit.distance - skinWidth) * directionY;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                //checks whether the collision is above or below (or both)
                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }
        }
    }
    void HorizontalCollisions(ref Vector2 displacement)
    {
        //gets the direction and values of the x displacement, this is effectively the move direction
        float directionX = Mathf.Sign(displacement.x);
        //ensures the ray length is fired properly with the skin width
        float rayLength = Mathf.Abs(displacement.x) + skinWidth;
        //fires each of the rays
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //checks which way the ray should be fired, based on the displacement direction
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            //moves the ray origin along for each ray
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            //adding the displacement x means that the ray is cast from the point where the object will be once it has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //this is where the real collision detection and movement comes from
                //it changes the displacement so that it will only be as far as the box can move before hitting a collider
                displacement.x = (hit.distance - skinWidth) * directionX;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    public void Move(Vector2 displacement)
    {
        //sets the origins for all the raycasts
        UpdateRaycastOrigins();
        //TODO: find a  way of applying gravity to the enemies easily

        //only have the set to be a bit more efficient, but we could just call them each frame.
        if (displacement.x != 0)
        {
            collisions.ResetAll();
            HorizontalCollisions(ref displacement);
        }
        if (displacement.y != 0)
        {
            collisions.ResetAll();
            VerticalCollisions(ref displacement);
        }
        transform.Translate(displacement);
    }
}
