using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraFollower: Follower
{
    [Space]
    [SerializeField, Range(1f,3f)] private float _zoomScale = 1.3f;

    private Camera _camera;
    private float _defaultFOV = 80;

    private readonly Vector2 _minMaxZoomScale = new(1f, 3f);

    protected void Start()
    {
        _camera = GetComponent<Camera>();
    }

    protected void SetCameraZoom()
    {
        _camera.fieldOfView = _defaultFOV / _zoomScale;
    }

    public void ApplyZoomScale(float zoomScale)
    {
        _zoomScale += zoomScale;

        if (_zoomScale < _minMaxZoomScale.x) _zoomScale = _minMaxZoomScale.x;
        if (_zoomScale > _minMaxZoomScale.y) _zoomScale = _minMaxZoomScale.y;
    }
}

