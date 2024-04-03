using UnityEngine;

public interface IConsumer
{
    bool TryApplyElectricity(uint value);
    Vector3 GetPosition();
}