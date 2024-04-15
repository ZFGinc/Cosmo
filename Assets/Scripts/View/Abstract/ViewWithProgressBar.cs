using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Mirror;

public abstract class ViewWithProgressBar<T> : View<T>
    where T : IMachine
{
    [BoxGroup("Параметры градиента"), SerializeField] private Gradient _progressBarGradient;
    [BoxGroup("Параметры градиента"), SerializeField] protected Slider _progressBar;
    [BoxGroup("Параметры градиента"), SerializeField] protected Image _fillAreaImage;

    [SyncVar] protected float _currentProgress = 0;
    [SyncVar] protected float _maxProgress = 1;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        ProgressBar();
    }

    protected void OnResetProgress() {
        _currentProgress = 0;

        UpdateProgressBarValue();
        UpdateProgressBarColor();
    }

    protected void ProgressBar()
    {
        if (!_machine.IsWorking) return;
        _currentProgress += Time.fixedDeltaTime;

        UpdateProgressBarValue();
        UpdateProgressBarColor();
    }

    protected override void OnUpdateView(MachineInfo machineInfo, Item itemInfo, uint electricity)
    {
        base.OnUpdateView(machineInfo, itemInfo, electricity);

        if (_machineInfo != null)
        {
            _currentProgress = 0;
            _maxProgress = 60 / _machineInfo.SpeedWorking;
        }

        ShowView();
    }

    private void UpdateProgressBarColor()
    {
        if (_fillAreaImage == null) return;

        _fillAreaImage.color = _progressBarGradient.Evaluate(_currentProgress / _maxProgress);
    }
    private void UpdateProgressBarValue()
    {
        if (_progressBar == null || !_enabledUI) return;

        _progressBar.value = _currentProgress / _maxProgress;
    }

    [Button("Set default settings with Progress Bar")]
    private void SetDefaultSettings()
    {
        SetBaseDefaultSettings();

        if (_canvas == null) return;

        _progressBar = _canvas.transform.Find("ProgressBar").GetComponent<Slider>();
        if(_progressBar == null) return;

        _fillAreaImage = _progressBar.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }
}