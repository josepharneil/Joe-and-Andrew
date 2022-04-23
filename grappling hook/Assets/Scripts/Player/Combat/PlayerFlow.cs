using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlow : MonoBehaviour
{
    private bool _inFlow = false;
    //having this script refernce the player inputs, maybe there is a cleaner way to do it in case 
    //there are other modifiers affecting move or attack speed
    [Header("Components")]
    [SerializeField] private Player.PlayerInputs _playerInputs;
    [SerializeField] private RectTransform _uiTransform;

    [Header("Stats")]
    [SerializeField] private float _maxFlow = 1f;
    [SerializeField] private float _flowDecayRate;
    [SerializeField] private float _flowAddedPerHit;
    private float _currentFlow;

    [Header("Testing")]
    [SerializeField] private bool _increaseMoveSpeed;
    [SerializeField] private float _moveSpeedIncrease;
    [SerializeField] private bool _increaseAttackDamage;
    [SerializeField] private bool _incraseAttackSpeed;
    [SerializeField] private float _attackSpeedIncrease;
    //this is the increase as proportional to the amount of flow the player has compaerd to some arbitrary maximum
    [SerializeField] private bool _setIncreasesProportionalToAmountOfFlow;
    //this one only turns on the effects of flow if the player gets it to a certain level, then keeps it on until it drops
    [SerializeField] private bool _buldFlowBeforeActivating;

    //Called in the PlayerCombat Attack() method
    public void BeginFlow()
    {
        //check if in flow, if so add the extra flow,otherwise start the flow
        if (_inFlow)
        {
            AddFlow();
        }
        else
        {
            _currentFlow = _flowAddedPerHit;
            _inFlow = true;
            //checks the conditions and applies the selected testing parameters
            if (_increaseMoveSpeed)
            {
                _playerInputs.MultiplyMoveSpeed(_moveSpeedIncrease);
            }
            if (_incraseAttackSpeed)
            {
                _playerInputs.GetPlayerAttackDriver().AttackSpeed = _playerInputs.GetPlayerAttackDriver().AttackSpeed * _attackSpeedIncrease;
            }

        }
    }
    void ContinueFlow()
    {
        //scales the UI bar based on how much flow there is compared to the maximum
        Vector3 scaleVector = _uiTransform.localScale;
        float xScale = _currentFlow / _maxFlow;
        scaleVector.x = xScale;
        _uiTransform.localScale = scaleVector;
        //reduce the flow by the decay rate
        _currentFlow -= _flowDecayRate * Time.deltaTime;
    }
    void EndFlow()
    {
        _currentFlow = 0f;
        _inFlow = false;
        if (_increaseMoveSpeed)
        {
            _playerInputs.ResetMoveSpeed();
        }
        if (_incraseAttackSpeed)
        {
            _playerInputs.GetPlayerAttackDriver().AttackSpeed = 1f;
        }
    }

    void AddFlow()
    {
        //check if adding the flow would tip it over the maximum
        //if it would, just go to the max, otherwise add the flow
        if (_currentFlow + _flowAddedPerHit >= _maxFlow)
        {
            _currentFlow = _maxFlow;
        }
        else
        {
            _currentFlow += _flowAddedPerHit;
        }
    }
    void Update()
    {
        if (_inFlow)
        {
            ContinueFlow();
            if (_currentFlow<=0f)
            {
                EndFlow();
            }
        }
    }

}
