using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class OreMiner : Miner
{
    [Space]
    [SerializeField] private Transform _productionAreaPivot;
    [SerializeField] private Transform _productionSpawnPoint;

    public override event Action<MinerInfo, MinedItem, uint> UpdateView;

    private uint _countOres = 0;
    private MinedItemType _newMinedItemType = MinedItemType.Null;

    private void Start()
    {
        UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MinerInfo.RadiusMining);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, MinerInfo.RadiusMining*1.5f);
    }

    private void GetOre()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_productionAreaPivot.position, MinerInfo.RadiusMining);
        Dictionary<MinedItemType, uint> countOres = new Dictionary<MinedItemType, uint>();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out Ore ore))
            {
                MinedItemType type = ore.GetMinedItemType();

                if(countOres.ContainsKey(type)) countOres[type]++;
                else countOres.Add(type, 1);
            }
        }

        if (countOres.Count > 0)
        {
            var maxOres = countOres.Aggregate((x, y) => x.Value > y.Value ? x : y);
            _countOres = maxOres.Value;
            _newMinedItemType = maxOres.Key;
        }
        else
        {
            _countOres = 0;
        }
    }

    private bool IsHasFinishedProduct()
    { 
        if (MinedItemType == _newMinedItemType || MinedItemType == MinedItemType.Null)
        {
            MinedItemType = _newMinedItemType;
            return true;
        }

        return false;
    }

    private void GetCountCurrnetItemsAround()
    {
        uint count = 0;

        Collider[] hitColliders = Physics.OverlapSphere(_productionAreaPivot.position, MinerInfo.RadiusMining*1.5f);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out ItemObject item))
            {
                count++;
            }
        }

        CurrentItemsCount = count;
    }

    private void SpawnNewItemAround()
    {
        if (!IsHasProductCopacity()) return;

        var obj = Instantiate(MinedItemInfo.Prefab, _productionSpawnPoint.position, Quaternion.identity);

        if (!IsHasProductCopacity()) Destroy(obj.gameObject);
    }

    public override bool IsHasProductCopacity()
    {
        if (CurrentItemsCount >= MinerInfo.Copacity) return false;

        return true;
    }

    public override void TryStartMine()
    {
        GetCountCurrnetItemsAround();

        if (IsMiningStarted) return;
        if (IsMined) return;

        GetOre();

        if (!IsHasProductCopacity()) return;
        if (!IsHasFinishedProduct()) return;
        if (!IsHaveElectricity) return;

        if (_countOres == 0 || MinedItemType == MinedItemType.Null) return;

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
        StartCoroutine(Mine(MinerInfo.SpeedMining));
    }

    public override void StopMine()
    {
        IsMined = false;
        StopCoroutine(Mine(MinerInfo.SpeedMining));
    }

    public override IEnumerator Mine(uint countPerMinute)
    {
        IsMiningStarted = true;

        float time = 60 / countPerMinute; //value per minute

        while (IsMined && IsHaveElectricity && IsHasProductCopacity() && IsMiningStarted)
        {
            yield return new WaitForSeconds(time);
            if(!IsMined) break;

            if (!IsHasProductCopacity())
            {
                IsMined = false;
                break;
            }

            bool result = TryUsageElectricity(1);
            if (!result)
            {
                IsMined = false;
                break;
            }

            SpawnNewItemAround();

            UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
        }

        IsMiningStarted = false;
    }
}
