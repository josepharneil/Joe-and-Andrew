using System;
using System.Collections;
using Cinemachine;
using UnityEngine;


[Serializable] public class CameraShakeData
{
    public float Amplitude = 3f;
    public float Frequency = 0.1f;
    public float Duration = 1f;
}

public class CinemachineShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    public void ShakeCamera(CameraShakeData cameraShakeData)
    {
        StartCoroutine(ShakeCoroutine(cameraShakeData));
    }

    private IEnumerator ShakeCoroutine(CameraShakeData cameraShakeData)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin
            = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = cameraShakeData.Amplitude;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = cameraShakeData.Frequency;

        yield return new WaitForSeconds(cameraShakeData.Duration);
        
        // todo
        // https://www.youtube.com/watch?v=ACf1I27I6Tk
        // this lerps downwards to 0 instead of setting to 0, which is interesting
        // try that too!
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 0f;
    }
}
