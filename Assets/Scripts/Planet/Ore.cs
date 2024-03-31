using UnityEngine;

public class Ore: MonoBehaviour
{
    [SerializeField] private OreInfo _info;

    public TypeOre GetTypeOre() => _info.Type;
}

