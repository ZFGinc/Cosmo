using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class ElecticGenerator : Miner
{
    public override event Action<MachineInfo, MinedItem, uint> UpdateView;

    protected HashSet<IConsumer> _connections = new();

    protected void Start()
    {
        UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
        
        StartCoroutine(EnergyDistribution());
    }

    protected new void FixedUpdate()
    {
        if (ThisPickableObject.IsHold) TryStopMine();
        else TryStartMine();

        if (!IsHasProductCopacity())
        {
            TryStopMine();
            _electricity = MinerInfo.Copacity;
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
                UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
                TryStartMine();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public override bool IsHasProductCopacity()
    {
        return _electricity < MinerInfo.Copacity;
    }

    public override void TryStartMine()
    {
        if(IsWorking) return;
        if (!IsHasProductCopacity())
        {
            IsWorking = false;
            return;
        }

        StartMine();
    }

    public override void TryStopMine() 
    {
        if (!IsWorking) return;
        if (!IsHasProductCopacity()) return;

        StopMine();
    }

    public override void StartMine()
    {
        IsWorking = true;
        StartCoroutine(Mine(MinerInfo.SpeedWorking));
    }

    public override void StopMine()
    {
        IsWorking = false;
        StopCoroutine(Mine(MinerInfo.SpeedWorking));
    }

    public override IEnumerator Mine(uint countPerMinute)
    {
        float time = 60 / countPerMinute; //value per minute

        while (IsWorking)
        {
            yield return new WaitForSeconds(time);
            if (!IsWorking) break;

            TryApplyElectricity(1);

            if (!IsHasProductCopacity())
            {
                _electricity = MinerInfo.Copacity;
                IsWorking = false;
                UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
                break;
            }

            UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
        }
    }

}
