using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexGridSettings", menuName = "HexGrid/New HexGridSettings")]
public class HexTileGenerationSettings: ScriptableObject
{
    [field: SerializeField, Min(1), MaxValue(50)] public float HexSize {get; private set;}
    [field: SerializeField] public Vector3 GenerationOffset { get; private set; }
    [field: SerializeField, ReorderableList] public List<TileDictionary> TilePrefabs { get; private set; } = new();

    public GameObject GetTile(TileType tileType)
    {
        foreach (var item in TilePrefabs)
        {
            if(item.Type == tileType) return item.Prefab;
        }

        return null;
    }

    [Serializable]
    public struct TileDictionary
    {
        public TileType Type;
        public GameObject Prefab;
    }
}
