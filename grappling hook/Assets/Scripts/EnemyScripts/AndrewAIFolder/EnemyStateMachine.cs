using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStateMachine : MonoBehaviour
{
    List<BaseState> states;
    Dictionary<Type, BaseState> _attachedStates;
    BaseState? currentState;
    // Start is called before the first frame update
    void Awake()
    {
        //The order of the states on the game object is the order that they appear in the game object
        //So if we want the enemy to start chasing the player from moment one, we can just put it as the first state that is called
        states = new List<BaseState>();
        _attachedStates = new Dictionary<Type, BaseState>();
        gameObject.GetComponents(states);

        foreach(var a in states)
        {
            _attachedStates.Add(a.GetType(), a);
        }

        InitialiseStateMachine();
    }

    void InitialiseStateMachine()
    {
        currentState = states[0];
    }

    private void Update()
    {
        var nextState = currentState?.Tick();
        if (nextState != null)
        {
            SwitchToNewState(nextState);
        }
    }

    private void SwitchToNewState(Type nextState)
    {
        currentState = _attachedStates[nextState];
    }
}
