using System;
using System.Text;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerAttackDriver
    {
        // Start attack, windup for this amount of time
        [Range(0f,5f)] [SerializeField] private float _windUpDuration = 0.5f;
        
        // Attack frame
        
        // End attack, recovery for this amount of time
        [Range(0f,5f)] [SerializeField] private float _recoveryDuration = 0.5f;
        
        private float _timer = 0f;

        [SerializeField] private PlayerCombat _combat;

        [SerializeField] private bool _showDebugGUI = false;

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
                    Debug.LogError("Unknown attack state in PlayerAttackDriver", _combat);
                    return;
            }
        }

        private void WindUpAttack()
        {
            _timer += Time.deltaTime;
            if (_timer < _windUpDuration) return;
            
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
            if (_timer < _recoveryDuration) return;
            
            _timer = 0f;
            _attackState = AttackState.NotAttacking;
        }

        private float? GetCurrentDuration()
        {
            switch (_attackState)
            {
                case AttackState.NotAttacking:
                    break;
                case AttackState.WindUp:
                    break;
                case AttackState.AttackFrame:
                    break;
                case AttackState.Recovery:
                    break;
                default:
                    Debug.LogError("Unknown attack state in PlayerAttackDriver", _combat);
                    break;
            }

            return 0f;
        }
        
        public void ShowDebugGUI()
        {
            if (!_showDebugGUI) return;
            if (_attackState == AttackState.NotAttacking) return;
            
            GUI.skin.box.fontSize = 24;
            
            Vector2 targetPos = CameraManager.GetActiveCamera().WorldToScreenPoint(_combat.transform.position);
            targetPos.y += 50;

            StringBuilder guiTextBuilder = new StringBuilder("Recovery: 0.00/0.00 (100%)".Length);

            void BuildString()
            {
                float durationToUse = _attackState == AttackState.Recovery ? _recoveryDuration : _windUpDuration;
                
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
            
            GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 120, 40),
                guiTextBuilder.ToString());
        }
    }
}