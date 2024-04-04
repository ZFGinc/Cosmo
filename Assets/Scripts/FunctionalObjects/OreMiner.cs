using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class OreMiner : Miner
{
    [SerializeField] private Transform _productionAreaPivot;

    public override event Action<MinerInfoView> OnMined;

    private uint _countOres = 0;
    private ProductType _newProductType = ProductType.Null;

    private void Start()
    {
        OnMined?.Invoke(InfoView);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Info.RadiusMine);
    }

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
            _newProductType = maxOres.Key;
        }
        else
        {
            _countOres = 0;
        }
    }

    private bool IsHasFinishedProduct()
    { 
        if (ProductType == _newProductType || ProductType == ProductType.Null)
        {
            ProductType = _newProductType;
            return true;
        }

        return false;
    }

    public override void TryStartMine()
    {
        if (IsMined) return;

        GetOre();

        if (!IsHasProductCopacity()) return;
        if (!IsHasFinishedProduct()) return;
        if (!IsHaveElectricity) return;


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

        while (IsMined && IsHaveElectricity && IsHasProductCopacity())
        {
            yield return new WaitForSeconds(time);
            CurrentProductCount += countOres;
            TryUsageElectricity(1);

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
}
