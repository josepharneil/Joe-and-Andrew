using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateTransition
{
    // Condition under which we transition to the "To" state
    public Func<bool> Condition { get; }
    // Where this transitions to
    public BaseState To { get; }

    // Constructor.
    public StateTransition(BaseState to, Func<bool> condition)
    {
        To = to;
        Condition = condition;
    }
}
