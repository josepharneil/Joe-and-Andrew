using System;
using UnityEngine;

namespace AI
{
    [Serializable] public abstract class Sight
    {
        [SerializeField] protected Transform _eyes;
        [NonSerialized] protected Transform Target;
        [SerializeField] protected float _sightRange = 10;

        public void Setup(Transform eyes, Transform target)
        {
            Debug.Assert(eyes, "Eyes must not be null.");
            Debug.Assert(target, "Target must not be null.");
            Target = target;
            if (_eyes == null)
            {
                _eyes = eyes;
            }
        }

        protected virtual bool IsValid()
        {
            bool valid = true;
            if (!Target)
            {
                Debug.LogError("Target is null");
                valid = false;
            }
            if (!_eyes)
            {
                Debug.LogError("Eyes is null");
                valid = false;
            }
            if (_sightRange <= 0)
            {
                Debug.LogError("Sight range is 0 or less");
                valid = false;
            }
            return valid;
        }
        public abstract bool CanSeeTarget();
        public abstract void DrawGizmos();
    }

    [Serializable] public class SightDistanceCheck : Sight
    {
        public override bool CanSeeTarget()
        {
            if (!IsValid())
            {
                return false;
            }
            return ((Vector2)_eyes.position).DistanceToSquared(Target.position) < _sightRange * _sightRange;
        }

        public override void DrawGizmos()
        {
            if (!IsValid())
            {
                return;
            }
            Gizmos.color = CanSeeTarget() ? Color.green : Color.white;
            
            var eyesPosition = _eyes.position;
            Gizmos.DrawWireSphere(eyesPosition, _sightRange);
            Gizmos.DrawRay(eyesPosition, (Target.position - eyesPosition).normalized * _sightRange);
            
            Gizmos.color = Color.white;
        }
    }


    [Serializable] public class SightRaycast : Sight
    {
        [SerializeField] protected LayerMask _blockingLayer;
        
        protected override bool IsValid()
        {
            bool isValid = base.IsValid();
            if (_blockingLayer <= 0)
            {
                Debug.LogError("Blocking layer is less than or 0");
                isValid = false;
            }
            return isValid;
        }

        public override bool CanSeeTarget()
        {
            if (!IsValid())
            {
                return false;
            }
            
            CastRayToTarget(out RaycastHit2D hit);
            
            return hit && (hit.transform.gameObject.layer == Target.gameObject.layer);
        }

        private void CastRayToTarget(out RaycastHit2D hit)
        {
            int targetLayer = Target.gameObject.layer;
            Vector2 eyesPosition = _eyes.position;
            Vector2 targetPosition = Target.position;
            
            hit = Physics2D.Raycast(eyesPosition, 
                eyesPosition.DirectionTo(targetPosition), _sightRange, 
                _blockingLayer.value | 1 << targetLayer);
        }

        public override void DrawGizmos()
        {
            if (!IsValid())
            {
                return;
            }
            
            CastRayToTarget(out RaycastHit2D hit);
            // no hit: white
            if (!hit)
            {
                Gizmos.color = Color.white;
            }
            // hit the target: green
            else if (hit.transform.gameObject.layer == Target.gameObject.layer)
            {
                Gizmos.color = Color.green;
            }
            // Hit that that isn't the target, and target is in range: red
            else if(_eyes.position.DistanceToSquared(Target.position) < _sightRange * _sightRange)
            {
                Gizmos.color = Color.red;
            }
            
            var eyesPosition = _eyes.position;
            Gizmos.DrawWireSphere(eyesPosition, _sightRange);
            Gizmos.DrawRay(eyesPosition, eyesPosition.DirectionTo(Target.position).normalized * _sightRange);
            
            Gizmos.color = Color.white;
        }
    }
    
}