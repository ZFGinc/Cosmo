using UnityEngine;

public class CameraFollowerUpdate: CameraFollower
{
    protected new void Start()
    {
        base.Start();
    }

    private void Update()
    {
        Move(Time.deltaTime);
        SetCameraZoom(Time.deltaTime);
    }
}

