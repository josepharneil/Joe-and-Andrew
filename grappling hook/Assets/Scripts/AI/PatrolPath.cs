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

        public List<Transform> GetPatrolPoints()
        {
            return patrolPoints;
        }

        public PatrolType GetPatrolType()
        {
            return patrolType;
        }

        public Transform GetCurrentPatrolPoint()
        {
            return patrolPoints[currentPatrolPoint];
        }

        public Transform SetNextPatrolPoint()
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


        public void Validate()
        {
            foreach (var patrolPoint in patrolPoints)
            {
                Debug.AssertFormat(patrolPoint != null, "Patrol point should not be null", this);
            }
        }
    }
}