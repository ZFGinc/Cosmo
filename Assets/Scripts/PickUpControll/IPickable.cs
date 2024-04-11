using UnityEngine;

public interface IPickable
{
    GameObject This();
    void SetParent(Transform parent);
    void SetPickUpController(PickUpController controller);
    bool IsHold { get; }
    bool IsCanPickup { get; }
}

