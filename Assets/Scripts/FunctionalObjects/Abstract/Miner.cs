using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PickableObject))]
public abstract class Miner : ElectricityConsumer, IMachine
{
    public virtual event Action<MachineInfo, MinedItem, uint> UpdateView;
    public virtual event Action<uint, uint> UpdateElectricityView;

    [SerializeField] private MinerInfo _minerInfo;
    [Space]
    [SerializeField] protected DecalProjector _decalProjector;
    [Space]
    [SerializeField] private bool _isMined;
    [SerializeField] private uint _currentItemsCount;
    [SerializeField] private MinedItemType _minedItemType = MinedItemType.Null;

    private PickableObject _thisPickableObject;

    public MinerInfo MinerInfo => _minerInfo;
    public PickableObject ThisPickableObject => _thisPickableObject;

    public bool IsMiningStarted { get; protected set; } = false;
    public bool IsWorking { get => _isMined; protected set { _isMined = value; } }
    public uint CurrentItemsCount { get => _currentItemsCount; protected set { _currentItemsCount = value; } }
    public MinedItemType MinedItemType { get => _minedItemType; protected set { _minedItemType = value; } }
    public uint Copacity { get => _minerInfo.Copacity; }

    protected void Awake()
    {
        _thisPickableObject = GetComponent<PickableObject>();

        if (_minerInfo == null) throw new Exception("≈блан, ты забыл MinerInfo указать!");
        SetElectricityCopacity(_minerInfo.ElectricityCopacity);

        if (_decalProjector == null) return;

        float diametr = _minerInfo.RadiusMining * 2;
        _decalProjector.size = new Vector3(diametr, diametr, 10);


    }

    protected void FixedUpdate()
    {
        if(_currentItemsCount > _minerInfo.Copacity)
        {
            TryStopMine();
            _currentItemsCount = _minerInfo.Copacity;
        }


        if (_thisPickableObject.IsHold) TryStopMine();
        else TryStartMine();


        _decalProjector.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));

        _decalProjector.gameObject.SetActive(ThisPickableObject.IsHold);

        if (_currentItemsCount == 0 && !_isMined) _minedItemType = MinedItemType.Null;
    }

    public virtual MinedItem MinedItemInfo => LoaderItemsFromResources.Instance.GetMinedItemByType(_minedItemType);

    public virtual bool IsHasProductCopacity()
    {
        if (_currentItemsCount >= _minerInfo.Copacity) return false;

        return true;
    }

    protected override bool TryUsageElectricity(uint value)
    {
        UpdateElectricityView?.Invoke(_electricity, _minerInfo.ElectricityCopacity);

        return base.TryUsageElectricity(value);
    }

    public override bool TryApplyElectricity(uint value)
    {
        UpdateElectricityView?.Invoke(_electricity, _minerInfo.ElectricityCopacity);

        return base.TryApplyElectricity(value);
    }

    public abstract void TryStartMine();

    public abstract void TryStopMine();

    public abstract void StartMine();

    public abstract void StopMine();

    public abstract IEnumerator Mine(uint countPerMinute);
}
