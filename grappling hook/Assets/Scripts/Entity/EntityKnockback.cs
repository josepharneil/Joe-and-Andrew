using Physics;
using UnityEngine;
using UnityEngine.Events;

namespace Entity
{
    public class EntityKnockback : MonoBehaviour
    {
        [SerializeField] private float decelerationRate;
        [SerializeField] private MovementController movementController;
        [SerializeField] private AnimationCurve decelerationCurve;

        private Vector2 _knockbackDirection;
        private float _knockbackStrength;
        private bool _isBeingKnockedBack;
        private float _lerpCurrent;


        public bool IsBeingKnockedBack()
        {
            return _isBeingKnockedBack;
        }


        public void Update()
        {
            if (_isBeingKnockedBack)
            {
                UpdateKnockback();
            }
        }

        /// <summary>
        /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
        /// Call StartKnockBack(origin, strengh) to knocback an entity
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
            _lerpCurrent = 0;
        }
        
        public void UpdateKnockback()
        {
            if (_knockbackStrength <= 0.05f)
            {
                _isBeingKnockedBack = false;
                return;
            }
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, decelerationRate*Time.deltaTime);
            _knockbackStrength = Mathf.Lerp(_knockbackStrength, 0f, decelerationCurve.Evaluate(_lerpCurrent));
            movementController.MoveAtSpeed(_knockbackDirection * _knockbackStrength);
        }
    }
}

