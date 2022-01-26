using Enemy;
using UnityEngine;

namespace AI
{ 
    [CreateAssetMenu(menuName = "AI/State Actions/Destroy")]
    public class DestroyStateAction : StateAction
    {
        public override void PerformAction(GameObject aiGameObject)
        {
            RequestDestruction(aiGameObject);
        }
        
        private static void RequestDestruction(GameObject aiGameObject)
        {
            aiGameObject.TryGetComponent<EnemyParentReference>(out EnemyParentReference parentGameObject);
            if (parentGameObject)
            {
                EnemyManager.Instance.AddForDestruction(parentGameObject.parent);
            }
            else
            {
                Debug.LogError("There should be an EnemyParentReference component on this. " +
                               "Attempting to destroy the AI object instead.");
                EnemyManager.Instance.AddForDestruction(aiGameObject);
            }
        }

    }
}