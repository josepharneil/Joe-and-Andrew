using System;
using UnityEngine;

namespace Enemy
{
    [Serializable] public class AgentAction : MonoBehaviour
    {
        public virtual void OnEnter(){}

        public virtual bool WindUp(DemoBossEnemyController bossEnemyController)
        {
            return false;
        }
        
        public virtual bool Act(DemoBossEnemyController bossEnemyController){return false;}
        
        public virtual bool Recovery(DemoBossEnemyController bossEnemyController){return false;}
        
        public virtual void OnExit(){}
    }
}