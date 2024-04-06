using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsingRecipes : ElectricityConsumer
{
    [SerializeField] private Transform _pivotForCheckItems;
    [SerializeField] private Transform _pivotForSpawnNewItem;
    [SerializeField] private RecipeUserInfo _recipeUserInfo = null;

    protected Transform PivotForCheckItems => _pivotForCheckItems;
    protected Transform PivotForSpawnNewItem => _pivotForSpawnNewItem;
    protected RecipeUserInfo RecipeUserInfo => _recipeUserInfo;

    //Инва о текущем состоянии и об объекте
    public bool IsUsingRecipe = false;

    //Ивенты для обновления View
    public abstract event Action<RecipeUserInfo, Item, uint> OnUpdateView;
    public abstract event Action<uint, uint> OnUpdateElectricityView;

    //Текущие предметы и рецепт 
    protected List<Item> _items = new();
    protected List<ItemObject> _itemObjects = new();
    protected Recipe _currentRecipe;

    public const float RadiusForCheckItemsAroundPivots = 1f;

    protected abstract void CheckItems();
    protected abstract void UpdateCurrentRecipe();
    protected abstract void UpdateView();
    protected abstract void UpdateElectricityView();

    protected abstract void TryStart();
    protected abstract void TryStop();

    protected abstract IEnumerator UsingRecipe();

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
