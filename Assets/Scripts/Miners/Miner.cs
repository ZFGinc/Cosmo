using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public abstract class Miner : ElectricityConsumer
{
    public abstract event Action<MinerInfoView> OnMined;

    [SerializeField] private MineInfo _info;
    [Space] 
    [SerializeField] private bool _isMined;
    [SerializeField] private uint _currentProductCount;
    [SerializeField] private ProductType _productType = ProductType.Null;

    private PickableObject _thisPickableObject;
    
    public MineInfo Info => _info;
    public PickableObject ThisPickableObject => _thisPickableObject;

    public bool IsMined { get => _isMined; protected set { _isMined = value; } }
    public uint CurrentProductCount { get => _currentProductCount; protected set { _currentProductCount = value; } }
    public uint MaximumProductCount { get => _info.MaxProductCount; }
    public ProductType ProductType { get => _productType; protected set { _productType = value; } }

    private void Start()
    {
        _thisPickableObject = GetComponent<PickableObject>();
    }

    private void FixedUpdate()
    {
        if(_thisPickableObject.IsHold) TryStopMine();
        else TryStartMine();

        if(_currentProductCount > _info.MaxProductCount)
        {
            TryStopMine();
            _currentProductCount = _info.MaxProductCount;
        }
    }

    public MinerInfoView InfoView => new MinerInfoView()
    {
        NameMiner = Info.Name,
        LevelMiner = Info.Level,
        CurrentProductCount = _currentProductCount,
        MaximumProductCount = MaximumProductCount,
        ProductType = this.ProductType
    };

    public virtual bool IsHasProductCopacity()
    {
        if (CurrentProductCount >= Info.MaxProductCount) return false;

        return true;
    }

    public abstract void TryStartMine();

    public abstract void TryStopMine();

    public abstract void StartMine();

    public abstract void StopMine();

    public abstract IEnumerator Mine(uint countPerMinute);

    public abstract IEnumerator Mine(uint countPerMinute, uint countOres);
}
