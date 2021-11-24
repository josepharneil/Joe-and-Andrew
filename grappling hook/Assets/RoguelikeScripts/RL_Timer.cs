using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RL_Timer : MonoBehaviour
{
    //from https://vionixstudio.com/2020/11/03/unity-timer-simplified/

    [Header("Config")]
    float duration = 30.0f;
    
    public Text displayText;
    public TimerState timerState = TimerState.NotYetActive;

    public enum TimerState
    {
        NotYetActive,
        Start,
        Running,
        Finished,
    }

    void Start()
    {
        displayText.enabled = false;
    }

    public void StartTimer()
    {
        StartCoroutine("RunTimer");
    }

    IEnumerator RunTimer()
    {
        float countDown = duration;
        displayText.enabled = true;
        while(countDown >= 0)
        {
            timerState = TimerState.Running;
            float f = countDown;
            displayText.text = f.ToString();
            countDown -= Time.deltaTime;
            yield return null;
        }
        timerState = TimerState.Finished;
        displayText.enabled = false;
    }
}
