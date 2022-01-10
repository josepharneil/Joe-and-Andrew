using UnityEngine;

namespace PluggableAI
{
    // See https://www.youtube.com/watch?v=cHUXh5biQMg&list=PLX2vGYjWbI0ROSj_B0_eir_VkHrEkd4pi&index=2

    /// <summary>
    /// Central state controller for AI.
    /// </summary>
    public class StateController : MonoBehaviour
    {
        [Tooltip("Current state.")]
        public State currentState;

        [Tooltip("Is the AI Active?")]
        [SerializeField] private bool aiActive = true;

        [Tooltip("Unique, special state that indicates we don't transition away.")]
        public State remainState;

        private void Update()
        {
            if (!aiActive)
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
            // NOTE: We maybe want to implement OnEnter and OnExit on states?
            if (nextState != remainState)
            {
                currentState = nextState;
            }
        }
    }
}
