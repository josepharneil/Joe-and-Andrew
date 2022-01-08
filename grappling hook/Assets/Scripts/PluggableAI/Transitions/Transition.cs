using UnityEngine;

namespace PluggableAI
{
    /// <summary>
    /// Transition between states, based on a decision.
    /// </summary>
    [System.Serializable]
    public class Transition : Behaviour
    {
        public Decision decision; // Decision related to this transition.
        public State trueState; // If decision returns true, go to this state.
        public State falseState; // If decision returns false, go to this state.
    }
}
