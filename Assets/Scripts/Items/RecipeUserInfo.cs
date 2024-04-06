using UnityEngine;

[CreateAssetMenu(fileName = "RecipeUser", menuName = "Objects/New RecipeUser")]
public class RecipeUserInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public uint Level { get; private set; }
    [field: SerializeField] public uint Copacity { get; private set; }
    [field: SerializeField] public uint SpeedUseRecipe { get; private set; }
    [field: SerializeField] public uint ElectricityCopacity { get; private set; }
}