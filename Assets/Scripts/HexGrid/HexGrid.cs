using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField, Min(2)] private int _gridRadius = 2;
    [Space]
    [SerializeField, Expandable] private HexTileGenerationSettings _settings;

    private Dictionary<Vector2, GameObject> _cachedObjects = new Dictionary<Vector2, GameObject>();

    private float _hexSize => _settings.HexSize;

    private Vector3 GetHexCoords(int x, int y)
    {
        bool shouldOffset = (x % 2) == 0;
        float width = 2f * _hexSize;
        float height = Mathf.Sqrt(3) * _hexSize;

        float horizontalDistance = width * (3f / 4f);
        float verticalDistance = height;

        float offset = shouldOffset ? height / 2 : 0;
        float xPosition = x * horizontalDistance;
        float yPosition = (y * verticalDistance) - offset;

        return new Vector3(xPosition, 0, -yPosition);
    }

    private void Clear()
    {
        List<GameObject> childrens = new List<GameObject>();

        for(int i = 0; i < transform.childCount; i++) 
        {
            GameObject child = transform.GetChild(i).gameObject;
            childrens.Add(child);
        }

        foreach(GameObject child in childrens)
        {
            DestroyImmediate(child, true);
        }
    }

    [Button("Generate Grid")]
    private void LayoutAppend()
    {
        for(int y = -_gridRadius; y < _gridRadius; y++)
        {
            for(int x = -_gridRadius; x < _gridRadius; x++)
            {
                if (_cachedObjects.ContainsKey(new Vector2(x, y))) continue;

                GameObject tile = _settings.GetTile((TileType)Random.Range(0, 3));

                Vector3 hexCoords = GetHexCoords(x, y);
                Vector3 position = new Vector3(_settings.GenerationOffset.x + hexCoords.x, _settings.GenerationOffset.y, _settings.GenerationOffset.z + hexCoords.z);

                if (Mathf.Abs(Vector3.Distance(position, transform.position+_settings.GenerationOffset)) > _gridRadius*_hexSize) continue;

                tile.transform.localScale = Vector3.one * _hexSize*2f;

                var obj = Instantiate(tile, position, Quaternion.identity, transform);
                obj.name = $"Q:{x}; R:{y};";

                _cachedObjects.Add(new Vector2(x, y), obj);
            }
        }
    }

    [Button("Regenerate Grid")]
    private void LayoutUpdate()
    {
        Clear();
        _cachedObjects.Clear();
        LayoutAppend();
    }
}
