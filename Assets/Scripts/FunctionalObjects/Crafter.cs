using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics.Tracing;

[RequireComponent(typeof(PickableObject))]
public class Crafter : UsingRecipes, IActionObject
{
    [Space]
    [SerializeField] private Vector3 _sizeCubeForCheckItems;
    [SerializeField, Min(0f)] private float _additionalDistanceBetweenItems = 0.5f;

    private PickableObject _pickableObject;

    public override event Action<RecipeUserInfo, Item, uint> OnUpdateView;
    public override event Action<uint, uint> OnUpdateElectricityView;
    public override event Action OnResetProgress;

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
        if (_pickableObject.IsHold && !IsUsingRecipe)
        {
            if (_itemObjects.Count == 0) return;
            foreach (ItemObject itemObject in _itemObjects) itemObject.EnableGravity();

            _items.Clear();
            _itemObjects.Clear();
            TryStop();
            return;
        }

        SetPositionAndRotationForItemObjects(Time.deltaTime * 4f);
        CheckItems();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(PivotForCheckItems.position, _sizeCubeForCheckItems);
    }

    private void SetPositionAndRotationForItemObjects(float deltaTime)
    {
        for (int i = 0; i < _itemObjects.Count; i++)
        {
            float additionalSize = _sizeCubeForCheckItems.x + _additionalDistanceBetweenItems;
            float startPosition = (PivotForCheckItems.position.x - (additionalSize / 2));
            float distanceBetweetItems = additionalSize / (RecipeUserInfo.Copacity + 1);
            float newxPosition = startPosition + distanceBetweetItems * (i + 1);
            Vector3 newPosition = new Vector3(newxPosition, PivotForCheckItems.position.y, PivotForCheckItems.position.z);

            _itemObjects[i].transform.position = Vector3.Lerp(_itemObjects[i].transform.position, newPosition, deltaTime);
            _itemObjects[i].transform.rotation = Quaternion.identity;
        }
    }

    private void LockItems()
    {
        foreach (var item in _itemObjects)
        {
            item.ControllCollisionDetectOff();
            item.DisableHold();
            item.transform.parent = null;
        }
    }

    private bool IsAllObjectNotHold()
    {
        foreach (var item in _itemObjects)
        {
            if (item.IsHold) return false;
        }
        return true;
    }

    protected override void UpdateView()
    {
        if (_currentRecipe == null) OnUpdateView?.Invoke(RecipeUserInfo, null, _electricity);
        else OnUpdateView?.Invoke(RecipeUserInfo, _currentRecipe.Item, _electricity);
    }

    protected override void UpdateElectricityView()
    {
        OnUpdateElectricityView?.Invoke(_electricity, RecipeUserInfo.ElectricityCopacity);
    }

    protected override void ResetProgress()
    {
        OnResetProgress?.Invoke();
    }

    protected override void CheckItems()
    {
        if (_pickableObject.IsHold || IsUsingRecipe) return;

        Collider[] hitColliders = Physics.OverlapBox(PivotForCheckItems.position, _sizeCubeForCheckItems);
        List<ItemObject> itemObjects = new();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out ItemObject itemObject))
            {
                if (itemObject.Item.GetType() != typeof(MinedItem))
                {
                    itemObjects.Add(itemObject);
                }
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

    protected override void UpdateCurrentRecipe()
    {
        _currentRecipe = LoaderItemsFromResources.Instance.GetRecipeByNeedItems(_items);
    }

    protected override IEnumerator UsingRecipe()
    {
        if (_currentRecipe == null) yield return null;

        IsUsingRecipe = true;
        UpdateView();
        ResetProgress();
        LockItems();

        float time = 60 / RecipeUserInfo.SpeedUseRecipe; //value per minute
        yield return new WaitForSeconds(time);
        if (!IsUsingRecipe || _items.Count == 0) yield return null;

        for (int i = 0; i < _currentRecipe.CountProductedItems; i++)
        {
            Instantiate(_currentRecipe.Item.Prefab, PivotForSpawnNewItem.position, Quaternion.identity);
        }

        foreach (ItemObject itemObject in _itemObjects)
        {
            Destroy(itemObject.gameObject);
        }

        _items.Clear();
        _itemObjects.Clear();

        IsUsingRecipe = false;
        UpdateView();
        ResetProgress();
    }

    protected override void TryStart()
    {
        if (IsUsingRecipe) return;
        if (!IsHaveElectricity) return;
        if (_items.Count == 0) return;
        if (_currentRecipe == null) return;
        if (!IsAllObjectNotHold()) return;

        StartCoroutine(UsingRecipe());
    }

    protected override void TryStop()
    {
        if (!IsUsingRecipe) return;

        StopCoroutine(UsingRecipe());
        IsUsingRecipe = false;
    }

    public void Action()
    {
        TryStart();
    }
}
