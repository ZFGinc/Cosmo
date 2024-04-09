using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.UI;

public abstract class ViewWithProgressBarAndSpeedText<T> : ViewWithProgressBar<T>
    where T : IMachine
{
    [BoxGroup("Параметр текста скорости"), SerializeField] private TMP_Text _textSpeedWorking;

    protected override void ShowView()
    {
        base.ShowView();
        if (_machineInfo == null || _itemInfo == null) return;
        _textSpeedWorking.text = $"{_machineInfo.SpeedWorking} / мин.";
    }

    protected override void HideView()
    {
        base.HideView();
        if (_itemInfo == null) return;
        _textSpeedWorking.text = "? / мин.";
    }

    [Button("Set Miner default settings with Progress Bar & Speed Text")]
    private void SetDefaultSettings()
    {
        SetBaseDefaultSettings();

        if (_canvas == null) return;

        _textSpeedWorking = _canvas.transform.Find("SpeedText").GetComponent<TMP_Text>();

        _progressBar = _canvas.transform.Find("ProgressBar").GetComponent<Slider>();
        if (_progressBar == null) return;

        _fillAreaImage = _progressBar.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }
}