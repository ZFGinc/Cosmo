using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : NetworkBehaviour, IPickable, IPushObject
{
    [SerializeField] private bool _isCanPush = false;

    [SyncVar] private bool _isHold = false;
    private float _speedMove = 5f;
    private float _speedRotation = 300f;
    private Rigidbody _rigidBody;
    private PickUpController _controller;

    public bool IsHold { get=>_isHold; set => _isHold=value; }
    [field: SerializeField] public bool IsCanPickup { get; set; } = true;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        IsCanPickup = true;
    }

    private void FixedUpdate()
    {
        ControllRigidBody(IsHold);

        if (!IsHold) return;

        MoveToParent();
        RotateToParent();
    }

    private void MoveToParent()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, _speedMove * Time.fixedDeltaTime);
    }

    private void RotateToParent()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, Time.fixedDeltaTime * _speedRotation);
    }

    private void PushObject(Vector3 direction)
    {
        if(_controller != null) _controller.TryPickUpObject(true);
        IsHold = false;
        ControllRigidBody(IsHold);

        _rigidBody.AddForce(direction * 10, ForceMode.Impulse);
    }

    public void ControllRigidBody(bool value)
    {
        _rigidBody.isKinematic = value;
    }

    public void ControllCollisionDetect(bool value)
    {
        _rigidBody.detectCollisions = value;
    }

    public void SetParent(Transform parent)
    {
        IsHold = false;

        this.gameObject.transform.parent = parent;

        if (parent == null) return;

        IsHold = true; 
    }

    public void SetPickUpController(PickUpController controller)
    {
        _controller = controller;
    }

    public void Action(Transform player)
    {
        if (!_isCanPush) return;

        Vector3 direction = transform.position - player.position;
        PushObject(direction);
    }

    public GameObject This() => this.gameObject;
}

