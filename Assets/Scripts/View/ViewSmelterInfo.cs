using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Smelter))]
public class ViewSmelterInfo : View
{
    [Space]
    [SerializeField] private TMP_Text _textSmelterNamePlusLevel;
    [SerializeField] private TMP_Text _textNewItemName;
    [SerializeField] private TMP_Text _textForStart;
    [SerializeField] private Image _imageItemIcon;
    [Space]
    [SerializeField] private Slider _smelterCurrentElectricity;
    [Space]
    [SerializeField] private Gradient _progressBarGradient;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Image _fillAreaImage;

    private Smelter _smelter;
    private RecipeUserInfo _smelterInfo;
    private Item _item;

    private uint _electricity;
    private float _currentProgress = 0;
    private float _maxProgress = 1;

    private void Awake()
    {
        _smelter = GetComponent<Smelter>();
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
        _smelter.OnUpdateView += UpdateView;
        _smelter.OnUpdateElectricityView += UpdateElectricityView;
    }

    private void OnDisable()
    {
        _smelter.OnUpdateView -= UpdateView;
        _smelter.OnUpdateElectricityView -= UpdateElectricityView;
    }

    private void OnDestroy()
    {
        _smelter.OnUpdateView -= UpdateView;
        _smelter.OnUpdateElectricityView -= UpdateElectricityView;
    }

    private void ProgressBar()
    {
        if (_progressBar == null || !_enabledUI) return;
        _progressBar.value = _currentProgress / _maxProgress;

        if (!_smelter.IsUsingRecipe) return;
        _currentProgress += Time.fixedDeltaTime;

        if (_fillAreaImage == null) return;
        _fillAreaImage.color = _progressBarGradient.Evaluate(_currentProgress / _maxProgress);
    }

    private void UpdateView(RecipeUserInfo smelterInfo, Item item, uint electricity)
    {
        if(item != null) _item = item;
        _smelterInfo = smelterInfo;
        _electricity = electricity;

        _currentProgress = 0;
        _maxProgress = 60 / _smelterInfo.SpeedUseRecipe;

        ShowView();
    }

    private void UpdateElectricityView(uint electricity, uint copacity)
    {
        _electricity = electricity;

        _smelterCurrentElectricity.maxValue = copacity;
        _smelterCurrentElectricity.value = electricity;
    }

    private void ShowView()
    {
        _smelterCurrentElectricity.maxValue = _smelterInfo.ElectricityCopacity;
        _smelterCurrentElectricity.value = _electricity;

        _textSmelterNamePlusLevel.text = $"{_smelterInfo.Name} (ур.{_smelterInfo.Level})";

        if (_item == null || _smelterInfo == null) return;

        _textNewItemName.text = $"{_item.Name}";
        _textForStart.gameObject.SetActive(true);

        _imageItemIcon.gameObject.SetActive(true);
        _imageItemIcon.sprite = _item.Icon;
    }

    private void HideView()
    {
        if (_item != null) return;

        _textNewItemName.text = "ќжидание руды";
        _textForStart.gameObject.SetActive(false);

        _imageItemIcon.gameObject.SetActive(false);
    }
}