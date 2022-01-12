namespace PluggableAI
{
    /// <summary>
    /// Transition between states, based on a decision.
    /// </summary>
    [System.Serializable]
    public class Transition
    {
        public TransitionPredicate transitionPredicate; // Predicate related to this transition.
        public State nextState; // If predicate is true, go to this state.
    }
}
