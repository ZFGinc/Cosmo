using UnityEngine;

public class CameraFollower: Follower
{
    [Space]
    [SerializeField, Range(1f,2f)] private float _zoomScale = 1f;
    [SerializeField] private Camera _camera;

    private float _defaultFOV = 60;
    private float _currentZoom;

    private readonly Vector2 _minMaxZoomScale = new(1f, 2f);

    protected void Start()
    {
        _currentZoom = _zoomScale;
    }

    protected void SetCameraZoom(float deltaTime)
    {
        _currentZoom = Mathf.Lerp(_currentZoom, _zoomScale, deltaTime*2);
        _camera.fieldOfView = _defaultFOV / _currentZoom;
    }

    public void ApplyZoomScale(float zoomScale)
    {
        _zoomScale += zoomScale;

        if (_zoomScale < _minMaxZoomScale.x) _zoomScale = _minMaxZoomScale.x;
        if (_zoomScale > _minMaxZoomScale.y) _zoomScale = _minMaxZoomScale.y;
    }
}

