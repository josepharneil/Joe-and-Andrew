using AI;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Can See Player")]
    public class CanSeePlayerTransitionPredicate : TransitionPredicate
    {
        public override bool IsPredicateSatisfied(GameObject aiGameObject)
        {
            return aiGameObject.GetComponent<CheckLookForPlayer>().Check(5f);
        }
    }
}
