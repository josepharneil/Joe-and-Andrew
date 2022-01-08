using UnityEditor;
using UnityEngine;

namespace PluggableAI
{
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
    public class LookDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetVisible = Look(controller);
            return targetVisible;
        }

        private bool Look(StateController controller)
        {
            // TODO do code here, e.g RayCast, SphereCast
            // Can include Debug.DrawRay for example
            // SphereCast gives a nice area
            return false;
        }
    }
}
