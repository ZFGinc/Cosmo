using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class Boer : Miner
{
    [Space]
    [SerializeField] private Transform _productionAreaPivot;
    [SerializeField] private Transform _productionSpawnPoint;
    [Space]
    [SerializeField] private Animator _animator;
    [SerializeField, AnimatorParam("_animator")] private int _animatorTriggerStartMining;
    [SerializeField, AnimatorParam("_animator")] private int _animatorTriggerEndMining;

    public override event Action<MinerInfo, MinedItem, uint> UpdateView;
    public event Action ResetProgress;

    private uint _countOres = 0;
    private MinedItemType _newMinedItemType = MinedItemType.Null;

    private void Start()
    {
        UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
        ResetProgress?.Invoke();

        if (_animator == null)
            throw new Exception("≈блан, а че анимировать мне?");
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
        _animator.SetTrigger(_animatorTriggerStartMining);

        int checkCount = 5;
        float time = 60 / countPerMinute; //value per minute
        float timeForCheckIfHerePickUp = time / checkCount;

        while (IsMined && IsHaveElectricity && IsHasProductCopacity() && IsMiningStarted)
        {
            UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);

            for (int i = 0; i < checkCount; i++)
            {
                yield return new WaitForSeconds(timeForCheckIfHerePickUp);
                if (!IsMined) break;
            }
            if (!IsMined)
            {
                ResetProgress?.Invoke();
                break;
            }

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
        }

        IsMiningStarted = false;
        _animator.SetTrigger(_animatorTriggerEndMining);
        UpdateView?.Invoke(MinerInfo, MinedItemInfo, _electricity);
    }
}
