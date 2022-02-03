using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [Serializable] public class PatrolPath
    {
        public enum PatrolType
        {
            Ladder, // Loops in a single direction, then goes back down, like a ladder.
            Cycle, // Loops around the patrol.
        }
        public enum PatrolDirection
        {
            Forwards,
            Backwards
        }
        
        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private PatrolType patrolType = PatrolType.Ladder;
        [SerializeField] private PatrolDirection currentPatrolDirection = PatrolDirection.Forwards;
        [SerializeField] private int currentPatrolPoint = 0;

        public PatrolType GetPatrolType()
        {
            return patrolType;
        }
        
        private Transform SetNextPatrolPoint()
        {
            if (currentPatrolDirection == PatrolDirection.Forwards)
            {
                // Check if we're at the end
                if (currentPatrolPoint + 1 > patrolPoints.Count - 1)
                {
                    switch (patrolType)
                    {
                        case PatrolType.Ladder:
                            // Go back down the ladder.
                            currentPatrolPoint--;
                            currentPatrolDirection = PatrolDirection.Backwards;
                            break;
                        case PatrolType.Cycle:
                            // Start from the beginning.
                            currentPatrolPoint = 0;
                            break;
                        default:
                            Debug.LogError("Unimplemented patrol type.");
                            break;
                    }
                }
                // Otherwise keep going
                else
                {
                    currentPatrolPoint++;
                }
            }
            else // Backwards
            {
                // Check if we're at the end
                if (currentPatrolPoint - 1 < 0)
                {
                    switch (patrolType)
                    {
                        case PatrolType.Ladder:
                            // Go back down the ladder.
                            currentPatrolPoint++;
                            currentPatrolDirection = PatrolDirection.Forwards;
                            break;
                        case PatrolType.Cycle:
                            // Start from the beginning.
                            currentPatrolPoint = patrolPoints.Count - 1;
                            break;
                        default:
                            Debug.LogError("Unimplemented patrol type.");
                            break;
                    }
                }
                // Otherwise keep going
                else
                {
                    currentPatrolPoint--;
                }
            }
            return patrolPoints[currentPatrolPoint];
        }

        public Transform UpdatePatrolPath(Transform patrollerTransform, float distanceThreshold, bool onlyUseXDimension)
        {
            Transform targetPatrolPoint = patrolPoints[currentPatrolPoint];

            bool isAtPatrolPoint = false;
            if (onlyUseXDimension)
            {
                // If we're close enough to our destination point, update the destination
                float targetPatrolPointX = targetPatrolPoint.position.x;
                float thisPositionX = patrollerTransform.position.x;
                isAtPatrolPoint = Mathf.Abs(thisPositionX - targetPatrolPointX) < distanceThreshold;
            }
            else
            {
                Vector2 targetPatrolPointV2 = targetPatrolPoint.position;
                Vector2 thisPosition = patrollerTransform.position;
                float num1 = targetPatrolPointV2.x - thisPosition.x;
                float num2 = targetPatrolPointV2.y - thisPosition.y;
                float sqDistance = (float)((double) num1 * (double) num1 + (double) num2 * (double) num2);
                isAtPatrolPoint = sqDistance < distanceThreshold * distanceThreshold;
            }
            
            if (isAtPatrolPoint)
            {
                targetPatrolPoint = SetNextPatrolPoint();
            }
            return targetPatrolPoint;
        }

        public void Validate()
        {
            foreach (var patrolPoint in patrolPoints)
            {
                Debug.AssertFormat(patrolPoint != null, "Patrol point should not be null", this);
            }
        }

        public void DrawGizmos()
        {
            const float wireSphereRadius = 0.5f;
            
            if (patrolPoints.Count == 0) return;
            
            for (var index = 0; index < patrolPoints.Count - 1; index++)
            {
                // Draw current patrol point sphere
                Transform currPatrolPoint = patrolPoints[index];
                if (currPatrolPoint == null) continue;
                Vector3 currPatrolPointPosition = currPatrolPoint.position;
                Gizmos.DrawWireSphere(currPatrolPointPosition, wireSphereRadius);

                // Draw line to next point
                Transform nextPatrolPoint = patrolPoints[index + 1];
                if (nextPatrolPoint == null) continue;
                Vector3 nextPatrolPointPosition = nextPatrolPoint.position;

                Gizmos.DrawLine(currPatrolPointPosition, nextPatrolPointPosition);
            }
            
            // Draw last sphere
            Transform lastPatrolPoint = patrolPoints[patrolPoints.Count - 1];
            if (lastPatrolPoint != null)
            {
                Gizmos.DrawWireSphere(patrolPoints[patrolPoints.Count - 1].position, wireSphereRadius);

                // If its a cycle, draw a line from the end to the start
                if (patrolType == PatrolType.Cycle)
                {
                    Transform firstPatrolPoint = patrolPoints[0];
                    if (firstPatrolPoint != null)
                    {
                        Gizmos.DrawLine(lastPatrolPoint.position, firstPatrolPoint.position);
                    }
                }
            }
        }
    }
}