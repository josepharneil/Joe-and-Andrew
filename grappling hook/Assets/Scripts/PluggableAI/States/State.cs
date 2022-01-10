using Cinemachine;
using UnityEngine;

namespace PluggableAI
{
    /// <summary>
    /// Holds a collection of Actions and Decisions.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/State")]
    public class State : ScriptableObject
    {
        [Tooltip("A list of actions taken in this state.")]
        public StateAction[] actions;

        [Tooltip("A list of transitions from this state.")]
        public Transition[] transitions;

        [Tooltip("Colour of the gizmo in the scene.")]
        public Color sceneGizmoColour = Color.grey;
        
        // NOTE: We may want to add OnEnter and OnExit ?
        public void UpdateState(StateController controller)
        {
            DoActions(controller);
            CheckTransitions(controller);
        }

        private void DoActions(StateController controller)
        {
            foreach (StateAction action in actions)
            {
                action.Act(controller);
            }
        }

        private void CheckTransitions(StateController controller)
        {
            foreach (Transition transition in transitions)
            {
                bool decisionSucceeded = transition.decision.Decide(controller);
                controller.TransitionToState(decisionSucceeded ? transition.trueState : transition.falseState);
            }
        }
    }
}
