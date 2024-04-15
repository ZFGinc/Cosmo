using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Mirror;

[RequireComponent(typeof(PickableObject))]
public abstract class UsingRecipes : ElectricityConsumer, IMachine
{
    [Foldout("Позиции"), SerializeField] private Transform _pivotForCheckItems;
    [Foldout("Позиции"), SerializeField] private Transform _pivotForSpawnNewItem;
    [Space]
    [Foldout("Инфа о данном приборе"), SerializeField, Expandable, SyncVar] private RecipeUserInfo _recipeUserInfo = null;
    [Foldout("Инфа о данном приборе"), SerializeField] private TypeRecipeUser _recipeUser;
    [Space]
    [Foldout("Настройки области для предметов"), SerializeField] private Vector3 _sizeCubeForCheckItems;
    [Foldout("Настройки области для предметов"), SerializeField, Min(0f)] private float _additionalDistanceBetweenItems = 0.5f;

    protected Transform PivotForCheckItems => _pivotForCheckItems;
    protected Transform PivotForSpawnNewItem => _pivotForSpawnNewItem;
    protected MachineInfo RecipeUserInfo => _recipeUserInfo;

    [SyncVar] private bool _isWorking = false;  
    [HideInInspector] public bool IsWorking { get => _isWorking; protected set => _isWorking = value; }

    public abstract event Action<MachineInfo, Item, uint> OnUpdateView;
    public abstract event Action<uint, uint> OnUpdateElectricityView;
    public abstract event Action OnResetProgress;

    protected List<Item> _items = new();
    protected List<ItemObject> _itemObjects = new();
    protected Recipe _currentRecipe;
    protected PickableObject _pickableObject;

    public const float RadiusForCheckItemsAroundPivots = 1f;

    private void Awake()
    {
        _pickableObject = GetComponent<PickableObject>();
    }
    private void Start()
    {
        UpdateView();
        ResetProgress();
    }
    private void FixedUpdate()
    {
        if (_pickableObject.IsHold && !IsWorking)
        {
            if (_itemObjects.Count == 0) return;
            foreach (ItemObject itemObject in _itemObjects) itemObject.EnableGravity();

            _items.Clear();
            _itemObjects.Clear();
            TryStop();
            return;
        }

        SetPositionAndRotationForItemObjects();
        CheckItems();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PivotForCheckItems.position, _sizeCubeForCheckItems);
    }

    private void SetPositionAndRotationForItemObjects()
    {
        for (int i = 0; i < _itemObjects.Count; i++)
        {
            float additionalSize = _sizeCubeForCheckItems.x + _additionalDistanceBetweenItems;
            float startPosition = (PivotForCheckItems.position.x - (additionalSize / 2));
            float distanceBetweetItems = additionalSize / (RecipeUserInfo.Copacity + 1);
            float newxPosition = startPosition + distanceBetweetItems * (i + 1);
            Vector3 newPosition = new Vector3(newxPosition, PivotForCheckItems.position.y, PivotForCheckItems.position.z);

            _itemObjects[i].transform.position = Vector3.Lerp(_itemObjects[i].transform.position, newPosition, Time.deltaTime * 4f);
            _itemObjects[i].transform.rotation = Quaternion.identity;
        }
    }
    private void UpdateCurrentRecipe()
    {
        _currentRecipe = LoaderItemsFromResources.Instance.GetRecipeByNeedItems(_items, _recipeUser);
    }
    private void CheckItems()
    {
        if (_pickableObject.IsHold || IsWorking) return;

        Collider[] hitColliders = Physics.OverlapBox(PivotForCheckItems.position, _sizeCubeForCheckItems);
        List<ItemObject> itemObjects = new();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out ItemObject itemObject))
            {
                itemObjects.Add(itemObject);
            }
        }

        if (itemObjects.Count > RecipeUserInfo.Copacity)
        {
            int i = 0;
            uint countTrying = RecipeUserInfo.Copacity;
            while (i < itemObjects.Count && countTrying != 0)
            {
                countTrying--;

                if (_itemObjects.Contains(itemObjects[i]))
                {
                    i++;
                    continue;
                }

                itemObjects.RemoveAt(i);
            }
        }

        _items.Clear();
        _itemObjects.Clear();

        foreach (ItemObject itemObject in itemObjects)
        {
            _itemObjects.Add(itemObject);
            _items.Add(itemObject.Item);

            itemObject.DisableGravity();

            if (_itemObjects.Count >= RecipeUserInfo.Copacity) break;
        }

        UpdateCurrentRecipe();
        UpdateView();
        if (_items.Count == 0) TryStop();
    }

    [Command(requiresAuthority = false)]
    protected virtual void SpawnNewItem()
    {
        var obj = Instantiate(_currentRecipe.Item.Prefab, PivotForSpawnNewItem.position, Quaternion.identity).gameObject;
        NetworkServer.Spawn(obj);
    }
    protected virtual void LockItems()
    {
        foreach (var item in _itemObjects)
        {
            item.DisableCanPickUp();
            item.transform.parent = transform;
        }
    }
    protected bool IsAllObjectNotHold()
    {
        foreach (var item in _itemObjects)
        {
            if (item.IsHold) return false;
        }
        return true;
    }

    protected abstract IEnumerator UsingRecipe();

    protected abstract void UpdateView();
    protected abstract void UpdateElectricityView();
    protected abstract void ResetProgress();

    protected abstract void TryStart();
    protected abstract void TryStop();

    protected override bool TryUsageElectricity(uint value)
    {
        if (!IsHaveElectricity) return false;
        if (_electricity - value <= 0) return false;

        _electricity -= value;
        UpdateElectricityView();
        return true;
    }
    public override bool TryApplyElectricity(uint value)
    {
        if (IsElectricityFull) return false;
        if (_electricity + value > RecipeUserInfo.ElectricityCopacity) return false;

        _electricity += value;
        UpdateElectricityView();
        return true;
    }
}
