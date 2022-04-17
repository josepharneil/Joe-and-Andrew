using UnityEngine;

namespace Enemy
{
    public class ChargeAction : AgentAction
    {
        // Note: these timers could probably just be one timer since they don't overlap lol.
            
        [SerializeField] private float _windUpDuration = 2f;
        private float _windupTimer = 0f;

        [SerializeField] private float _chargeSpeed = 5f;
        [SerializeField] private float _chargeDuration = 2f;
        private float _chargeTimer = 0f;
        private Vector3 _chargeTargetPosition;

        [SerializeField] private float _recoveryDuration = 2f;
        private float _recoveryTimer = 0f;

        public override void OnEnter()
        {
            _windupTimer = 0f;
            _chargeTimer = 0f;
            _recoveryTimer = 0f;
        }

        public override bool WindUp(DemoBossEnemyController bossEnemyController)
        {
            _windupTimer += Time.deltaTime;
            if (_windupTimer < _windUpDuration)
            {
                return false;
            }
            else
            {
                _chargeTargetPosition = bossEnemyController.target.transform.position;
                return true;
            }
        }

        public override bool Act(DemoBossEnemyController bossEnemyController)
        {
            bossEnemyController.MovementController.Move(bossEnemyController.transform.position.DirectionToNormalized(_chargeTargetPosition) * _chargeSpeed);
                
            _chargeTimer += Time.deltaTime;
            return _chargeTimer > _chargeDuration;
        }

        public override bool Recovery(DemoBossEnemyController bossEnemyController)
        {
            _recoveryTimer += Time.deltaTime;
            return _recoveryTimer > _recoveryDuration;
        }

        public override void OnExit(){}
    }
}