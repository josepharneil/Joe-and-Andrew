using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/State Actions/Simple Patrol Pathing")]
    public class SimplePatrolStateAction : StateAction
    {
        private PatrolBase _patrolAction;
        public override void PerformAction(GameObject self)
        {
            if (_patrolAction)
            {
                _patrolAction.UpdatePatrol();
            }
            else
            {
                _patrolAction = self.GetComponent<PatrolBase>();
            }
        }
    }
}
