using UnityEngine;

public interface IPickable
{
    void SetParent(Transform parent);
    void SetPickUpController(PickUpController controller);
    bool IsHold { get; }
}

