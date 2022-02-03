using UnityEngine;

namespace Enemy
{
    public class PiranhaFireball : MonoBehaviour
    {
        public LayerMask targetLayerMask;
        public bool hasHitPlayer = false;
        public float lifespanTimer = 0f;
        public Vector2 direction = Vector2.zero;
        
        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject.layer == targetLayerMask)
            {
                hasHitPlayer = true;
            }
        }
    }
}