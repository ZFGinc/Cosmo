using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Crafter))]
public class ViewCrafterInfo : View
{
    [Space]
    [SerializeField] private TMP_Text _textCrafterNamePlusLevel;
    [SerializeField] private TMP_Text _textNewItemName;
    [SerializeField] private TMP_Text _textForStart;
    [SerializeField] private Image _imageItemIcon;
    [Space]
    [SerializeField] private Slider _crafterCurrentElectricity;
    [Space]
    [SerializeField] private Gradient _progressBarGradient;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Image _fillAreaImage;

    private Crafter _crafter;
    private RecipeUserInfo _crafterInfo;
    private Item _item;

    private uint _electricity;
    private float _currentProgress = 0;
    private float _maxProgress = 1;

    private void Awake()
    {
        _crafter = GetComponent<Crafter>();
        HideView();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        ProgressBar();
        if(_item == null) HideView();
    }

    private void OnEnable()
    {
        _crafter.OnUpdateView += UpdateView;
        _crafter.OnUpdateElectricityView += UpdateElectricityView;
        _crafter.OnResetProgress += OnResetProgress;
    }

    private void OnDisable()
    {
        _crafter.OnUpdateView -= UpdateView;
        _crafter.OnUpdateElectricityView -= UpdateElectricityView;
        _crafter.OnResetProgress -= OnResetProgress;
    }

    private void OnDestroy()
    {
        _crafter.OnUpdateView -= UpdateView;
        _crafter.OnUpdateElectricityView -= UpdateElectricityView;
        _crafter.OnResetProgress -= OnResetProgress;
    }

    private void ProgressBar()
    {
        if (!_crafter.IsUsingRecipe) return;
        _currentProgress += Time.fixedDeltaTime;

        UpdateProgressBarValue();
        UpdateProgressBarColor();
    }

    private void UpdateProgressBarColor()
    {
        if (_fillAreaImage == null) return;

        _fillAreaImage.color = _progressBarGradient.Evaluate(_currentProgress / _maxProgress);
    }
    private void UpdateProgressBarValue()
    {
        if (_progressBar == null || !_enabledUI) return;

        _progressBar.value = _currentProgress / _maxProgress;
    }

    private void UpdateView(RecipeUserInfo crafterInfo, Item item, uint electricity)
    {
        if(item != null) _item = item;
        _crafterInfo = crafterInfo;
        _electricity = electricity;

        _currentProgress = 0;
        _maxProgress = 60 / _crafterInfo.SpeedUseRecipe;

        ShowView();
    }

    private void UpdateElectricityView(uint electricity, uint copacity)
    {
        _electricity = electricity;

        _crafterCurrentElectricity.maxValue = copacity;
        _crafterCurrentElectricity.value = electricity;
    }

    private void OnResetProgress()
    {
        _currentProgress = 0;

        UpdateProgressBarValue();
        UpdateProgressBarColor();
    }

    private void ShowView()
    {
        _crafterCurrentElectricity.maxValue = _crafterInfo.ElectricityCopacity;
        _crafterCurrentElectricity.value = _electricity;

        _textCrafterNamePlusLevel.text = $"{_crafterInfo.Name} (ур.{_crafterInfo.Level})";

        if (_item == null || _crafterInfo == null) return;

        _textNewItemName.text = $"{_item.Name}";
        _textForStart.gameObject.SetActive(true);

        _imageItemIcon.gameObject.SetActive(true);
        _imageItemIcon.sprite = _item.Icon;
    }

    private void HideView()
    {
        if (_item != null) return;

        _textNewItemName.text = "Ожидание предметов";
        _textForStart.gameObject.SetActive(false);

        _imageItemIcon.gameObject.SetActive(false);
    }
}