using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
    public class PatrolStateAction : StateAction
    {
        public override void Act(StateController controller)
        {
            
        }
        
        private void Patrol(StateController controller)
        {
            // Example:
            // controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
            // controller.navMeshAgent.Resume();
            
            // if(controller.nav  .... )
            // blah blah blah, navigation stuff (episode 4)
        }
    }
}
