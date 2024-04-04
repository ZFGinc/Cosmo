using UnityEngine;

public class PickUpController: MonoBehaviour
{
    [SerializeField] private Transform _pickUpCheckerPivot;
    [SerializeField] private float _pickUpRadius = 0.2f;
    [SerializeField] private float _pushForce = 2f;

    private IPickable _pickableObject;

    public void TryPickUpObject()
    {
        if (_pickableObject != null) DropObject();
        else PickUpObject();
    }

    public void TryPushObject(Vector3 direction)
    {
        if (_pickableObject == null) return;

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_pickUpCheckerPivot.position, _pickUpRadius);
    }

    private void DropObject()
    {
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
                break;
            }
        }
    }
}

