using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_DoorScript : MonoBehaviour
{

    [Header("Config")]
    [SerializeField] private DoorType doorType;
    [SerializeField] private RL_EnemyManager enemyManager;
    [SerializeField] private RL_Timer timer;
    public enum DoorType
    {
        Enemy,
        Timer,
    }
    // Start is called before the first frame update
    void Start()
    {
        if(doorType == DoorType.Enemy)
        {
            enemyManager = RL_EnemyManager.Instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (doorType)
        {
            case DoorType.Enemy:
                EnemyDoor();
                break;
            case DoorType.Timer:
                TimerDoor();
                break;
        }

        
    }

    void EnemyDoor()
    {
        if (enemyManager.transform.childCount == 0f)
        {
            Destroy(gameObject);
        }
    }

    void TimerDoor()
    {
        if(timer.timerState == RL_Timer.TimerState.Completed)
        {
            Destroy(gameObject);
        }
    }
}
