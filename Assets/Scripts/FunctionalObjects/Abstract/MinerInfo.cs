using UnityEngine;

[CreateAssetMenu(fileName = "MineInfo", menuName = "Objects/New MineInfo")]
public class MinerInfo : MachineInfo
{
    [field: SerializeField] public float RadiusMining { get; private set; }
}