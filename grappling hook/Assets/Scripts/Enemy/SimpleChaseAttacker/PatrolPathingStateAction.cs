using UnityEngine;

namespace AI
{ 
    [CreateAssetMenu(menuName = "AI/State Actions/Patrol Pathing")]
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