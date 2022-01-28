using UnityEngine;

namespace AI
{ 
    [CreateAssetMenu(menuName = "AI/State Actions/Chase Target")]
    public class ChaseTargetStateAction : StateAction
    {
        private Enemy.EnemyChaseTarget _enemyChaseTarget; 
        public override void PerformAction(GameObject agent)
        {
            Patrol(agent);
        }
        
        private void Patrol(GameObject agent)
        {
            if (_enemyChaseTarget == null)
            {
                agent.TryGetComponent<Enemy.EnemyChaseTarget>(out _enemyChaseTarget);
                Debug.Assert(_enemyChaseTarget != null, "No EnemyChaseTarget found", this);
                if (!_enemyChaseTarget)
                {
                    return;
                }
            }
            _enemyChaseTarget.UpdateChaseTarget();
        }

    }
}