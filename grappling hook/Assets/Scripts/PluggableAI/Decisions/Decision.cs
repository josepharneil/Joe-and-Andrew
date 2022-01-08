using UnityEngine;

namespace PluggableAI
{ 
    /// <summary>
    /// Things the AI does that DO have potential to change state, eg patrolling or attacking.
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController controller);
    }
}
