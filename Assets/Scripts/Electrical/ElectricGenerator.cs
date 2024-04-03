using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ElecticGenerator : Miner
{
    public override event Action<MinerInfoView> OnMined;
    
    protected HashSet<IConsumer> _connections = new();

    public override MinerInfoView InfoView => new MinerInfoView()
    {
        NameMiner = Info.Name,
        LevelMiner = Info.Level,
        CurrentProductCount = _electricity,
        MaximumProductCount = MaximumProductCount,
        ProductType = this.ProductType
    };

    protected void Start()
    {
        OnMined?.Invoke(InfoView);

        StartCoroutine(EnergyDistribution());
    }

    protected new void FixedUpdate()
    {
        if (ThisPickableObject.IsHold) TryStopMine();
        else TryStartMine();

        if (!IsHasProductCopacity())
        {
            TryStopMine();
            _electricity = Info.MaxProductCount;
        }

        _connections = ElectricalCircuit.Instance.GetConsumers(this);
    }

    private IEnumerator EnergyDistribution()
    {
        while (true)
        {
            foreach (var connection in _connections)
            {
                if (!IsHaveElectricity) continue;

                bool result = connection.TryApplyElectricity(1);
                if (!result) continue;

                _electricity--;
                OnMined?.Invoke(InfoView);
                TryStartMine();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public override bool IsHasProductCopacity()
    {
        return _electricity < Info.MaxProductCount;
    }

    public override void TryStartMine()
    {
        if(IsMined) return;
        if (!IsHasProductCopacity())
        {
            IsMined = false;
            return;
        }

        StartMine();
    }

    public override void TryStopMine() 
    {
        if (!IsMined) return;
        if (!IsHasProductCopacity()) return;

        StopMine();
    }

    public override void StartMine()
    {
        IsMined = true;
        StartCoroutine(Mine(Info.SpeedMine));
    }

    public override void StopMine()
    {
        IsMined = false;
        StopCoroutine(Mine(Info.SpeedMine));
    }

    public override IEnumerator Mine(uint countPerMinute, uint countOres)
    {
        yield return null;
    }

    public override IEnumerator Mine(uint countPerMinute)
    {
        float time = 60 / countPerMinute; //value per minute

        while (IsMined)
        {
            yield return new WaitForSeconds(time);
            _electricity += 1;

            if (!IsHasProductCopacity())
            {
                _electricity = Info.MaxProductCount;
                IsMined = false;
                OnMined?.Invoke(InfoView);
                yield break;
            }

            OnMined?.Invoke(InfoView);
        }
    }

}
