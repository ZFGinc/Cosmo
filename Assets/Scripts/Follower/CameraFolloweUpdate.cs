using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraFolloweUpdate: CameraFollower
{
    protected new void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        Move(Time.deltaTime);
        SetCameraZoom();
    }
}

