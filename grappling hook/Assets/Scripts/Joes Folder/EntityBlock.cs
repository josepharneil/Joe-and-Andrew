using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlock : MonoBehaviour
{
    public bool isBlocking = false;
    public int blockDamageReductionFactor = 2;

    public bool blockHasBeenBroken = false;
    [SerializeField] private float blockBreakDuration = 1.0f;
    private float _blockTimer = 0.0f;
    public float blockSpeedModifier = 0.25f;

    public void BreakBlock()
    {
        blockHasBeenBroken = true;
        _blockTimer = blockBreakDuration;
    }
    
    /// <summary>
    /// Todo maybe unify all timers into one single update function in one script
    /// </summary>
    private void Update()
    {
        if (!blockHasBeenBroken) return;
        
        _blockTimer -= Time.deltaTime;
        if (_blockTimer <= 0)
        {
            blockHasBeenBroken = false;
        }
    }
}
