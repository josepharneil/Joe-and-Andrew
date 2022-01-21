using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Enemy Is Dead")]
    public class EnemyIsDeadTransitionPredicate : TransitionPredicate
    {
        private EntityHealth _entityHealthComponent;
        
        /// <summary>
        /// Is the agent dead?
        /// </summary>
        public override bool IsPredicateSatisfied(GameObject agent)
        {
            Debug.Assert(agent != null, this);
            if (_entityHealthComponent == null)
            {
                agent.TryGetComponent<EntityHealth>(out _entityHealthComponent);
                Debug.Assert(_entityHealthComponent != null,
                    "No EntityHealth component found\n EntityHealth component required for this transition.", 
                    this);
                return false;
            }
            
            return !_entityHealthComponent.IsAlive();
        }
    }
}