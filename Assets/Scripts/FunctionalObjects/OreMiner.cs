using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class OreMiner : Miner, IElectricity
{
    [SerializeField] private Transform _productionAreaPivot;
    private uint _countOres = 0;

    public uint Electricity { get; private set; }
    public uint ElectricityCopacity { get; private set; }

    public override event Action<MinerInfoView> OnMined;

    private void GetOre()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_productionAreaPivot.position, Info.RadiusMine);
        Dictionary<ProductType, uint> countOres = new Dictionary<ProductType, uint>();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out Ore ore))
            {
                ProductType type = ore.GetTypeOre();

                if(countOres.ContainsKey(type)) countOres[type]++;
                else countOres.Add(type, 1);
            }
        }

        if (countOres.Count > 0)
        {
            var maxOres = countOres.Aggregate((x, y) => x.Value > y.Value ? x : y);
            _countOres = maxOres.Value;
            ProductType = maxOres.Key;
        }
        else
        {
            _countOres = 0;
            ProductType = ProductType.Null;
        }
    }

    public override void TryStartMine()
    {
        if (IsMined) return;
        if (!IsHasProductCopacity()) return;
        if (Electricity == 0) return;

        GetOre();

        if (_countOres == 0 || ProductType == ProductType.Null) return;

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
        StartCoroutine(Mine(Info.SpeedMine, _countOres));
    }

    public override void StopMine()
    {
        IsMined = false;
        StopCoroutine(Mine(Info.SpeedMine, _countOres));
    }

    public override IEnumerator Mine(uint countPerMinute, uint countOres)
    {
        float time = 60 / countPerMinute; //value per minute

        while (IsMined && Electricity>0 && IsHasProductCopacity())
        {
            yield return new WaitForSeconds(time);
            CurrentProductCount += countOres;
            Electricity--;

            if (!IsHasProductCopacity())
            {
                CurrentProductCount = Info.MaxProductCount;
            }

            OnMined?.Invoke(InfoView);
        }
    }

    public override IEnumerator Mine(uint countPerMinute)
    {
        yield return null;
    }

    public bool TryApplyElectricity(uint value)
    {
        if (Electricity == ElectricityCopacity) return false;
        if (Electricity + value > ElectricityCopacity) return false;

        Electricity += value;
        return true;
    }
}
