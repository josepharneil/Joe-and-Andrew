using AI;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Cannot See Player")]
    public class CannotSeePlayerTransitionPredicate : TransitionPredicate
    {
        public override bool IsPredicateSatisfied(GameObject aiGameObject)
        {
            return !aiGameObject.GetComponent<CheckLookForPlayer>().Check(5f);
        }
    }
}
