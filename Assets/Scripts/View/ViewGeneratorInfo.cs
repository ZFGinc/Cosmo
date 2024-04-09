using UnityEngine;

[RequireComponent(typeof(Miner))]
public class ViewGeneratorInfo : ViewWithSpeedText<ElecticGenerator>
{
    private void OnEnable()
    {
        _machine.UpdateView += OnUpdateView;
        _machine.UpdateElectricityView += OnUpdateElectricityView;
    }

    private void OnDisable()
    {
        _machine.UpdateView -= OnUpdateView;
        _machine.UpdateElectricityView -= OnUpdateElectricityView;
    }

    private void OnDestroy()
    {
        _machine.UpdateView -= OnUpdateView;
        _machine.UpdateElectricityView -= OnUpdateElectricityView;
    }
}