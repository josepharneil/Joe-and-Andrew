using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlow : MonoBehaviour
{
    private bool _inFlow=false;
    [SerializeField] private int _maxFlowDuration = 100;
    private int _flowTimer=0;
    [SerializeField] private RectTransform uiTransform;
    void BeginFlow()
    {
        _inFlow = true;
    }
    void ContinueFlow()
    {
        _flowTimer++;
    }
    void EndFlow()
    {
        _inFlow = false;
        _flowTimer = 0;
    }
    void Update()
    {
        
    }
}
