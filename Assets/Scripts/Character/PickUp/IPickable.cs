using UnityEngine;

public interface IPickable
{
    void SetParent(Transform parent);
    bool IsHold { get; }
}

