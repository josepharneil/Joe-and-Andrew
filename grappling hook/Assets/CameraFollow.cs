using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //copied from https://generalistprogrammer.com/unity/unity-2d-how-to-make-camera-follow-player/

    public Transform followTransform;

    // Start is called before the first frame update
   

    
    void LateUpdate()
    {
        this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);
    }
}
