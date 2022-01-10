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
            Patrol(controller);
        }
        
        private void Patrol(StateController controller)
        {
            controller.gameObject.GetComponent<PatrolPathing>().UpdatePatrol();
        }  
    }
}
