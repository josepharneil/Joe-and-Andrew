using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStateMachine : MonoBehaviour
{
    List<BaseState> states;
    Dictionary<Type, BaseState> _attachedStates;
    private Dictionary<BaseState,Dictionary<StateTransition,int>> _transitions 
        = new Dictionary<BaseState, Dictionary<StateTransition,int>>();
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
        
        foreach(var b in _attachedStates)
        {
            Dictionary<StateTransition, int> holder = new Dictionary<StateTransition, int>();
            foreach (var c in b.Value.transitions)
            {
                holder.Add(c.Key, c.Value);
            }
            _transitions.Add(b.Value, holder);
        }

        InitialiseStateMachine();
    }

    void InitialiseStateMachine()
    {
        currentState = states[0];
        Debug.Log("___ALL THE STATES ATTACHED___");
        foreach(var a in _attachedStates )
        {
            Debug.Log(a.Value.GetType().ToString());
        }
        Debug.Log("_____ALL THE STATES AND TRANSITIONS______");
        foreach(var a in _transitions)
        {
            foreach(var b in a.Value)
            {
                Debug.Log("State: " + a.Key.GetType() + "  Transition: "+b.Key+"  Priority: " + b.Value);
            }
            
        }
    }

    private void Update()
    {
        bool conditionMet = false;
        foreach (var a in _transitions)
        {
            
            foreach(var b in a.Value)
            { 
                if (b.Key.Condition())
                {
                    conditionMet = true;
                    if(currentState.GetType() != a.Key.GetType())
                    {
                        SwitchToNewState(a.Key.GetType());
                    }

                }

            }
        }
        currentState = conditionMet ? currentState : states[0];
        currentState.Tick();
    }

    private void SwitchToNewState(Type nextState)
    {
        currentState = _attachedStates[nextState];
    }
}
