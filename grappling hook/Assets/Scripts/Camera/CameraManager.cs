using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera ActiveCamera;
    public CinemachineShake Shake;

    public static Camera GetActiveCamera()
    {
        return Instance.ActiveCamera;
    }
}