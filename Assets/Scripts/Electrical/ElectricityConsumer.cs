using UnityEngine;

public abstract class ElectricityConsumer : MonoBehaviour, IConsumer
{
    private uint _electricityCopacity = 10;
    protected uint _electricity;

    protected bool IsHaveElectricity => _electricity > 0;
    protected bool IsElectricityFull => _electricity >= _electricityCopacity;

    protected void SetElectricityCopacity(uint copacity) => _electricityCopacity = copacity;

    protected virtual bool TryUsageElectricity(uint value)
    {
        if (!IsHaveElectricity) return false;
        if (_electricity - value <= 0) return false;

        _electricity -= value;
        return true;
    }

    public virtual bool TryApplyElectricity(uint value)
    {
        if (IsElectricityFull) return false;
        if (_electricity + value > _electricityCopacity) return false;

        _electricity += value;
        return true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
