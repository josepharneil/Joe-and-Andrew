using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RL_Timer : MonoBehaviour
{
    //from https://vionixstudio.com/2020/11/03/unity-timer-simplified/

    [Header("Config")]
    [SerializeField] private float duration = 30.0f;
    [SerializeField] private float xPositionReset;
    [SerializeField] private float yPositionReset;
    [SerializeField] private Rigidbody2D playerRB;

    public Text displayText;
    public TimerState timerState = TimerState.NotYetActive;


    public enum TimerState
    {
        NotYetActive,
        Running,
        Completed,
        Failed,
    } 

    void Start()
    {
        displayText.enabled = false;
    }

    public void StartTimer()
    {
        if (timerState == TimerState.NotYetActive || timerState== TimerState.Failed)
        {
            timerState = TimerState.Running;
            StartCoroutine("RunTimer");
        }
    }

    public void EndTimer()
    {
        timerState = TimerState.Completed;
    }

    IEnumerator RunTimer()
    {
        float countDown = duration;
        displayText.enabled = true;
        while(countDown >= 0)
        {
            if(timerState == TimerState.Completed)
            {
                break;
            }
            timerState = TimerState.Running;
            float f = countDown;
            displayText.text = f.ToString();
            countDown -= Time.deltaTime;
            yield return null;
        }
        if(timerState  != TimerState.Completed)
        {
            TimerFailed();
        }
        displayText.enabled = false;
    }

    void TimerFailed()
    {
        //AK TODO pretty hacky method heere
        timerState = TimerState.Failed;
        playerRB.position = new Vector3(xPositionReset, yPositionReset, 0f);
    }
}
