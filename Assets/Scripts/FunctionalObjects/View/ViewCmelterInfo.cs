using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Miner))]
public class ViewSmelterInfo : View
{
    [Space]
    [SerializeField] private TMP_Text _textSmelterNamePlusLevel;
    [SerializeField] private TMP_Text _textNewItemName;
    [SerializeField] private TMP_Text _textSpeedSmelter;
    [SerializeField] private Slider _slederCurrentElectricity;
    [SerializeField] private Image _imageMiningItemIcon;

    private Miner _miner;

    private MinerInfo _minerInfo;
    private MinedItem _minedItem;
    private uint _electricity;

    private void Awake()
    {
        _miner = GetComponent<Miner>();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (_minedItem == null || _minerInfo == null) HideView();
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

    private void UpdateView(MinerInfo minerInfo, MinedItem minedItem, uint electricity)
    {
        _minerInfo = minerInfo;
        _minedItem = minedItem;
        _electricity = electricity;

        ShowView();
    }

    private void UpdateElectricityView(uint electricity, uint copacity)
    {
        _electricity = electricity;

        _slederCurrentElectricity.maxValue = copacity;
        _slederCurrentElectricity.value = electricity;
    }

    private void ShowView()
    {
        _slederCurrentElectricity.maxValue = _minerInfo.ElectricityCopacity;
        _slederCurrentElectricity.value = _electricity;

        _textSmelterNamePlusLevel.text = $"{_minerInfo.Name} (ур.{_minerInfo.Level})";

        if (_minedItem == null || _minerInfo == null) return;

        _textNewItemName.text = $"{_minedItem.Name}";
        _textSpeedSmelter.text = $"{_minerInfo.SpeedMining} / мин.";

        _imageMiningItemIcon.gameObject.SetActive(true);
        _imageMiningItemIcon.sprite = _minedItem.Icon;
    }

    private void HideView()
    {
        _slederCurrentElectricity.maxValue = _minerInfo.ElectricityCopacity;
        _slederCurrentElectricity.value = _electricity;

        _textNewItemName.text = "ќжидание добычи";
        _textSpeedSmelter.text = "? / мин.";

        _imageMiningItemIcon.gameObject.SetActive(false);
    }
}