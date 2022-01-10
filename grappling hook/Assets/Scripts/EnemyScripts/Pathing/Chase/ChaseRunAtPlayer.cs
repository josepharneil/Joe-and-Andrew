using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseRunAtPlayer : ChasePathing
{
    
    //Same as the patrol state, this gets called in the ChaseAction
    public override void UpdateChase()
    {
        GoTowardsAtSpeed(playerTransform.position, moveSpeed);
    }
}
