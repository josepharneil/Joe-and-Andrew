using UnityEngine;

namespace Physics
{
    // ===================================================================
    // Raycast Origins
    // ===================================================================
    [RequireComponent(typeof(BoxCollider2D))]
    public class CustomCollider2D : MonoBehaviour
    {
        #region CollisionState

        public bool CollisionAbove{ private set; get; }
        public bool CollisionBelow{ private set; get; }
        public bool CollisionLeft{ private set; get; }
        public bool CollisionRight{ private set; get; }
        private bool _fallThroughPlatform;

        #endregion
        
        private Vector2 _topLeft, _topRight;
        private Vector2 _bottomLeft, _bottomRight;
        [Header("Setup")]
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private LayerMask collisionMask;

        public LayerMask GetCollisionMask()
        {
            return collisionMask;
        }

        [Header("Debug")] 
        [SerializeField] private bool _debugDraw;
        [SerializeField] private bool _debugDrawWallJumpRays;
        [SerializeField] private bool _debugJumpCornerClippingEnabled = true;
        [SerializeField] private bool _debugIgnoreHeadClippingEnabled = true;
        
        private const float SkinWidth = 0.15f;
        private int _horizontalRayCount = 4;
        private int _verticalRayCount = 4;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;

        #region UnityFunctions
        private void Awake()
        {
            Debug.Assert(collisionMask != 0,
                "Collision Mask is 0, this implies we're going to fall through everything and this script is doing nothing.",
                this);
            Debug.Assert(boxCollider != null, "Missing box collider", this);
            Debug.Assert(boxCollider.isTrigger, "This should be a trigger: " + gameObject.name, this);
            gameObject.TryGetComponent(out Rigidbody2D rigidbody2DForTriggers);
            Debug.Assert(rigidbody2DForTriggers != null, "A rigidbody is required for triggers to active.", this);
            Debug.Assert(rigidbody2DForTriggers.simulated, "Rigidbody needs to be simulated for Triggers to activate", this);
        }

        private void Start()
        {
            SetupCollisionRaySpacing();
            UpdateRaycastOrigins();
        }
        #endregion

        //calculates the spacing of the rays, based on how many rays were set at the start
        private void SetupCollisionRaySpacing()
        {
            Bounds bounds = boxCollider.bounds;
            //fires the raycasts from a bit inside the collider
            bounds.Expand(SkinWidth * -2);
            //limits the minimum number of rays to 2
            _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
            _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);
            //calculates the ray spacing
            _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
        }
        
        public void ResetCollisionState()
        {
            CollisionAbove = false;
            CollisionBelow = false;
            CollisionLeft = false;
            CollisionRight = false;
        }
        
        //gets the positions of bounds of the rays, which is used for each move
        public void UpdateRaycastOrigins()
        {
            Bounds bounds = boxCollider.bounds;
            //fires the raycasts from a bit inside the collider
            bounds.Expand(SkinWidth * -2);
            _bottomLeft  = new Vector2(bounds.min.x, bounds.min.y);
            _topLeft     = new Vector2(bounds.min.x, bounds.max.y);
            _bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _topRight    = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CheckHorizontalCollisions(out bool left, out bool right, float overrideSize = SkinWidth)
        {
            right = left = false;
            for (int rayIndex = 0; rayIndex < _horizontalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _bottomLeft + Vector2.up * (_horizontalRaySpacing * rayIndex);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, overrideSize, collisionMask);
#if UNITY_EDITOR
                if (_debugDrawWallJumpRays)
                {
                    Debug.DrawRay(rayOrigin, Vector2.left * overrideSize, Color.red);
                }
#endif
                if (!hit) continue;
                
                left = true;
                break;
            }
            for (int rayIndex = 0; rayIndex < _horizontalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _bottomRight + (Vector2.up * (_horizontalRaySpacing * rayIndex));
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, overrideSize, collisionMask);
#if UNITY_EDITOR
                if (_debugDrawWallJumpRays)
                {
                    Debug.DrawRay(rayOrigin, Vector2.right * overrideSize, Color.red);
                }
#endif
                if (!hit) continue;
                
                right = true;
                break;
            }
        }

        /// <summary>
        /// Checks collisions to the left and right.
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="ignoreBottomHorizontalRays"></param>
        public void CheckHorizontalCollisions(ref Vector2 displacement, bool ignoreBottomHorizontalRays = false)
        {
            //gets the direction and values of the x displacement, this is effectively the move direction
            int directionX = (int)Mathf.Sign(displacement.x);
            //ensures the ray length is fired properly with the skin width
            float rayLength = Mathf.Abs(displacement.x) + SkinWidth;

            int lowerBoundRayIndex = 0;
            if(_debugJumpCornerClippingEnabled && ignoreBottomHorizontalRays)
            {
                lowerBoundRayIndex++;
            }
            Vector2 bottomRightOrLeft = (directionX == -1) ? _bottomLeft : _bottomRight;
            for (int rayIndex = lowerBoundRayIndex; rayIndex < _horizontalRayCount; rayIndex++)
            {
                // Checks which way the ray should be fired, based on the displacement direction
                // Moves the ray origin along for each ray
                Vector2 rayOrigin = bottomRightOrLeft + Vector2.up * (_horizontalRaySpacing * rayIndex);
                // Adding the displacement x means that the ray is cast from the point where the object will be once it has moved
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                DebugDrawCollisionRay(rayOrigin, Vector2.right * (directionX * rayLength));

                if (!hit) continue;
                
                if (hit.collider.CompareTag("OneWayPlatform")) 
                {
                    return;
                }
                // This is where the real collision detection and movement comes from
                // it changes the displacement so that it will only be as far as the box can move before hitting a collider
                displacement.x = (hit.distance - SkinWidth) * directionX;
                // This stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                CollisionLeft = directionX == -1;
                CollisionRight = directionX == 1;
            }
        }

        /// <summary>
        /// Checks collisions above and below.
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="ignoreLeftHeadClips"></param>
        /// <param name="ignoreRightHeadClips"></param>
        public void CheckVerticalCollisions(ref Vector2 displacement, bool ignoreLeftHeadClips = false, bool ignoreRightHeadClips = false)
        {
            //gets the direction and values of the y displacement
            int directionY = (int)Mathf.Sign(displacement.y);
            float rayLength = Mathf.Abs(displacement.y) + SkinWidth;

            int lowerBoundRayIndex = 0;
            int upperBoundRayIndex = _verticalRayCount;
            if (_debugIgnoreHeadClippingEnabled)
            {
                if (ignoreLeftHeadClips)
                {
                    lowerBoundRayIndex++;
                }
                if (ignoreRightHeadClips)
                {
                    upperBoundRayIndex--;
                }
            }

            Vector2 topOrBottomLeft = ((directionY == -1) ? _bottomLeft : _topLeft);
            for (int rayIndex = lowerBoundRayIndex; rayIndex < upperBoundRayIndex; rayIndex++)
            {
                Vector2 rayOrigin = topOrBottomLeft + Vector2.right * (_verticalRaySpacing * rayIndex + displacement.x);
                // Adding the displacement x means that the ray is cast from the point where the object will be after the x displacement
                // Do this because we call the y displacement after x
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                DebugDrawCollisionRay(rayOrigin, Vector2.up * (directionY * rayLength));

                if (!hit) continue;
                
                // Checks if the player is hitting a one way platform, and if the player is jumping up as well
                if(hit.collider.CompareTag("OneWayPlatform") && (displacement.y > 0f || _fallThroughPlatform))
                {
                    return;
                }
                // Same as horizontal collisions, this sets the amount of displacement to be the max
                // That we can get to without a collision
                displacement.y = (hit.distance - SkinWidth) * directionY;
                // This stops the rays from hitting something that is further away than the closest collision
                rayLength = hit.distance;
                // Checks whether the collision is above or below (or both)
                CollisionAbove = directionY == 1;
                CollisionBelow = directionY == -1;
            }
        }
        
        public bool CheckIfGrounded()
        {
            //works the same as the other two
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _bottomLeft + Vector2.right * (_verticalRaySpacing * rayIndex);
                DebugDrawCollisionRay(rayOrigin, Vector2.down * rayLength);
                if (Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask))
                { 
                    return true;
                }
            }
            return false;
        }

        public bool CheckIfHittingCeiling()
        {
            //works the same as the others
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _topLeft + (Vector2.right * (_verticalRaySpacing * rayIndex));
                DebugDrawCollisionRay(rayOrigin, Vector2.up * rayLength);
                if (Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask))
                { 
                    return true;
                }
            }
            return false;
        }
        
        public bool CheckIfOneWayPlatform()
        {
            //Basically the same as the CheckIfGrounded code
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _bottomLeft + (Vector2.right * (_verticalRaySpacing * rayIndex));
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);
                DebugDrawCollisionRay(rayOrigin, Vector2.down * rayLength);
                if (hit && hit.collider.CompareTag("OneWayPlatform"))
                {
                    return true;
                }
            }
            return false;
        }

        public void SetFallThroughPlatform(bool passThrough)
        {
            _fallThroughPlatform = passThrough;
        }

        private void DebugDrawCollisionRay(Vector2 rayOrigin, Vector2 direction)
        {
#if UNITY_EDITOR
            if (_debugDraw)
            {
                Debug.DrawRay(rayOrigin, direction, Color.red);
            }
#endif
        }
    }

}