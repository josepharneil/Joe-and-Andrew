using UnityEngine;

namespace Entity
{
    public class EntityKnockback : MonoBehaviour
    {
        private Vector2 _knockbackDirection;
        private float _knockbackStrength;
        private bool _isBeingKnockedBack;

        public bool IsBeingKnockedBack()
        {
            return _isBeingKnockedBack;
        }

        [SerializeField] private float knockBackDecrementAmount;
        [SerializeField] private MovementController movementController;

        /// <summary>
        /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
        /// </summary>
        /// <param name="knockbackOrigin"></param>
        /// <param name="knockbackStrength"></param>
        public void StartKnockBack(Vector2 knockbackOrigin, float knockbackStrength)
        {
            _knockbackDirection = (Vector2)transform.position - knockbackOrigin;
            _knockbackDirection.Normalize();
            // _knockbackDirection.y = 0f; // Possibly only horizontal knockback?
            _knockbackStrength = knockbackStrength;
            _isBeingKnockedBack = true;
        }
        
        public void UpdateKnockback()
        {
            if (_knockbackStrength <= 0)
            {
                _isBeingKnockedBack = false;
            }
            movementController.MoveAtSpeed(_knockbackDirection * _knockbackStrength);
            _knockbackStrength -= knockBackDecrementAmount * Time.deltaTime;
        }
    }
}

