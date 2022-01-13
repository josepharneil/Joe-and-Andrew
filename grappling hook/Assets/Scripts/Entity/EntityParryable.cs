using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityParryable : MonoBehaviour
{
    public bool isCurrentlyParryable;
    public bool hasBeenParried;

    [SerializeField] private float parryDuration = 1f;
    private float _parryTimer;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color _originalColor;
    
    private void Start()
    {
        _originalColor = spriteRenderer.color;
    }

    public void SetParryable()
    {
        spriteRenderer.color = Color.red;
        isCurrentlyParryable = true;
    }
    
    public void SetNotParryable()
    {
        spriteRenderer.color = _originalColor;
        isCurrentlyParryable = false;
    }

    public void Parry()
    {
        SetNotParryable();
        hasBeenParried = true;
        _parryTimer = parryDuration;
        print("parried");
    }

    private void Update()
    {
        if (hasBeenParried)
        {
            _parryTimer -= Time.deltaTime;
            if (_parryTimer <= 0.0f)
            {
                hasBeenParried = false;
            }
        }
    }
}
