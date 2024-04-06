using UnityEngine;

[RequireComponent (typeof(Camera))]
public sealed class CameraFollowerUpdate: CameraFollower
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

