using System;
using UnityEngine;

namespace AI
{
    [Serializable] public abstract class Sight
    {
        [NonSerialized] public Transform Eyes;
        [SerializeField] protected Transform _target;
        [SerializeField] protected float _sightRange = 10;
        
        protected virtual bool IsValid()
        {
            bool valid = true;
            if (!_target)
            {
                Debug.LogError("Target is null");
                valid = false;
            }
            if (!Eyes)
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
            return ((Vector2)Eyes.position).DistanceToSquared(_target.position) < _sightRange * _sightRange;
        }

        public override void DrawGizmos()
        {
            if (!IsValid())
            {
                return;
            }
            Gizmos.color = CanSeeTarget() ? Color.green : Color.white;
            
            var eyesPosition = Eyes.position;
            Gizmos.DrawWireSphere(eyesPosition, _sightRange);
            Gizmos.DrawRay(eyesPosition, (_target.position - eyesPosition).normalized * _sightRange);
            
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
            
            return hit && (hit.transform.gameObject.layer == _target.gameObject.layer);
        }

        private void CastRayToTarget(out RaycastHit2D hit)
        {
            int targetLayer = _target.gameObject.layer;
            Vector2 eyesPosition = Eyes.position;
            Vector2 targetPosition = _target.position;
            
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
            else if (hit.transform.gameObject.layer == _target.gameObject.layer)
            {
                Gizmos.color = Color.green;
            }
            // Hit that that isn't the target, and target is in range: red
            else if(Eyes.position.DistanceToSquared(_target.position) < _sightRange * _sightRange)
            {
                Gizmos.color = Color.red;
            }
            
            var eyesPosition = Eyes.position;
            Gizmos.DrawWireSphere(eyesPosition, _sightRange);
            Gizmos.DrawRay(eyesPosition, eyesPosition.DirectionTo(_target.position).normalized * _sightRange);
            
            Gizmos.color = Color.white;
        }
    }
    
}