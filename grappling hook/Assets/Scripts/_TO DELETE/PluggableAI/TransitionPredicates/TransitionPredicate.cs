using UnityEngine;

namespace PluggableAI
{ 
    /// <summary>
    /// Used within Transitions. If the main function PredicateIsSatisfied returns true, the Transition activates
    /// and transitions to the defined state.
    /// </summary>
    public abstract class TransitionPredicate : ScriptableObject
    {
        public abstract bool PredicateIsSatisfied(StateController controller);
    }
}
