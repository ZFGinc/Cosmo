using TMPro;
using UnityEngine;
using NaughtyAttributes;

public abstract class ViewWithSpeedText<T> : View<T>
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

    [Button("Set default settings with Speed Text")]
    private void SetDefaultSettings()
    {
        SetBaseDefaultSettings();

        if (_canvas == null) return;

        _textSpeedWorking = _canvas.transform.Find("SpeedText").GetComponent<TMP_Text>();
    }
}