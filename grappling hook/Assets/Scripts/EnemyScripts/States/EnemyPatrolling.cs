using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolling : IState
{
    private EnemyPathing pathing;

    public EnemyPatrolling(EnemyPathing pathing)
    {
        this.pathing = pathing;
    }
    public void OnEnter() {}

    public void Tick()
    {
        pathing.UpdatePatrol();
    }
    
    public void FixedTick()
    {
    }

    public void OnExit() {}
}
