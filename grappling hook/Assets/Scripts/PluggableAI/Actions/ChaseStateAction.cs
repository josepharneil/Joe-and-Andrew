using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName ="PluggableAI/Actions/Chase")]
    public class ChaseStateAction : StateAction
    {
        public override void Act(StateController controller)
        {
            Chase(controller);
        }

        private void Chase(StateController controller)
        {
            controller.gameObject.GetComponent<ChasePathing>().UpdateChase();
        }
    }
}
