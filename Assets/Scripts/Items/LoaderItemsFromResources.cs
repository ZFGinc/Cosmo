using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoaderItemsFromResources : MonoBehaviour
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

    public Recipe GetRecipeByNeedItems(List<Item> needItems, TypeRecipeUser recipeUser)
    {
        if(needItems.Count == 0) return null;

        needItems.Sort();
        foreach (var recipe in _recipes)
        {
            if (recipe.RecipeUser != recipeUser) continue;

            recipe.NeedItems.Sort();
            if (needItems.SequenceEqual(recipe.NeedItems)) return recipe;
        }

        return null;
    }
}
