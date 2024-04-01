using UnityEngine;

[CreateAssetMenu(fileName = "OreInfo", menuName = "Infos/Ore/New OreInfo")]
public class OreInfo: ScriptableObject
{
    [field: SerializeField] public ProductType Type { get; private set; }
    [field: SerializeField] public GameObject Model { get; private set; }
}

