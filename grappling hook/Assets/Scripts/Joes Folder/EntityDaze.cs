using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDaze : MonoBehaviour
{
    [Tooltip("When the entity is hit, how long are they dazed for?")]
    [SerializeField] private float dazeDuration = 0.8f;
    
    public bool isDazed = false;
    private float _dazeTimer = 0.0f;

    public void Daze()
    {
        isDazed = true;
        _dazeTimer = dazeDuration;
    }

    private void Update()
    {
        if (isDazed)
        {
            _dazeTimer -= Time.deltaTime;
            if (_dazeTimer <= 0.0f)
            {
                isDazed = false;
            }
        }
    }
}
