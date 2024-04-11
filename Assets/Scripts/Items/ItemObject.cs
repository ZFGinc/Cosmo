using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ItemObject : MonoBehaviour
{
    public Item Item => _item;

    [SerializeField] private Item _item;

    private PickableObject _pickableObject;

    private void Awake()
    {
        _pickableObject = GetComponent<PickableObject>();
    }

    public bool IsHold => _pickableObject.IsHold;

    public void DisableHold()
    {
        _pickableObject.IsHold = false;
    }

    public void DisableGravity()
    {
        _pickableObject.ControllRigidBody(true);
    }

    public void EnableGravity()
    {
        _pickableObject.ControllRigidBody(false);
    }

    public void DisableCanPickUp()
    {
        _pickableObject.IsCanPickup = false;
    }

    public void ControllCollisionDetect()
    {
        _pickableObject.ControllCollisionDetect(false);
    }
}
