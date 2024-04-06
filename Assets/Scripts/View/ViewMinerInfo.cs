using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Miner))]
public class ViewMinerInfo : View
{
    [Space]
    [SerializeField] private TMP_Text _textMinerNamePlusLevel;
    [SerializeField] private TMP_Text _textMiningItemName;
    [SerializeField] private TMP_Text _textSpeedMining;
    [Space]
    [SerializeField] private Slider _sliderCurrentElectricity;
    [Space]
    [SerializeField] private Gradient _progressBarGradient;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Image _fillAreaImage;
    [Space]
    [SerializeField] private Image _imageMiningItemIcon;

    private Miner _miner;
    private MinerInfo _minerInfo;
    private MinedItem _minedItem;

    private uint _electricity = 0;
    private float _currentProgress = 0;
    private float _maxProgress = 1;

    private void Awake()
    {
        _miner = GetComponent<Miner>();
        HideView();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        ProgressBar();
    }

    private void OnEnable()
    {
        _miner.UpdateView += UpdateView;
        _miner.UpdateElectricityView += UpdateElectricityView;
    }

    private void OnDisable()
    {
        _miner.UpdateView -= UpdateView;
        _miner.UpdateElectricityView -= UpdateElectricityView;
    }

    private void OnDestroy()
    {
        _miner.UpdateView -= UpdateView;
        _miner.UpdateElectricityView -= UpdateElectricityView;
    }

    private void ProgressBar()
    {
        if (_progressBar == null || !_enabledUI) return;
        _progressBar.value = _currentProgress / _maxProgress;

        if (!_miner.IsMined) return;
        _currentProgress += Time.fixedDeltaTime;

        if (_fillAreaImage == null) return;
        _fillAreaImage.color = _progressBarGradient.Evaluate(_currentProgress / _maxProgress);
    }

    private void UpdateView(MinerInfo minerInfo, MinedItem minedItem, uint electricity)
    {
        _minerInfo = minerInfo;
        _minedItem = minedItem;
        _electricity = electricity;
        _currentProgress = 0;
        _maxProgress = 60 / _minerInfo.SpeedMining;

        ShowView();
    }

    private void UpdateElectricityView(uint electricity, uint copacity)
    {
        _electricity = electricity;

        _sliderCurrentElectricity.maxValue = copacity;
        _sliderCurrentElectricity.value = _electricity;
    }

    private void ShowView()
    {
        _sliderCurrentElectricity.maxValue = _minerInfo.ElectricityCopacity;
        _sliderCurrentElectricity.value = _electricity;

        _textMinerNamePlusLevel.text = $"{_minerInfo.Name} (ур.{_minerInfo.Level})";

        if (_minedItem == null || _minerInfo == null) return;

        _textMiningItemName.text = $"{_minedItem.Name}";
        _textSpeedMining.text = $"{_minerInfo.SpeedMining} / мин.";

        _imageMiningItemIcon.gameObject.SetActive(true);
        _imageMiningItemIcon.sprite = _minedItem.Icon;
    }

    private void HideView()
    {
        if (_minedItem != null) return;

        _textMiningItemName.text = "ќжидание добычи";
        _textSpeedMining.text = "? / мин.";

        _imageMiningItemIcon.gameObject.SetActive(false);
    }
}