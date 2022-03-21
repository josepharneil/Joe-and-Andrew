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
        private bool _collisionAbove;
        private bool _collisionBelow;
        private bool _collisionLeft;
        private bool _collisionRight;
        private bool _fallThroughPlatform = false;

        private void SetCollisionAbove(bool value, GameObject collidedGameObject)
        {
            bool initialState = _collisionAbove;
            _collisionAbove = value;
            MaybeTriggerOnCollisionEnter(initialState, value, collidedGameObject);
        }
        public bool GetCollisionAbove()
        {
            return _collisionAbove;
        }
        private void SetCollisionBelow(bool value, GameObject collidedGameObject)
        {
            bool initialState = _collisionBelow;
            _collisionBelow = value;
            MaybeTriggerOnCollisionEnter(initialState, value, collidedGameObject);
        }
        public bool GetCollisionBelow()
        {
            return _collisionBelow;
        }
        private void SetCollisionLeft(bool value, GameObject collidedGameObject)
        {
            bool initialState = _collisionLeft;
            _collisionLeft = value;
            MaybeTriggerOnCollisionEnter(initialState, value, collidedGameObject);
        }
        public bool GetCollisionLeft()
        {
            return _collisionLeft;
        }
        private void SetCollisionRight(bool value, GameObject collidedGameObject)
        {
            bool initialState = _collisionRight;
            _collisionRight = value;
            MaybeTriggerOnCollisionEnter(initialState, value, collidedGameObject);
        }
        public bool GetCollisionRight()
        {
            return _collisionRight;
        }

        private void MaybeTriggerOnCollisionEnter(bool initialState, bool newState, GameObject collidedGameObject)
        {
            if (initialState == false && newState == true)
            {
                // OnCollisionEnter?.Invoke(collidedGameObject);
            }
        }

        // private void OnTriggerEnter2D(Collider2D col)
        // {
        //     OnTriggerEnter?.Invoke(col);
        // }
        // private void OnTriggerEnter2D(Collision2D col)
        // {
        //     // todo, not sure if this should be an event to be honest
        //     OnCollisionEnter?.Invoke(col);
        // }

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
            _collisionAbove = false;
            _collisionBelow = false;
            _collisionLeft = false;
            _collisionRight = false;
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
                    //checks if the player is going to bonk into a one way platfom
                    if (hit.collider.CompareTag("OneWayPlatform"))
                    {
                        return;
                    }
                    //this is where the real collision detection and movement comes from
                    //it changes the displacement so that it will only be as far as the box can move before hitting a collider
                    displacement.x = (hit.distance - SkinWidth) * directionX;
                    //this stops the rays from hitting something that is further away than the closest collision
                    rayLength = hit.distance;
                    GameObject hitGameObject = hit.transform.gameObject;
                    SetCollisionLeft(directionX == -1, hitGameObject);
                    SetCollisionRight(directionX == 1, hitGameObject);
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
                    //checks if the player is hitting a one way platform, and if the player is jumping up as well
                    if(hit.collider.CompareTag("OneWayPlatform") && (displacement.y > 0f || _fallThroughPlatform))
                    {
                        return;
                    }
                    //same as horizontal collisions, this sets the amount of displacement to be the max
                    //that we can get to without a collision
                    displacement.y = (hit.distance - SkinWidth) * directionY;
                    //this stops the rays from hitting something that is further away than the closest collision
                    rayLength = hit.distance;
                    //checks whether the collision is above or below (or both)
                    GameObject hitGameObject = hit.transform.gameObject;
                    SetCollisionAbove(directionY == 1, hitGameObject);
                    SetCollisionBelow(directionY == -1, hitGameObject);
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
                Vector2 rayOrigin = _bottomLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex);
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
                }
                if (Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask))
                { 
                    grounded = true;
                }
            }
            return grounded;
        }

        public bool CheckIfHittingCeiling()
        {
            //works the same as the others
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            bool hittingCeiling = false;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _topLeft + (Vector2.right * (_verticalRaySpacing * rayIndex));
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);
                }
                if (Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask))
                { 
                    hittingCeiling = true;
                }
            }
            return hittingCeiling;
        }
        
        public bool CheckIfOneWayPlatform()
        {
            //Basically the same as the CheckIfGrounded code
            UpdateRaycastOrigins();
            const float rayLength = SkinWidth + 0.01f;
            bool oneWay = false;
            for (int rayIndex = 0; rayIndex < _verticalRayCount; rayIndex++)
            {
                Vector2 rayOrigin = _bottomLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * rayIndex);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);
                if (debugDraw)
                {
                    Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);
                }
                if (hit && hit.collider.CompareTag("OneWayPlatform"))
                {
                    oneWay = true;
                }
            }
            return oneWay;
        }

        public void SetFallThroughPlatform(bool passThrough)
        {
            _fallThroughPlatform = passThrough;
        }
    }

}