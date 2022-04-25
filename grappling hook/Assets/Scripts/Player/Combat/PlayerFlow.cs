using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFlow : MonoBehaviour
{
    private bool _inFlow = false;
    //having this script refernce the player inputs, maybe there is a cleaner way to do it in case 
    //there are other modifiers affecting move or attack speed
    [Header("Components")]
    [SerializeField] private Player.PlayerInputs _playerInputs;
    [SerializeField] private RectTransform _uiTransform;
    [SerializeField] private RectTransform _bgTransform;
    [SerializeField] private Image _barImage;
    
    [Header("Flow Stats")]
    [SerializeField] private float _maxFlow = 1f;
    [SerializeField] private float _flowDecayRate;
    [SerializeField] private float _flowAddedPerHit;
    private float _currentFlow;

    [Header("MoveSpeed")]
    [SerializeField] private bool _increaseMoveSpeed;
    [SerializeField] private float _moveSpeedIncrease;
    
    [Header("Attack Damage")]
    //TODO implement
    [SerializeField] private bool _increaseAttackDamage;
    
    [Header("Attack Speed")]
    [SerializeField] private bool _increaseAttackSpeed;
    [SerializeField] private float _attackSpeedIncrease = 2f;
    
    [Header("Flow type")]
    //this is the increase as proportional to the amount of flow the player has compaerd to some arbitrary maximum
    //TODO implement
    [SerializeField] private bool _setIncreasesProportionalToAmountOfFlow;
    //this one only turns on the effects of flow if the player gets it to a certain level, then keeps it on until it drops
    [SerializeField] private bool _buildFlowBeforeActivating;
    private bool _hasHitMaximumFlow = false;

    [Header("Big Bar")]
    [SerializeField] private bool _bigFlowBar;
    [SerializeField] private bool _tieredFlow;
    //TODO implement

    private void OnValidate()
    {
        Debug.Assert(_attackSpeedIncrease != 0, "Shouldn't be 0", this);
    }

    private void Start()
    {
        if (_buildFlowBeforeActivating)
        {
            _barImage.color = Color.gray;
        }
        if (_bigFlowBar)
        {
            //makes the transform of the bar bigger, as well as increasing the actual flow the player can generate
            _uiTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700f);
            _bgTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 750f);
            Vector3 scaleVector = _uiTransform.localScale;
            scaleVector.x = 0f;
            _uiTransform.localScale = scaleVector;
            _maxFlow = 1000;
            _flowDecayRate = 10;
            _flowAddedPerHit = 50;
        }
    }

    // Called in the PlayerCombat Attack() method
    public void BeginFlow()
    {
        // check if in flow, if so add the extra flow,otherwise start the flow
        if (_inFlow)
        {
            AddFlow();
        }
        else
        {
            _currentFlow = _flowAddedPerHit;
            _inFlow = true;
            // checks the conditions and applies the selected testing parameters
            if (!_buildFlowBeforeActivating)
            {
                if (_setIncreasesProportionalToAmountOfFlow)
                {
                    return;
                }
                if (_increaseMoveSpeed)
                {
                    _playerInputs.MultiplyMoveSpeed(_moveSpeedIncrease);
                }
                if (_increaseAttackSpeed)
                {
                    Player.PlayerAttackDriver attackDriver = _playerInputs.GetPlayerAttackDriver();
                    attackDriver.AttackSpeed *= _attackSpeedIncrease;
                }
            }
        }
    }
    
    private void ContinueFlow()
    {

        //scales the UI bar based on how much flow there is compared to the maximum
        Vector3 scaleVector = _uiTransform.localScale;
        float xScale = _currentFlow / _maxFlow;
        scaleVector.x = xScale;
        _uiTransform.localScale = scaleVector;
        if (_setIncreasesProportionalToAmountOfFlow)
        {
            //ak 25/4/22 super simple linear scaling atm, might be worth making it curved at some point to test feel
            float proportionalIncrease = (_maxFlow + _currentFlow) / _maxFlow;
            if (_increaseMoveSpeed)
            {
                _playerInputs.MultiplyMoveSpeed(_moveSpeedIncrease * proportionalIncrease);
            }
            if (_increaseAttackSpeed)
            {
                _playerInputs.GetPlayerAttackDriver().AttackSpeed = _playerInputs.GetPlayerAttackDriver().AttackSpeed * _attackSpeedIncrease;
            }
        }
        //reduce the flow by the decay rate
        _currentFlow -= _flowDecayRate * Time.deltaTime;
    }
    
    private void EndFlow()
    {
        _currentFlow = 0f;
        _inFlow = false;
        if (_increaseMoveSpeed)
        {
            _playerInputs.ResetMoveSpeed();
        }
        if (_increaseAttackSpeed)
        {
            _playerInputs.GetPlayerAttackDriver().AttackSpeed = 1f;
        }
        if (_buildFlowBeforeActivating)
        {
            _barImage.color = Color.gray;
            _hasHitMaximumFlow = false;
        }
    }

    private void AddFlow()
    {

        //check if adding the flow would tip it over the maximum
        //if it would, just go to the max, otherwise add the flow
        if (_currentFlow + _flowAddedPerHit >= _maxFlow)
        {
            _currentFlow = _maxFlow;
            if (_buildFlowBeforeActivating && !_hasHitMaximumFlow)
            {
                _barImage.color = Color.green;
                if (_increaseMoveSpeed)
                {
                    _playerInputs.MultiplyMoveSpeed(_moveSpeedIncrease);
                }
                if (_increaseAttackSpeed)
                {
                    _playerInputs.GetPlayerAttackDriver().AttackSpeed = _playerInputs.GetPlayerAttackDriver().AttackSpeed * _attackSpeedIncrease;
                }
                _hasHitMaximumFlow = true;
            }
        }
        else
        {
            _currentFlow += _flowAddedPerHit;

        }
    }
    
    private void Update()
    {
        if (!_inFlow) return;
        
        ContinueFlow();
        if (_currentFlow <= 0f)
        {
            EndFlow();
        }
    }

}
