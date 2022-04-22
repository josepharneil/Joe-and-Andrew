using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlow : MonoBehaviour
{
    private bool _inFlow=false;
    [SerializeField] private int _maxFlowDuration = 100;
    private int _flowTimer=0;
    [SerializeField] private RectTransform _uiTransform;

    //Called in the PlayerCombatAttackCode
    public void BeginFlow()
    {
        _flowTimer = 0;
        _inFlow = true;
    }
    void ContinueFlow()
    {
        Vector3 scaleVector = _uiTransform.localScale;
        float xScale = 1-(float)_flowTimer / (float)_maxFlowDuration;
        scaleVector.x = xScale;
        _uiTransform.localScale = scaleVector;
        _flowTimer++;
    }
    void EndFlow()
    {
        _inFlow = false;
        _flowTimer = _maxFlowDuration;
    }
    void Update()
    {
        if (_inFlow)
        {
            ContinueFlow();
            if (_flowTimer >= _maxFlowDuration)
            {
                EndFlow();
            }
        }
    }
}
