using UnityEngine;

public class PickUpController: MonoBehaviour
{
    [SerializeField] private Transform _pickUpCheckerPivot;

    private IPickable _pickableObject;
    private float _pickUpRadius = 0.2f;

    public void TryPickUpObject()
    {
        if (_pickableObject != null) DropObject();
        else PickUpObject();
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

