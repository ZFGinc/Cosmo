using System.Collections.Generic;
using UnityEngine;

public class LoaderItemsFromResources : MonoBehaviour
{
    [field: SerializeField] public List<Item> _items { get; private set; }
    [field: SerializeField] public List<MinedItem> _minedItems { get; private set; }

    public static LoaderItemsFromResources Instance { get; private set; }

    private void Awake()
    {
        TrySetInstance();
        LoadItems();

        DontDestroyOnLoad(this.gameObject);
    }

    private void TrySetInstance()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void LoadItems()
    {

    }

    public MinedItem GetMinedItemByType(MinedItemType type)
    {
        foreach(var item in _minedItems)
        {
            if(item.Type == type) return item;
        }

        return null;
    }
}
