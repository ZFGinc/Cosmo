using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionController: MonoBehaviour
{
    [SerializeField, Required] private Canvas _menuCanvas;
    [SerializeField, Required] private Canvas _debugCanvas;

    private UserInterface _uiInput;

    private bool _menuActive = false;
    private bool _debugActive = false;

    private void Awake()
    {
        _uiInput = new UserInterface();
        _uiInput.Enable();

        _menuActive = false;
        _debugActive = false;

        _menuCanvas.gameObject.SetActive(_menuActive);
        _debugCanvas.gameObject.SetActive(_debugActive);
    }

    private void OnEnable()
    {
        _uiInput.UI.MenuButton.performed += OnMenuPerformed;
        _uiInput.UI.DebugInfo.performed += OnDebugPerformed;
    }

    private void OnDestroy()
    {
        _uiInput.UI.MenuButton.performed -= OnMenuPerformed;
        _uiInput.UI.DebugInfo.performed -= OnDebugPerformed;
    }

    private void OnDisable()
    {
        _uiInput.UI.MenuButton.performed -= OnMenuPerformed;
        _uiInput.UI.DebugInfo.performed -= OnDebugPerformed;
    }

    private void OnDebugPerformed(InputAction.CallbackContext obj)
    {
        _debugActive = !_debugActive;
        _debugCanvas.gameObject.SetActive(_debugActive);
    }

    private void OnMenuPerformed(InputAction.CallbackContext obj)
    {
        _menuActive = !_menuActive;
        _menuCanvas.gameObject.SetActive(_menuActive);
    }
}
