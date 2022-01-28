using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "AI/Transition Predicates/Can See Target")]
    public class CanSeeTargetTransitionPredicate : TransitionPredicate
    {
        private Enemy.EnemyCheckCanSeeTarget _enemyCheckCanSeeTarget;
        
        /// <summary>
        /// Is the target in view.
        /// </summary>
        public override bool IsPredicateSatisfied(GameObject agent)
        {
            if (_enemyCheckCanSeeTarget != null)
            {
                return _enemyCheckCanSeeTarget.CheckCanSeeTarget();
            }
            
            Debug.Assert(agent != null, this);
            agent.TryGetComponent<Enemy.EnemyCheckCanSeeTarget>(out _enemyCheckCanSeeTarget);
            Debug.Assert(_enemyCheckCanSeeTarget != null,
                "No EnemyCheckCanSeeTarget component found\n EnemyCheckCanSeeTarget component required for this transition.", 
                this);
            return false;

        }
        
        
    }
}