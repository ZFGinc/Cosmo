using UnityEngine;

[CreateAssetMenu(fileName = "MinedItem", menuName = "Items/New MinedItem")]
public class MinedItem : Item
{
    [field: SerializeField] public MinedItemType Type { get; private set; }
}
