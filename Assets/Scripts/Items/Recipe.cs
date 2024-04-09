using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Items/New Recipe")]
public class Recipe : ScriptableObject
{
    [field: SerializeField] public List<Item> NeedItems {  get; private set; }
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField, Min(1)] public uint CountProductedItems { get; private set; } = 1;
    [field: SerializeField] public TypeRecipeUser RecipeUser { get; private set; }
}

public enum TypeRecipeUser: byte
{
    All = 0,
    Smelter = 1,
    Crafter = 2,
    DieCutter = 3,
    MultiPress = 4
}
