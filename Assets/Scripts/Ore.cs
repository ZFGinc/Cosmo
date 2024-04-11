using UnityEngine;

public class Ore: MonoBehaviour
{
    [SerializeField] private MinedItem _minedItem;

    public MinedItem GetMinedItem() => _minedItem;
    public MinedItemType GetMinedItemType() => _minedItem.Type;
}

