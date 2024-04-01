using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class WindGenerator : Miner, IElectricity
{
    [SerializeField] private Transform _rotationVisualMined;
    [SerializeField] private Vector3 _vectorRotationVisual;
    [SerializeField] private LineRenderer _wireless;

    public uint Electricity { get => CurrentProductCount; private set { CurrentProductCount += value; } }
    public uint ElectricityCopacity { get => Info.MaxProductCount; }

    public override event Action<MinerInfoView> OnMined;

    private List<IElectricity> _connectionsElectricity = new List<IElectricity>();

    private void Awake()
    {
        StartCoroutine(DistributionOfElectricity());
    }

    private void Update()
    {
        if (IsMined && IsHasProductCopacity())
        {
            _rotationVisualMined.Rotate(_vectorRotationVisual);
        }

        if(!IsMined && !ThisPickableObject.IsHold)
        {
            GetConnectionsElectricity();
        }
    }

    private void GetConnectionsElectricity()
    {
        _connectionsElectricity = new List<IElectricity>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Info.RadiusMine);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out IElectricity connection))
            {
                _connectionsElectricity.Add(connection);
            }
        }

        _wireless.positionCount = _connectionsElectricity.Count;

        foreach(IElectricity connection in _connectionsElectricity)
        {

        }
    }

    private IEnumerator DistributionOfElectricity()
    {
        while (true)
        {
            foreach(IElectricity connection in _connectionsElectricity)
            {
                if (Electricity == 0) break;
                connection.TryApplyElectricity(1);
                Electricity--;
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

    public bool TryApplyElectricity(uint value)
    {
        if (Electricity == ElectricityCopacity) return false;
        if (Electricity + value > ElectricityCopacity) return false;

        Electricity += value;
        return true;
    }
}
