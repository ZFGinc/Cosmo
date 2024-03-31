using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class Miner : MonoBehaviour
{
    [SerializeField] private float _countOreMined = 0;
    [SerializeField] private MineInfo _info;
    [SerializeField] private Transform _productionAreaPivot;

    private PickableObject _thisPickableObject;
    private bool _isMined;
    private float _countOres = 0;
    private TypeOre _typeOre = TypeOre.Null;

    private void Awake()
    {
        _thisPickableObject = GetComponent<PickableObject>();
    }

    private void FixedUpdate()
    {
        if(_thisPickableObject.IsHold) TryStopMine();
        else TryStartMine();
    }

    private void GetOre()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_productionAreaPivot.position, _info.RadiusMine);
        Dictionary<TypeOre, int> countOres = new Dictionary<TypeOre, int>();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out Ore ore))
            {
                TypeOre type = ore.GetTypeOre();

                if(countOres.ContainsKey(type)) countOres[type]++;
                else countOres.Add(type, 1);
            }
        }

        if (countOres.Count > 0)
        {
            var maxOres = countOres.Aggregate((x, y) => x.Value > y.Value ? x : y);
            _countOres = maxOres.Value;
            _typeOre = maxOres.Key;
        }
        else
        {
            _countOres = 0;
            _typeOre = TypeOre.Null;
        }
    }

    private void TryStartMine()
    {
        if(_isMined) return;

        GetOre();

        if (_countOres == 0 || _typeOre == TypeOre.Null) return;

        StartMine();
    }

    private void TryStopMine() 
    {
        if (!_isMined) return;
        StopMine();
    }

    private void StartMine()
    {
        _isMined = true;
        StartCoroutine(Mine(_info.SpeedMine, _countOres));
    }

    private void StopMine()
    {
        _isMined = false;
        StopCoroutine(Mine(_info.SpeedMine, _countOres));
    }

    private IEnumerator Mine(float countPerMinute, float countOres)
    {
        float time = 60 / countPerMinute; //value per minute

        while (_isMined)
        {
            yield return new WaitForSeconds(time);
            _countOreMined += countOres;
        }
    }
}
