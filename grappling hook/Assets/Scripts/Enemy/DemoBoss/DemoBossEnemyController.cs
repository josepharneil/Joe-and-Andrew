using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Enemy
{
    public class DemoBossEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [HideInInspector] public MovementController MovementController;
        [SerializeField] private BoxCollider2D arenaBox;
        [HideInInspector] public GameObject target;


        // [Header("Actions")]
        // public int SelectedActionIndex { public set; private get; } = 0;
        public int SelectedActionIndex { private get; set; }

        [SerializeField] private List<AgentAction> _agentActions;

        public int GetNumActions()
        {
            return _agentActions.Count;
        }

        public DemoBossEnemyController GetSelf()
        {
            return this;
        }

        public AgentAction GetSelectedAction()
        {
            return _agentActions[SelectedActionIndex];
        }
        
        // [SerializeField] private ChargeAction _chargeAction;

        private void Awake()
        {
            MovementController = GetComponent<MovementController>();
        }

        private void Start()
        {
            // CurrentAction = _chargeAction;
        }

        
        public bool TargetIsInBossArea()
        {
            return arenaBox.bounds.Contains(target.transform.position);
        }
    }
    
    // public abstract class AgentAction
    // {
    //     public virtual void OnEnter(){}
    //
    //     public virtual bool WindUp(DemoBossEnemyController bossEnemyController)
    //     {
    //         return false;
    //     }
    //     
    //     public virtual bool Act(DemoBossEnemyController bossEnemyController){return false;}
    //     
    //     public virtual bool Recovery(DemoBossEnemyController bossEnemyController){return false;}
    //     
    //     public virtual void OnExit(){}
    // }
}