using UnityEngine;

[RequireComponent(typeof(Crafter))]
public class ViewCrafterInfo : ViewWithProgressBar<Crafter>
{
    private void OnEnable()
    {
        _machine.OnUpdateView += OnUpdateView;
        _machine.OnUpdateElectricityView += OnUpdateElectricityView;
        _machine.OnResetProgress += OnResetProgress;
    }

    private void OnDisable()
    {
        _machine.OnUpdateView -= OnUpdateView;
        _machine.OnUpdateElectricityView -= OnUpdateElectricityView;
        _machine.OnResetProgress -= OnResetProgress;
    }

    private void OnDestroy()
    {
        _machine.OnUpdateView -= OnUpdateView;
        _machine.OnUpdateElectricityView -= OnUpdateElectricityView;
        _machine.OnResetProgress -= OnResetProgress;
    }
}