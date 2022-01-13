using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class CameraFollow : MonoBehaviour
{
    //copied from https://generalistprogrammer.com/unity/unity-2d-how-to-make-camera-follow-player/

    public Transform followTransform;
    [SerializeField] private float followSpeed = 10f; 
    
    private void LateUpdate()
    {
        Vector3 followPosition = followTransform.position;
        followPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, followPosition, followSpeed*Time.deltaTime);

        //transform.position = new Vector3(followPosition.x, followPosition.y, transform.position.z);
    }

    [SerializeField] private float cameraShakeDuration = 0.2f;
    [SerializeField] private float cameraShakeAmount = 0.2f;

    public void Shake()
    {
        // todo
        //StartCoroutine(_Shake(cameraShakeDuration, cameraShakeAmount));
    }

    private IEnumerator _Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
