using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnToPatrol : IState
{
    private EnemyPathing pathing;

    public EnemyReturnToPatrol( EnemyPathing pathing )
    {
        this.pathing = pathing;
    }

    public void OnEnter() {}

    public void Tick()
    {
        pathing.UpdateReturnToPatrol();
    }

    public void OnExit() {}
}
