using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour, IPickable
{
    private bool _isHold = false;
    private float _speedMove = 5f;
    private float _speedRotation = 300f;
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ControllRigidBody(_isHold);

        if (!_isHold) return;

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

    private void ControllRigidBody(bool value)
    {
        _rigidBody.isKinematic = value;
        _rigidBody.detectCollisions = !value;
    }

    public void SetParent(Transform parent)
    {
        _isHold = false;

        this.gameObject.transform.parent = parent;

        if (parent == null) return;

        _isHold = true; 
    }
}

