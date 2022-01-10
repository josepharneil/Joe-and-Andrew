using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
    public class ChaseStateAction : StateAction
    {
        private ChasePathing _chasePathing;

        public override void Act(StateController controller)
        {
            Chase(controller);
        }

        private void Chase(StateController controller)
        {
            if (_chasePathing == null)
            {
                _chasePathing = controller.gameObject.GetComponent<ChasePathing>();
            }
            _chasePathing.UpdateChase();
        }
    }
}
