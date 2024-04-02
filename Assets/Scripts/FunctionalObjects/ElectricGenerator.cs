using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ElecticGenerator : Miner, IElectricalGenerator
{
    public override event Action<MinerInfoView> OnMined;
    
    protected List<IConsumer> _connections = new();

    private bool _isGetConsumers = false;

    protected void Start()
    {
        OnMined?.Invoke(InfoView);

        StartCoroutine(EnergyDistribution());
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        if (ThisPickableObject.IsHold)
        {
            _isGetConsumers = false;
            return;
        }

        if (!_isGetConsumers)
        {
            _connections = ElectricalCircuit.Instance.GetConsumers(this);
            _isGetConsumers = true;
        }
    }

    private IEnumerator EnergyDistribution()
    {
        while (true)
        {
            foreach (var connection in _connections)
            {
                if (!IsHaveElectricity) continue;

                bool result = connection.TryApplyElectricity(1);
                if (result) CurrentProductCount--;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public override void TryStartMine()
    {
        if(IsMined) return;
        if (!IsHasProductCopacity()) return;

        StartMine();
    }

    public override void TryStopMine() 
    {
        if (!IsMined) return;
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

        while (IsMined && IsHasProductCopacity())
        {
            yield return new WaitForSeconds(time);
            CurrentProductCount += 1;

            if (!IsHasProductCopacity())
            {
                CurrentProductCount = Info.MaxProductCount;
            }

            OnMined?.Invoke(InfoView);
        }
    }

    public ElecticGenerator GetElectricalGeneratorType()
    {
        return this;
    }
}
