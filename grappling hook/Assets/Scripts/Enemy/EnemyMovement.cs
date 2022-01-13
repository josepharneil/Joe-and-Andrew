using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyMovement : MonoBehaviour
{
    //this is very similar to the player movement, which will let us make more complex eneimes in future

    [Header("Ray Setup")]
    private const float SkinWidth = 0.15f;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private LayerMask collisionMask;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private BoxCollider2D _enemyCollider;
    private RayCastOrigins _raycastOrigins;
    private CollisionInfo _collisions;
    public FacingDirection FacingDirection{ get; private set; }

    struct RayCastOrigins
    {
        public Vector2 TopLeft, TopRight;
        public Vector2 BottomLeft, BottomRight;
    }

    #region setup
    private struct CollisionInfo
    {
        //stoers the collision info, this gets updated each time we call move
        //won't be using this that much for the basic enemy
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

    private void CalculateRaySpacing()
    {
        //calculates the spacing of the rays, based on how many rays were set at the start
        Bounds bounds = _enemyCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(SkinWidth * -2);
        //limits the minimum number of rays to 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        //calculates the ray spacing
        _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    #endregion

    private void Start()
    {
        _enemyCollider = gameObject.GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        UpdateRaycastOrigins();
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = _enemyCollider.bounds;
        //fires the raycasts from a bit inside the collider
        bounds.Expand(SkinWidth * -2);
        _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    private void VerticalCollisions(ref Vector2 displacement)
    {
        //gets the direction and values of the y displacement
        int directionY = (int)Mathf.Sign(displacement.y);
        float rayLength = Mathf.Abs(displacement.y) + SkinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + displacement.x);
            //adding the displacement x means that the ray is cast from the point where the object will be after the x displacement
            //do this because we call the y displacement after x
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                //same as horizontal collisions, this sets the amount of displacement to be the max
                //that we can get to without a collision
                displacement.y = (hit.distance - SkinWidth) * directionY;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                //checks whether the collision is above or below (or both)
                _collisions.Above = directionY == 1;
                _collisions.Below = directionY == -1;
            }
        }
    }

    private void HorizontalCollisions(ref Vector2 displacement)
    {
        //gets the direction and values of the x displacement, this is effectively the move direction
        int directionX = (int)Mathf.Sign(displacement.x);
        //ensures the ray length is fired properly with the skin width
        float rayLength = Mathf.Abs(displacement.x) + SkinWidth;
        //fires each of the rays
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //checks which way the ray should be fired, based on the displacement direction
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
            //moves the ray origin along for each ray
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
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
                _collisions.Left = directionX == -1;
                _collisions.Right = directionX == 1;
            }
        }
    }

    public void Move(Vector2 displacement)
    {
        //sets the origins for all the raycasts
        UpdateRaycastOrigins();
        //TODO: find a way of applying gravity to the enemies easily

        //only have the set to be a bit more efficient, but we could just call them each frame.
        if (displacement.x != 0)
        {
            _collisions.ResetAll();
            HorizontalCollisions(ref displacement);
        }
        if (displacement.y != 0)
        {
            _collisions.ResetAll();
            VerticalCollisions(ref displacement);
        }
        FacingDirection = (FacingDirection)Mathf.Sign(displacement.x);
        transform.Translate(displacement);
    }
}
