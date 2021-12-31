using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class MoveController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider2D playerCollider;
    
    [Header("Ray Setup")]
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;
    
    private const float SkinWidth = 0.15f;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private RayCastOrigins _raycastOrigins;
    public CollisionInfo Collisions;

    // ===================================================================
    // Raycast Origins
    // ===================================================================
    private struct RayCastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BottomLeft, BottomRight;
    }

    // ===================================================================
    // Stores the collision info, this gets updated each time we call move
    // ===================================================================
    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Left, Right;
        public void ResetAll()
        {
            Above = Below = false;
            Left = Right = false;
        }
        public void ResetHorizontal()
        {
            Left = Right = false;
        }

        public void ResetVertical()
        {
            Above = Below = false;
        }
    }

    private void Start()
    {
        CalculateRaySpacing();
    }

    //gets the positions of bounds of the rays, which is used for each move
    void UpdateRaycastOrigins()
    {
        Bounds bounds = playerCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(SkinWidth * -2);
        _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    private void CalculateRaySpacing()
    {
        // calculates the spacing of the rays, based on how many rays were set at the start
        Bounds bounds = playerCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(SkinWidth * -2);
        //limits the minimum number of rays to 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        //calculates the ray spacing
        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private void VerticalCollisions(ref Vector3 displacement)
    {
        //gets the direction and values of the y displacement
        int directionY = (int)Mathf.Sign(displacement.y);
        float rayLength = Mathf.Abs(displacement.y) +SkinWidth;
        for (int rayIndex = 0; rayIndex < verticalRayCount; rayIndex++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex + displacement.x);
            //adding the displacement x means that the ray is cast from the point where the object will be after the x displacement
            //do this because we call the y displacement after x
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin,Vector2.up * (directionY * rayLength), Color.red);

            if (hit)
            {
                //same as horizontal collisions, this sets the amount of displacement to be the max
                //that we can get to without a collision
                displacement.y = (hit.distance - SkinWidth) * directionY;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                //checks whether the collision is above or below (or both)
                Collisions.Above = directionY == 1;
                Collisions.Below = directionY == -1;
            }
        }
    }

    private void HorizontalCollisions(ref Vector3 displacement)
    {
        //gets the direction and values of the x displacement, this is effectively the move direction
        int directionX = (int)Mathf.Sign(displacement.x);
        //ensures the ray length is fired properly with the skin width
        float rayLength = Mathf.Abs(displacement.x) + SkinWidth;
        //fires each of the rays
        for (int rayIndex = 0; rayIndex < horizontalRayCount; rayIndex++)
        {
            //checks which way the ray should be fired, based on the displacement direction
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
            //moves the ray origin along for each ray
            rayOrigin += Vector2.up * (_horizontalRaySpacing * rayIndex);
            //adding the displacement x means that the ray is cast from the point where the object will be once it has moved
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //this is where the real collision detection and movement comes from
                //it changes the displacement so that it will only be as far as the box can move before hitting a collider
                displacement.x = (hit.distance - SkinWidth) * directionX;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                Collisions.Left = directionX == -1;
                Collisions.Right = directionX == 1;
            }
        }
    }


    public void Move(Vector3 displacement)
    {
        //sets the origins for all the raycasts
        UpdateRaycastOrigins();

        //only have the set to be a bit more efficient, but we could just call them each frame.
        if (displacement.x != 0)
        {
            Collisions.ResetAll();
            HorizontalCollisions(ref displacement);
        }
        if (displacement.y != 0)
        {
            Collisions.ResetAll();
            VerticalCollisions(ref displacement);
        }
        transform.Translate(displacement);
    }

    public bool CheckGrounded()
    {
        //works the same as the other two
        UpdateRaycastOrigins();
        const float rayLength = SkinWidth + 0.01f;
        bool grounded = false;
        for (int rayIndex = 0; rayIndex < verticalRayCount; rayIndex++)
        {
            Vector2 rayOrigin =_raycastOrigins.BottomLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down , rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

            if (hit)
            { 
                grounded = true;
            }
        }
        return grounded;
    }
}
