using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Follower))]
public class CircleSync : MonoBehaviour
{
    [SerializeField] private Material _wallMaterial;
    [SerializeField] private LayerMask _layerMask;

    public static int PosId = Shader.PropertyToID("_playerPosition");
    public static int SizeId = Shader.PropertyToID("_Size");

    private Camera _camera;
    private Follower _follower;
    private Transform _target;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _follower = GetComponent<Follower>();
    }

    private void Update()
    {
        _target = _follower.GetTarget();

        var direction = _camera.transform.position - _target.transform.position;
        var ray = new Ray(_target.transform.position, direction.normalized);
        var view = _camera.WorldToScreenPoint(_target.transform.position);

        if(Physics.Raycast(ray, 3000, _layerMask))
        {
            _wallMaterial.SetFloat(SizeId, 1);
        }
        else
        {
            _wallMaterial.SetFloat(SizeId, 0);
        }

        _wallMaterial.SetVector(PosId, view);
    }
}
