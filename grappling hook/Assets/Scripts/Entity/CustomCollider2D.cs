using System;
using UnityEngine;

namespace Physics
{
    // ===================================================================
    // Raycast Origins
    // ===================================================================
    public class CustomCollider2D : MonoBehaviour
    {
        public bool CollisionAbove { get; private set; }
        public bool CollisionBelow { get; private set; }
        public bool CollisionLeft { get; private set; }
        public bool CollisionRight { get; private set; }
        
        private Vector2 _topLeft, _topRight;
        private Vector2 _bottomLeft, _bottomRight;
        [Header("Setup")]
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private LayerMask collisionMask;

        [Header("Debug")] 
        [SerializeField] private bool debugDraw;

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
            _bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _topLeft = new Vector2(bounds.min.x, bounds.max.y);
            _bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _topRight = new Vector2(bounds.max.x, bounds.max.y);
        }
        
        /// <summary>
        /// Checks collisions to the left and right.
        /// </summary>
        /// <param name="displacement"></param>
        public void CheckHorizontalCollisions(ref Vector2 displacement)
        {
            //gets the direction and values of the x displacement, this is effectively the move direction
            int directionX = (int)Mathf.Sign(displacement.x);
            //ensures the ray length is fired properly with the skin width
            float rayLength = Mathf.Abs(displacement.x) + SkinWidth;
            //fires each of the rays
            for (int rayIndex = 0; rayIndex < _horizontalRayCount; rayIndex++)
            {
                //checks which way the ray should be fired, based on the displacement direction
                Vector2 rayOrigin = (directionX == -1) ? _bottomLeft : _bottomRight;
                //moves the ray origin along for each ray
                rayOrigin += Vector2.up * (_horizontalRaySpacing * rayIndex);
                //adding the displacement x means that the ray is cast from the point where the object will be once it has moved
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
                }

                if (hit)
                {
                    //this is where the real collision detection and movement comes from
                    //it changes the displacement so that it will only be as far as the box can move before hitting a collider
                    displacement.x = (hit.distance - SkinWidth) * directionX;
                    //this stops the rays from hitting something that is further away than the closest collision
                    rayLength = hit.distance;
                    CollisionLeft = directionX == -1;
                    CollisionRight = directionX == 1;
                }
            }
        }

        /// <summary>
        /// Checks collisions above and below.
        /// </summary>
        /// <param name="displacement"></param>
        public void CheckVerticalCollisions(ref Vector2 displacement)
        {
            //gets the direction and values of the y displacement
            int directionY = (int)Mathf.Sign(displacement.y);
            float rayLength = Mathf.Abs(displacement.y) + SkinWidth;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = (directionY == -1) ? _bottomLeft : _topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex + displacement.x);
                //adding the displacement x means that the ray is cast from the point where the object will be after the x displacement
                //do this because we call the y displacement after x
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin,Vector2.up * (directionY * rayLength), Color.red);
                }

                if (hit)
                {
                    //same as horizontal collisions, this sets the amount of displacement to be the max
                    //that we can get to without a collision
                    displacement.y = (hit.distance - SkinWidth) * directionY;
                    //this stops the rays from hitting something that is further away than the closest collision
                    rayLength = hit.distance;
                    //checks whether the collision is above or below (or both)
                    CollisionAbove = directionY == 1;
                    CollisionBelow = directionY == -1;
                }
            }
        }
        
        public bool CheckIfGrounded()
        {
            //works the same as the other two
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            bool grounded = false;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin =_bottomLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down , rayLength, collisionMask);
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
                }
                if (hit)
                { 
                    grounded = true;
                }
            }
            return grounded;
        }
    }
}