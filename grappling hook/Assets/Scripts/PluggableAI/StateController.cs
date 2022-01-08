using UnityEngine;

namespace PluggableAI
{
    // See https://www.youtube.com/watch?v=cHUXh5biQMg&list=PLX2vGYjWbI0ROSj_B0_eir_VkHrEkd4pi&index=2

    /// <summary>
    /// Central state controller for AI.
    /// </summary>
    public class StateController : MonoBehaviour
    {
        public State currentState;

        private bool _aiActive = true;

        public static State RemainState; // Special state that indicates we don't transition away.

        private void Update()
        {
            if (!_aiActive)
            {
                return;
            }
            // Update the current state!
            currentState.UpdateState(this);
        }

        private void OnDrawGizmos()
        {
            if (currentState == null)
            {
                return;
            }

            Gizmos.color = currentState.sceneGizmoColour;
            // For example, draw eye position if looking
            // Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
        }

        public void TransitionToState(State nextState)
        {
            if (nextState != RemainState)
            {
                currentState = nextState;
            }
        }
    }
}
