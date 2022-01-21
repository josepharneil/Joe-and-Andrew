using UnityEngine;

namespace AI
{
    public abstract class TransitionPredicate : ScriptableObject
    {
        public abstract bool IsPredicateSatisfied(GameObject agent);
    }
}
