using UnityEngine;

public class Ore: MonoBehaviour
{
    [SerializeField] private OreInfo _info;

    public ProductType GetTypeOre() => _info.Type;
}

