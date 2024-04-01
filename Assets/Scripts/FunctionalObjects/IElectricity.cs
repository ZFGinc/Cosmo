internal interface IElectricity
{
    public uint Electricity { get; }
    public uint ElectricityCopacity { get; }
    public bool TryApplyElectricity(uint value);
}