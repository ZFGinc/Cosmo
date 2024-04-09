using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public class Smelter : UsingRecipes, IActionObject
{
    [Space]
    [Foldout("Анимации"), SerializeField] private Animator _animator;
    [Foldout("Анимации"), ShowIf("IsAnimatorNull"), SerializeField, AnimatorParam("_animator")] private int _triggerForStart;
    [Foldout("Анимации"), ShowIf("IsAnimatorNull"), SerializeField, AnimatorParam("_animator")] private int _triggerForEnd;

    public override event Action<MachineInfo, Item, uint> OnUpdateView;
    public override event Action<uint, uint> OnUpdateElectricityView;
    public override event Action OnResetProgress;

    private bool IsAnimatorNull => _animator!=null;

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

    protected override void TryStart()
    {
        if (IsWorking) return;
        if (!IsHaveElectricity) return;
        if (_items.Count == 0) return;
        if (_currentRecipe == null) return;
        if (!IsAllObjectNotHold()) return;

        StartCoroutine(UsingRecipe());
    }
    protected override void TryStop()
    {
        if (!IsWorking) return;

        StopCoroutine(UsingRecipe());
        IsWorking = false;
    }

    protected override IEnumerator UsingRecipe()
    {
        if (_currentRecipe == null || !IsHaveElectricity) yield return null;

        IsWorking = true;
        UpdateView();
        ResetProgress();
        LockItems();
        //_animator.SetTrigger(_triggerForStart);

        float time = 60 / RecipeUserInfo.SpeedWorking; //value per minute
        yield return new WaitForSeconds(time);

        bool result = TryUsageElectricity(1);

        if (!IsWorking || _items.Count == 0 || !result) yield return null;

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

        IsWorking = false;
        ResetProgress();
        UpdateView();

        //_animator.SetTrigger(_triggerForEnd);
    }

    public void Action()
    {
        TryStart();
    }
}
