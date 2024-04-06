using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Items/New Recipe")]
public class Recipe : ScriptableObject
{
    [field: SerializeField] public List<Item> NeedItems {  get; private set; }
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField, Min(1)] public uint CountProductedItems { get; private set; } = 1;
}
