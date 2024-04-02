using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ElectricityConsumer : MonoBehaviour, IConsumer, IElectricalConnect
{
    [SerializeField] private uint _electricityCopacity;
    private uint _electricity;

    protected bool IsHaveElectricity => _electricity > 0;
    protected bool IsElectricityFull => _electricity >= _electricityCopacity;

    protected bool TryUsageElectricity(uint value)
    {
        if (!IsHaveElectricity) return false;
        if (_electricity - value <= 0) return false;

        _electricity -= value;
        return true;
    }

    public bool TryApplyElectricity(uint value)
    {
        if (IsElectricityFull) return false;
        if (_electricity + value > _electricityCopacity) return false;

        _electricity += value;
        return true;
    }

    public IElectricalConnect TryGetIElectricalConnect()
    {
        return this;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
