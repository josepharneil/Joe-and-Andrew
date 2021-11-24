using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RL_TimerTrigger : MonoBehaviour
{

    public RL_Timer timer;
    void OnTriggerEnter2D(Collider2D col)
    {
        timer.StartTimer();
    }
}
