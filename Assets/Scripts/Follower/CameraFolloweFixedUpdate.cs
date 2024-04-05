﻿using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraFollowerFixedUpdate: CameraFollower
{
    protected new void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
        SetCameraZoom();
    }
}
