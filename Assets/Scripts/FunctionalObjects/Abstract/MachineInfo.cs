using UnityEngine;

public abstract class MachineInfo: ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "Machine";
    [field: SerializeField, Min(1)] public uint Level { get; private set; } = 1;
    [field: SerializeField, Min(1)] public uint Copacity { get; private set; } = 1;
    [field: SerializeField, Min(1)] public uint SpeedWorking { get; private set; } = 1;
    [field: SerializeField, Min(5)] public uint ElectricityCopacity { get; private set; } = 5;
}