using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class WindGenerator : Miner
{
    [SerializeField] private Transform _rotationVisualMined;
    [SerializeField] private Vector3 _vectorRotationVisual;
    [SerializeField] private LineRenderer _wire;

    public override event Action<MinerInfoView> OnMined;
    
    private List<WireConnections> _connections = new List<WireConnections>();
    private bool _isWireConnected = false;

    private void Start()
    {
        OnMined?.Invoke(InfoView);

        StartCoroutine(EnergyDistribution());
    }

    private void LateUpdate()
    {
        if (ThisPickableObject.IsHold)
        {
            _wire.positionCount = 0;
            _isWireConnected = false;
        }

        if (IsMined && IsHasProductCopacity()) 
            _rotationVisualMined.Rotate(_vectorRotationVisual);

        if(!_isWireConnected) 
            GetWireConnections();
    }

    private void GetWireConnections()
    {
        _connections = new List<WireConnections>();

        //
        //find consumers 0~~~~
        //

        ViewWireConnections();

        _isWireConnected = true;
    }

    private void ViewWireConnections()
    {
        int index = 0;
        _wire.positionCount = 1;
        _wire.SetPosition(index, transform.position);

        foreach(WireConnections wire in _connections)
        {
            for(int j = 0; j < wire.ConnectionOrderList.Count; j++)
            {
                index++;
                AddWirePosition(index, wire.ConnectionOrderList[j]);
            }

            for (int j = wire.ConnectionOrderList.Count-1; j >= 0; j--)
            {
                index++;
                AddWirePosition(index, wire.ConnectionOrderList[j]);
            }
        }
    }

    private void AddWirePosition(int index, Vector3 position)
    {
        _wire.positionCount++;
        _wire.SetPosition(index, position);
    }

    private IEnumerator EnergyDistribution()
    {
        while (true)
        {
            foreach (var connection in _connections)
            {
                if (!IsHaveElectricity) continue;

                bool result = connection.Consumer.TryApplyElectricity(1);
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
}
