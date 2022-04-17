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

        // public List<BossActionState> BossActionStates;
        [SerializeField] private ChargeAttackState _chargeAttackState;

        // public void SetupActions()
        // {
            // _chargeAttackState.Setup(this);
        // }
        
        public void PerformSelectedAction()
        {
            _chargeAttackState.Act();
        }

        [Serializable] public abstract class BossActionState
        {
            protected enum Phase
            {
                OnEnter,
                WindUp,
                Action,
                Recovery,
                OnExit,
            }
            protected Phase CurrentPhase = Phase.OnEnter;
            protected DemoBossEnemyController DemoBossEnemyController;

            public virtual void Setup(DemoBossEnemyController demoBossEnemyController)
            {
                CurrentPhase = Phase.OnEnter;
                DemoBossEnemyController = demoBossEnemyController;
            }

            public virtual void Act()
            {
                switch (CurrentPhase)
                {
                    case Phase.OnEnter:
                        OnEnter();
                        break;
                    case Phase.WindUp:
                        WindUp();
                        break;
                    case Phase.Action:
                        Action();
                        break;
                    case Phase.Recovery:
                        Recovery();
                        break;
                    case Phase.OnExit:
                        OnExit();
                        break;
                }
            }

            protected abstract void OnEnter();
            protected abstract void WindUp();
            protected abstract void Action();
            protected abstract void Recovery();
            protected abstract void OnExit();
        }

        [Serializable] public class ChargeAttackState : BossActionState
        {
            [SerializeField] private float _windUpDuration = 2f;
            private float _windupTimer = 0f;

            [SerializeField] private float _chargeSpeed = 5f;
            [SerializeField] private float _chargeDuration = 2f;
            private float _chargeTimer = 0f;
            private Vector3 _chargeTargetPosition;

            [SerializeField] private float _recoveryDuration = 2f;
            private float _recoveryTimer = 0f;

            // public override void Setup(DemoBossEnemyController demoBossEnemyController)
            // {
                // base.Setup(demoBossEnemyController);
            // }

            protected override void OnEnter()
            {
                _windupTimer = 0f;
                _chargeTimer = 0f;
                _recoveryTimer = 0f;
                CurrentPhase = Phase.WindUp;
            }

            protected override void WindUp()
            {
                _windupTimer += Time.deltaTime;
                if (_windupTimer > _windUpDuration)
                {
                    _chargeTargetPosition = DemoBossEnemyController.target.transform.position;
                    CurrentPhase = Phase.Action;
                }
            }

            protected override void Action()
            {
                DemoBossEnemyController.MovementController.Move(DemoBossEnemyController.transform.position.DirectionToNormalized(_chargeTargetPosition) * _chargeSpeed);
                
                _chargeTimer += Time.deltaTime;
                if (_chargeTimer > _chargeDuration)
                {
                    CurrentPhase = Phase.Recovery;
                }
            }

            protected override void Recovery()
            {
                _recoveryTimer += Time.deltaTime;
                if (_recoveryTimer > _recoveryDuration)
                {
                    CurrentPhase = Phase.OnExit;
                }
            }
                        
            protected override void OnExit()
            {
                CurrentPhase = Phase.OnEnter;
            }
        }
        
        

        // [Header("Actions")]
        // public int SelectedActionIndex { public set; private get; } = 0;
        public int SelectedActionIndex { private get; set; }

        [SerializeField] private List<AgentAction> _agentActions;
        
        

        public int GetNumActions()
        {
            return _agentActions.Count;
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
            // for()
            _chargeAttackState.Setup(this);
        }

        // private void Start()
        // {
            // CurrentAction = _chargeAction;
        // }

        
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