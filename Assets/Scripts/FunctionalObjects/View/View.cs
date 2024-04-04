using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _canvas;
    [SerializeField] private float _radiusShowUINearPlayer = 5f;
    [SerializeField] private float _speedEnableUI = 5f;

    private bool _enabledUI = false;
    private Vector2 _minMaxSizeUI = new Vector2(0, 1);
    private RectTransform _rectTransformCanvas;

    protected void Start()
    {
        if (_canvas == null) return;
        _rectTransformCanvas = _canvas.GetComponent<RectTransform>();
    }

    protected void FixedUpdate()
    {
        LookAtCamera();
        ShowUINearPlayer();

        float tempSize = Mathf.Lerp(_rectTransformCanvas.localScale.x, (_enabledUI ? 1 : 0), Time.fixedDeltaTime * _speedEnableUI);
        _rectTransformCanvas.localScale = new Vector3(tempSize, 1, 1);

        if (!_enabledUI && tempSize <= 0.02) _canvas.gameObject.SetActive(false);
        else _canvas.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radiusShowUINearPlayer);
    }

    private void LookAtCamera()
    {
        _canvas.LookAt(new Vector3(_canvas.position.x, _camera.position.y, _camera.position.z));
        _canvas.Rotate(0, 180, 0);
    }

    private void ShowUINearPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radiusShowUINearPlayer);

        _enabledUI = false;
        foreach (Collider collider in hitColliders)
        {
            if(collider.gameObject.TryGetComponent(out Character character))
            {
                _enabledUI = true; 
                break;
            }
        }
    }
}
