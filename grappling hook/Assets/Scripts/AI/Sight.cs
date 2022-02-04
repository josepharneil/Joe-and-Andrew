using System;
using UnityEngine;

namespace AI
{
    [Serializable] public abstract class Sight
    {
        [SerializeField] protected Transform _eyes;
        [SerializeField] protected Transform _target;
        [SerializeField] protected float _sightRange;

        protected virtual bool IsValid()
        {
            bool valid = true;
            if (!_target)
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
            return ((Vector2)_eyes.position).DistanceToSquared(_target.position) < _sightRange * _sightRange;
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
            
            int targetLayer = 1 << _target.gameObject.layer;
            Vector2 eyesPosition = _eyes.position;
            Vector2 targetPosition = _target.position;
            
            RaycastHit2D hit = Physics2D.Raycast(eyesPosition, 
                targetPosition - eyesPosition, _sightRange, 
                _blockingLayer.value | 1 << targetLayer);

            return hit && hit.transform.gameObject.layer == targetLayer;
        }

        public override void DrawGizmos()
        {
            if (!IsValid())
            {
                return;
            }
            
            var eyesPosition = _eyes.position;
            if (CanSeeTarget())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawWireSphere(eyesPosition, _sightRange);
            Gizmos.DrawRay(eyesPosition, (_target.position - eyesPosition).normalized * _sightRange);
            Gizmos.color = Color.white;
        }
    }
    
}