using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RL_TimerTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Start,
        Checkpoint,
        End,
    }

    [Header("Config")]
    public TriggerType triggerType;
    public RL_Timer timer;

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (triggerType)
        {
            case TriggerType.Start:
                timer.StartTimer();
                break;
            case TriggerType.End:
                timer.EndTimer();
                break;
                //AK: currently don't have anything for a checkpoint, could be something good to add in future
        }
        
    }
}
