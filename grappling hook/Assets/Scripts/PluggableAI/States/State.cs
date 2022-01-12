using Cinemachine;
using UnityEngine;

namespace PluggableAI
{
    /// <summary>
    /// Holds a collection of Actions and Transitions.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/State")]
    public class State : ScriptableObject
    {
        [Tooltip("State Action on Enter")] 
        [SerializeField] private StateAction onEnterAction;

        [Tooltip("State Action on Exit")]
        [SerializeField] private StateAction onExitAction;
        
        [Tooltip("A list of actions taken in this state.")]
        [SerializeField] private StateAction[] stateActions;

        [Tooltip("A list of transitions from this state.")]
        [SerializeField] private Transition[] transitions;

        [Tooltip("Colour of the gizmo in the scene.")]
        public Color sceneGizmoColour = Color.grey;

        public void OnEnter(StateController controller)
        {
            if (onEnterAction)
            {
                onEnterAction.Act(controller);
            }
        }

        public void OnExit(StateController controller)
        {
            if(onExitAction)
            {
                onExitAction.Act(controller);
            }
        }

        public void UpdateState(StateController controller)
        {
            PerformStateActions(controller);
            CheckTransitions(controller);
        }

        private void PerformStateActions(StateController controller)
        {
            foreach (StateAction stateAction in stateActions)
            {
                stateAction.Act(controller);
            }
        }

        private void CheckTransitions(StateController controller)
        {
            foreach (Transition transition in transitions)
            {
                if (!transition.transitionPredicate.PredicateIsSatisfied(controller)) continue;
                
                // We no longer need to check other transitions once we transition once.
                controller.TransitionToState(transition.nextState);
                break;
            }
        }
    }
}
