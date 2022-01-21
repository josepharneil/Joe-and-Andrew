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
}
