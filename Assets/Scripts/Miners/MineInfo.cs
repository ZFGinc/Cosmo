using UnityEngine;

[CreateAssetMenu(fileName = "MineInfo", menuName = "Infos/FunctionalObjects/New MineInfo")]
public class MinerInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public uint Level { get; private set; }
    [field: SerializeField] public uint Copacity { get; private set; }
    [field: SerializeField] public uint SpeedMining { get; private set; }
    [field: SerializeField] public float RadiusMining { get; private set; }
    [field: SerializeField] public uint ElectricityCopacity { get; private set; }
}