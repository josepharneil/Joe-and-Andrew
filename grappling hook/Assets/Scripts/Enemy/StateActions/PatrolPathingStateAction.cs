using UnityEngine;

namespace AI
{ 
    [CreateAssetMenu(menuName = "EnemyAI/Actions/PatrolPathing")]
    public class PatrolPathingStateAction : StateAction
    {
        public override void PerformAction(GameObject aiGameObject)
        {
            Patrol(aiGameObject);
        }
        
        private void Patrol(GameObject aiGameObject)
        {
            aiGameObject.GetComponent<PatrolPathing>().UpdatePatrol();
        }

    }
}