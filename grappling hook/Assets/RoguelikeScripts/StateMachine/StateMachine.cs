using System;
using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using Object = System.Object;

// SOURCE: https://www.youtube.com/watch?v=V75hgcsCGOM

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

public class StateMachine
{
    private IState _currentState; // Current state of the State Machine.
    public string GetCurrentStateTypeName()
    {
        return _currentState.GetType().ToString();
    }

    // Map from Type (usually State i think?) to transitions
    // This is effectively all the lines coming out of each state
    private Dictionary<Type, List<Transition>> _allTransitionsDict = new Dictionary<Type, List<Transition>>();

    // The list of current / active transitions, when  we're in a given state.
    private List<Transition> _activeTransitions = new List<Transition>();
    // List of "any" transitions, which can be used from any state.
    private List<Transition> _anyTransitions = new List<Transition>();

    // Empty list of transitions, used when we transition to state with no transitions.
    // Eg. if there are 1000s of state machines, we can just transition to this, no more memory created.
    private readonly static List<Transition> EmptyTransitions = new List<Transition>(0);

    // Called once per frame, by an Update() function.
    // Checks if we should transition, and ticks for the current state.
    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            SetState(transition.To);
        }

        _currentState?.Tick();
    }

    // Set a new state
    public void SetState(IState state)
    {
        if (state == _currentState)
        {
            return;
        }
      
        _currentState?.OnExit();

        _currentState = state;

        // Get the active transitions.
        _allTransitionsDict.TryGetValue(_currentState.GetType(), out _activeTransitions);
        if (_activeTransitions == null)
        {
            _activeTransitions = EmptyTransitions;
        }
        
        _currentState.OnEnter();
    }

    // Adds a transition
    // Transition from one state to another state, based on a function "predicate"
    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        // If we don't already have a list of transitions for this from state in the map, create a new list
        if (_allTransitionsDict.TryGetValue(from.GetType(), out List<Transition> transitions) == false)
        {
            transitions = new List<Transition>();
            _allTransitionsDict[from.GetType()] = transitions;
        }

        // Then add the new transition.
        transitions.Add(new Transition(to, predicate));
    }

    // 
    public void AddAnyTransition(IState to, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(to, predicate));
    }

    // A transition goes to a state under a condition.
    // NOTE: There is no real sense of priority.
    private class Transition
    {
        // Condition under which we transition to the "To" state
        public Func<bool> Condition { get; }
        // Where this transitions to
        public IState To { get; }

        // Constructor.
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    // Try to get a transition.
    // NOTE: "Any transitions" are prioritised.
    // If no transition's condition is satisfied, return null.
    private Transition GetTransition()
    {
        // First, check the any transition.
        foreach (var transition in _anyTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }

        // Then check standard transitions.
        foreach (var transition in _activeTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }

        return null;
    }
}