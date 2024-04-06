using UnityEngine;

public sealed class PickUpController: MonoBehaviour
{
    [SerializeField] private Transform _pickUpCheckerPivot;
    [SerializeField] private float _pickUpRadius = 0.2f;
    [SerializeField] private float _pushForce = 2f;

    private IPickable _pickableObject;

    public void TryPickUpObject(bool isAction = false)
    {
        if (_pickableObject != null || isAction) DropObject();
        else PickUpObject();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_pickUpCheckerPivot.position, _pickUpRadius);
    }

    private void DropObject()
    {
        if (_pickableObject == null) return;

        _pickableObject.SetParent(null);
        _pickableObject = null;
    }

    private void PickUpObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_pickUpCheckerPivot.position, _pickUpRadius);
        foreach(Collider collider in hitColliders)
        {
            if(collider.gameObject.TryGetComponent(out IPickable pickableObject))
            {
                _pickableObject = pickableObject;
                _pickableObject.SetParent(_pickUpCheckerPivot);
                _pickableObject.SetPickUpController(this);
                break;
            }
        }
    }
}

