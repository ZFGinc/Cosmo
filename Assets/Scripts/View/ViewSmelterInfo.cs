using UnityEngine;

[RequireComponent(typeof(Smelter))]
public class ViewSmelterInfo : ViewWithProgressBar<Smelter>
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