using UnityEngine;

[CreateAssetMenu(fileName = "MineInfo", menuName = "Infos/FunctionalObjects/New MineInfo")]
public class MineInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public uint Level { get; private set; }
    [field: SerializeField] public float SpeedMine { get; private set; }
    [field: SerializeField] public float RadiusMine { get; private set; }
}