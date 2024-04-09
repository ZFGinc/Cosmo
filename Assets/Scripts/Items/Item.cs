using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New Item")]
public class Item : ScriptableObject, IComparable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
    [field: SerializeField, ShowAssetPreview] public ItemObject Prefab { get; private set; }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        Item item = obj as Item;

        if (item != null)
            return this.Name.CompareTo(item.Name);
        else
            throw new ArgumentException("Object is not a Item");
    }
}
