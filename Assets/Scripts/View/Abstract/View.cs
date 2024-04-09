using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public abstract class View<T> : MonoBehaviour
    where T : IMachine
{
    [BoxGroup("Базовые параметры"), SerializeField] private Transform _camera;
    [BoxGroup(""), SerializeField] protected Transform _canvas;
    [Space]
    [BoxGroup("Базовые параметры"), SerializeField] private TMP_Text _textNamePlusLevel;
    [BoxGroup("Базовые параметры"), SerializeField] protected Slider _sliderElectricity;
    [Space]
    [BoxGroup("Настройки View для предмета"), SerializeField] private TMP_Text _textItemName;
    [BoxGroup("Настройки View для предмета"), SerializeField] private Image _imageItemIcon;

    protected bool _enabledUI = false;
    protected uint _electricity = 0;

    private float _speedEnableUI = 5f;
    private float _radiusShowUINearPlayer = 5f;

    private Vector2 _minMaxSizeUI = Vector2.up;
    private RectTransform _rectTransformCanvas;

    protected T _machine;
    protected MachineInfo _machineInfo;
    protected Item _itemInfo;

    protected void Awake()
    {
        _machine = GetComponent<T>();
    }

    protected void Start()
    {
        if (_canvas == null) return;
        _rectTransformCanvas = _canvas.GetComponent<RectTransform>();

        HideView();
    }

    protected virtual void FixedUpdate()
    {
        LookAtCamera();
        ShowUINearPlayer();

        float tempSize = Mathf.Lerp(_rectTransformCanvas.localScale.x, (_enabledUI ? 1 : 0), Time.fixedDeltaTime * _speedEnableUI);
        _rectTransformCanvas.localScale = new Vector3(tempSize, 1, 1);

        if (!_enabledUI && tempSize <= 0.02) _canvas.gameObject.SetActive(false);
        else _canvas.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radiusShowUINearPlayer);
    }

    private void LookAtCamera()
    {
        _canvas.LookAt(new Vector3(_canvas.position.x, _camera.position.y, _camera.position.z));
        _canvas.Rotate(0, 180, 0);
    }

    private void ShowUINearPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radiusShowUINearPlayer);

        _enabledUI = false;
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out Character character))
            {
                _enabledUI = true;
                break;
            }
        }
    }

    protected virtual void ShowView()
    {
        if (_machineInfo == null) return;

        _textNamePlusLevel.text = $"{_machineInfo.Name} (ур.{_machineInfo.Level})";

        _sliderElectricity.maxValue = _machineInfo.ElectricityCopacity;
        _sliderElectricity.value = _electricity;

        if (_machineInfo == null || _itemInfo == null) return;

        _textItemName.text = $"{_itemInfo.Name}";

        _imageItemIcon.gameObject.SetActive(true);
        _imageItemIcon.sprite = _itemInfo.Icon;
    }

    protected virtual void HideView()
    {
        if (_itemInfo != null) return;

        _textItemName.text = "Ожидание...";

        _imageItemIcon.gameObject.SetActive(false);
    }

    protected virtual void OnUpdateView(MachineInfo machineInfo, Item itemInfo, uint electricity)
    {
        _machineInfo = machineInfo;
        _itemInfo = itemInfo;
        _electricity = electricity;

        ShowView();
    }

    protected virtual void OnUpdateElectricityView(uint electricity, uint copacity)
    {
        _electricity = electricity;

        _sliderElectricity.maxValue = copacity;
        _sliderElectricity.value = _electricity;
    }

    protected void SetBaseDefaultSettings()
    {
        _camera = Camera.main.transform;
        _canvas = transform.Find("Canvas");

        if (_canvas == null) return;

        _textNamePlusLevel = _canvas.transform.Find("Title").Find("Name & Level").GetComponent<TMP_Text>();
        _sliderElectricity = _canvas.transform.Find("Electricity").GetComponent<Slider>();
        _textItemName = _canvas.transform.Find("ItemName").GetComponent<TMP_Text>();
        _imageItemIcon = _canvas.transform.Find("ItemIcon").GetComponent<Image>();
    }
}
