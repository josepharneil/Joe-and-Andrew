using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/State Actions/Simple Patrol Pathing")]
    public class SimplePatrolStateAction : StateAction
    {
        //private SimplePatrol _patrolAction;
        public override void PerformAction(GameObject agent)
        {
            SimplePatrol patrolAction = null;
            if (!patrolAction)
            {
                agent.TryGetComponent<SimplePatrol>(out patrolAction);
                if (!patrolAction)
                {
                    Debug.Assert(patrolAction, "no patrol action", this);
                    return;
                }
            }
            patrolAction.UpdatePatrol();
        }
    }
}
