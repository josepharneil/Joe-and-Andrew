using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Cannot See Player")]
    public class CannotSeePlayerTransitionPredicate : TransitionPredicate
    {
        public override bool IsPredicateSatisfied(GameObject agent)
        {
            return !agent.GetComponent<CheckLookForPlayer>().Check(5f);
        }
    }
}
