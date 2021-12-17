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

    struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    private void Start()
    {
        playerCollider = gameObject.GetComponent<BoxCollider2D>();
        CalculateRaySpacing();

        //used for getting corners of box collider

    }

    private void Update()
    {
        
        
      
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
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                //this stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
            }
        }
    }

    
    public void Move(Vector3 velocity)
    {
        Debug.Log(velocity.ToString());
        UpdateRaycastOrigins();
        VerticalCollisions(ref velocity);
        transform.Translate(velocity);
    }
}
