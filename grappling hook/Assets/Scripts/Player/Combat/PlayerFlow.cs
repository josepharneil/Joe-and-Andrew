using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlow : MonoBehaviour
{
    private bool _inFlow = false;
    [SerializeField] private float _maxFlow = 1f;
    [SerializeField] private float _flowDecayRate;
    [SerializeField] private float _flowAddedPerHit;
    [SerializeField] private RectTransform _uiTransform;
    private float _currentFlow;

    //Called in the PlayerCombat Attack() method
    public void BeginFlow()
    {
        //check if in flow, if so add the extra flow,otherwise start the flow
        if (_inFlow)
        {
            AddFlow();
        }
        _currentFlow = _flowAddedPerHit;
        _inFlow = true;
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
