using System.Collections.Generic;
using UnityEngine;

public sealed class LoaderItemsFromResources : MonoBehaviour
{
    [field: SerializeField] public List<Item> _items { get; private set; }
    [field: SerializeField] public List<MinedItem> _minedItems { get; private set; }
    [field: SerializeField] public List<Recipe> _recipes { get; private set; }

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

    public Item GetItemByName(string name)
    {
        foreach (var item in _items)
        {
            if (item.name == name) return item;
        }

        return null;
    }

    public MinedItem GetMinedItemByType(MinedItemType type)
    {
        foreach (var item in _minedItems)
        {
            if (item.Type == type) return item;
        }

        return null;
    }

    public Recipe GetRecipeByEndingItem(Item item)
    {
        foreach(var recipe in _recipes)
        {
            if(recipe.Item.name == item.name) return recipe;
        }

        return null;
    }

    public Recipe GetRecipeByNeedItems(List<Item> needItems)
    {
        foreach (var recipe in _recipes)
        {
            if (new HashSet<Item>(recipe.NeedItems).SetEquals(needItems)) return recipe;
        }

        return null;
    }
}
