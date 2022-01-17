using AI;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Player Is In Attack Range")]
    public class PlayerIsInAttackRangeTransitionPredicate : TransitionPredicate
    {
        public override bool IsPredicateSatisfied(GameObject aiGameObject)
        {
            return aiGameObject.GetComponent<CheckLookForPlayer>().Check(2f);
        }
    }
}
