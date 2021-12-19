using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class MoveController : MonoBehaviour
{
    [Header("Ray Setup")]
    [SerializeField] const float skinWidth = 0.15f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D playerCollider;
    RayCastOrigins raycastOrigins;
    public CollisionInfo collisions;

    struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }

    private void Start()
    {
        playerCollider = gameObject.GetComponent<BoxCollider2D>();
        CalculateRaySpacing();

        //used for getting corners of box collider

    }


    void UpdateRaycastOrigins()
    {
        Bounds bounds = playerCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(skinWidth * -2);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    void CalculateRaySpacing()
    {
        Bounds bounds = playerCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(skinWidth * -2);
        //limits the minimum number of rays to 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        //gets the direction and values of the y velocity
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) +skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            //adding the velocity x means that the ray is cast from the point where the object will be once it has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin,Vector2.up*directionY*rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;

                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }
        }
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        //gets the direction and values of the y velocity
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            //adding the velocity x means that the ray is cast from the point where the object will be once it has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }


    public void Move(Vector3 velocity)
    {
        //Debug.Log(velocity.ToString());
        UpdateRaycastOrigins();
        collisions.Reset();
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);
    }
}
