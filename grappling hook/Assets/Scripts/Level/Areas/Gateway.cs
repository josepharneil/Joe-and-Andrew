using Ludiq;
using UnityEngine;

namespace Level
{
    public class Gateway : MonoBehaviour
    {
        // Dictates the gateway shape / size.
        // [SerializeField] private BoxCollider2D _boxCollider2D;
        
        // Exit position is set by the level manager.
        private Transform _exitPosition;
        [DoNotSerialize] public bool IsLocked { get; set; } = false;

        private void Awake()
        {
            if (gameObject.GetComponent<GatewayLock>() != null)
            {
                IsLocked = true;
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

        private bool TryEnterGateway(Transform playerPosition)
        {
            bool entered = false;
            
            // Check bounds
            bool withinBounds = true;//_boxCollider2D.bounds.Contains(playerPosition.position);
            if (withinBounds && !IsLocked)
            {
                // Teleport player simply by setting the position.
                playerPosition.position = _exitPosition.position;
                entered = true;
            }

            return entered;
        }
    }
}