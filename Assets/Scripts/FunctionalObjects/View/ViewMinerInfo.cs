using UnityEngine;
using TMPro;
using System.Linq;
using System;

[RequireComponent(typeof(Miner))]
public class ViewMinerInfo : View
{
    [Space]
    [SerializeField] private TMP_Text _nameandLeveMinerText;
    [SerializeField] private TMP_Text _typeProductMiningText;
    [SerializeField] private TMP_Text _speedMiningText;

    private string _nameMiner = "";
    private uint _levelMiner = 0;
    private uint _currentProductCount = 0;
    private uint _maxProductCount = 0;
    private ProductType _productType = ProductType.Null;

    private Miner _miner;

    private void Awake()
    {
        _miner = GetComponent<Miner>();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void OnEnable()
    {
        _miner.OnMined += UpdateInfo;
    }

    private void OnDisable()
    {
        _miner.OnMined -= UpdateInfo;
    }

    private void OnDestroy()
    {
        _miner.OnMined -= UpdateInfo;
    }

    private void UpdateInfo(MinerInfoView info)
    {
        _nameMiner = info.NameMiner;
        _levelMiner = info.LevelMiner;
        _currentProductCount = info.CurrentProductCount;
        _maxProductCount = info.MaximumProductCount;
        _productType = info.ProductType;

        ShowView();
    }

    private void ShowView()
    {
        _nameandLeveMinerText.text = $"{_nameMiner} (ур.{_levelMiner})";
        _typeProductMiningText.text = GetFullNameProduct(_productType.ToString());
        _speedMiningText.text = $"{_currentProductCount} / {_maxProductCount}";
    }

    private string GetFullNameProduct(string value)
    {   
        return string.Concat(value.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }
}