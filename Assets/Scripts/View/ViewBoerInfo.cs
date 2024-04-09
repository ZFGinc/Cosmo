using UnityEngine;

[RequireComponent(typeof(Boer))]
public class ViewBoerInfo : ViewWithProgressBarAndSpeedText<Boer>
{
    private void OnEnable()
    {
        _machine.UpdateView += OnUpdateView;
        _machine.UpdateElectricityView += OnUpdateElectricityView;
        _machine.ResetProgress += OnResetProgress;
    }

    private void OnDisable()
    {
        _machine.UpdateView -= OnUpdateView;
        _machine.UpdateElectricityView -= OnUpdateElectricityView;
        _machine.ResetProgress -= OnResetProgress;
    }

    private void OnDestroy()
    {
        _machine.UpdateView -= OnUpdateView;
        _machine.UpdateElectricityView -= OnUpdateElectricityView;
        _machine.ResetProgress -= OnResetProgress;
    }
}