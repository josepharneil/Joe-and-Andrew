using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public void OnEnter()
    {
        //do something
    }

    public override Type Tick()
    {
        Debug.Log("I am chasing the player");
        return null;
    }

}
