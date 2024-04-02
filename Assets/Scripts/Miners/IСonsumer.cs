public interface IConsumer
{
    bool TryApplyElectricity(uint value);
    IElectricalConnect TryGetIElectricalConnect();
}