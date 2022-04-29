using System;
using System.Text;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerAttackDriver
    {
        [Header("Components")]
        private PlayerCombat _combat;

        [Header("Attack Timing")]
        // Start attack, windup for this amount of time
        [Range(0f,5f)] [SerializeField] private float _windUpDuration = 0.5f;
        // Attack frame <- no timer here
        // End attack, recovery for this amount of time
        [Range(0f,5f)] [SerializeField] private float _recoveryDuration = 0.5f;

        public float AttackSpeed = 1f;
        
        private float _timer = 0f;

        [Header("Debug")]
        [SerializeField] private bool _showDebugGUI = false;

        public void Initialise(PlayerCombat combat)
        {
            _combat = combat;
        }

        private enum AttackState
        {
            NotAttacking,
            WindUp,
            AttackFrame,
            Recovery
        }
        private AttackState _attackState = AttackState.NotAttacking;

        public void StartAttack()
        {
            if (_attackState != AttackState.NotAttacking) return;

            _attackState = AttackState.WindUp;
        }

        public void UpdateAttack()
        {
            switch (_attackState)
            {
                case AttackState.NotAttacking:
                    return;
                case AttackState.WindUp:
                    WindUpAttack();
                    break;
                case AttackState.AttackFrame:
                    AttackFrame();
                    break;
                case AttackState.Recovery:
                    Recovery();
                    break;
                default:
                    Debug.LogError("Unknown attack state in PlayerAttackDriver");
                    return;
            }
        }

        private float GetWindUpDuration()
        {
            return _windUpDuration / AttackSpeed;
        }

        private float GetRecoveryDuration()
        {
            return _recoveryDuration / AttackSpeed;
        }

        private void WindUpAttack()
        {
            _timer += Time.deltaTime;
            if (_timer < GetWindUpDuration()) return;
            
            _timer = 0f;
            _attackState = AttackState.AttackFrame;
        }

        private void AttackFrame()
        {
            _combat.Attack(0);//int not used for now...
            
            _attackState = AttackState.Recovery;
        }

        private void Recovery()
        {
            _timer += Time.deltaTime;
            if (_timer < GetRecoveryDuration()) return;
            
            _timer = 0f;
            _attackState = AttackState.NotAttacking;
        }

        public void ShowDebugGUI()
        {
            if (!_showDebugGUI) return;
            if (_attackState == AttackState.NotAttacking) return;
            
            GUI.skin.box.fontSize = 24;
            
            Vector2 targetPos = CameraManager.GetActiveCamera().WorldToScreenPoint(_combat.PlayerTransform.position);
            targetPos.y += 50;

            StringBuilder guiTextBuilder = new StringBuilder(26);//"Recovery: 0.00/0.00 (100%)".Length

            void BuildString()
            {
                if(_attackState == AttackState.AttackFrame)
                {
                    guiTextBuilder.Append(_attackState);
                    return;
                }
                float durationToUse = _attackState == AttackState.Recovery ? GetRecoveryDuration() : GetWindUpDuration();
                
                guiTextBuilder.Append(_attackState);
                guiTextBuilder.Append(": ");
                guiTextBuilder.Append(_timer.ToString("N2"));
                guiTextBuilder.Append("/");
                guiTextBuilder.Append(durationToUse.ToString("N2"));
                guiTextBuilder.Append(" (");
                guiTextBuilder.Append((_timer / durationToUse).ToString("P0"));
                guiTextBuilder.Append(")");
            }
            BuildString();
            
            GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 400, 40),
                guiTextBuilder.ToString());
        }
    }
}