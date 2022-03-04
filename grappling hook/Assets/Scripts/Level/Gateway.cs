using level;
using UnityEngine;

namespace Level
{
    public class Gateway : MonoBehaviour
    {
        // Dictates the gateway shape / size.
        // [SerializeField] private BoxCollider2D _boxCollider2D;
        
        // Exit position is set by the level manager.
        private Transform _exitPosition;
        public bool _isLocked { get; set; } = false;

        private void Awake()
        {
            if (gameObject.GetComponent<GatewayLock>()!=null)
            {
                _isLocked = true;
            }
        }

        public void SetExitPosition(Transform exit)
        {
            _exitPosition = exit;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TryEnterGateway(other.transform);
            }
        }

        public bool TryEnterGateway(Transform playerPosition)
        {
            bool entered = false;
            
            // Check bounds
            bool withinBounds = true;//_boxCollider2D.bounds.Contains(playerPosition.position);
            if (withinBounds && !_isLocked)
            {
                // Teleport player simply by setting the position.
                playerPosition.position = _exitPosition.position;
                entered = true;
            }

            return entered;
        }
    }
}