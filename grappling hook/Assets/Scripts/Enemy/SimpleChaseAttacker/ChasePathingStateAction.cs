using UnityEngine;

namespace AI
{ 
    [CreateAssetMenu(menuName = "AI/State Actions/Chase Pathing")]
    public class ChasePathingStateAction : StateAction
    {
        public override void PerformAction(GameObject agent)
        {
            Patrol(agent);
        }
        
        private void Patrol(GameObject aiGameObject)
        {
            aiGameObject.GetComponent<ChaseRunAtPlayer>().UpdateChase();
        }

    }
}