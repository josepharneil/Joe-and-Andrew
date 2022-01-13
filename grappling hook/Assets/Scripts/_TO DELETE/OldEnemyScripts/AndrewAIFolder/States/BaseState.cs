using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using bits of https://www.youtube.com/watch?v=YdERlPfwUb0&t=1662s
public abstract class BaseState:MonoBehaviour
{ 
    public abstract Type Tick();
    //currently my thinking is that the int here be a priority tag
    public Dictionary<StateTransition,int> transitions = new Dictionary<StateTransition, int>();
    
}
