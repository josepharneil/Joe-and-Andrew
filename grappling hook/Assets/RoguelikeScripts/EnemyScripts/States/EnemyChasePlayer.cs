using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasePlayer : IState
{
    private EnemyPathing pathing;

    public EnemyChasePlayer( EnemyPathing pathing )
    {
        this.pathing = pathing;
    }

    public void OnEnter() {}

    public void Tick()
    {
        pathing.UpdateChasePlayer();
    }

    public void OnExit() {}
}
