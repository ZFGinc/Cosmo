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
    
    private List<WireConnections> _connections;
    private bool _isWireConnected = false;

    private void Start()
    {
        OnMined?.Invoke(InfoView);
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

        if(!isActiveAndEnabled) 
            GetWireConnections();
    }

    private void GetWireConnections()
    {
        _connections = new List<WireConnections>();

        _isWireConnected = true;
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

    public override bool IsHasProductCopacity()
    {
        return IsElectricityFull;
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
