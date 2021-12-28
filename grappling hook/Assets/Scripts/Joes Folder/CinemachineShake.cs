using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        StartCoroutine(Shake(amplitude, frequency, duration));
    }

    private IEnumerator Shake(float amplitude, float frequency, float duration)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin
            = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);
        
        // todo
        // https://www.youtube.com/watch?v=ACf1I27I6Tk
        // this lerps downwards to 0 instead of setting to 0, which is interesting
        // try that too!
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0f;
    }
}
