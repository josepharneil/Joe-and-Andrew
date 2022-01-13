using System;
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
        [SerializeField] private bool AIActive = true;

        private void Start()
        {
            if (!AIActive)
            {
                return;
            }
            // NOTE This could cause a bug if aiActive isn't initially active at the start,
            // and then its turned on during play... 
            currentState.OnEnter(this);
        }

        private void Update()
        {
            if (!AIActive)
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

        /// <summary>
        /// Called within States when a TransitionPredicate is satisfied.
        /// </summary>
        /// <param name="nextState">The next state to transition to.</param>
        public void TransitionToState(State nextState)
        {
            currentState.OnExit(this);
            currentState = nextState;
            currentState.OnEnter(this);
        }
    }
}
