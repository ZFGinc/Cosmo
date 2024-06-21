using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionController: MonoBehaviour
{
    //[SerializeField] private SteamLobby _steamLobby;
    [Space(10)]
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
        _uiInput.UI.GamepadArrows.performed += OnGamepadArrowsPerformed;
    }

    private void OnDestroy()
    {
        _uiInput.UI.MenuButton.performed -= OnMenuPerformed;
        _uiInput.UI.DebugInfo.performed -= OnDebugPerformed;
        _uiInput.UI.GamepadArrows.performed -= OnGamepadArrowsPerformed;
    }

    private void OnDisable()
    {
        _uiInput.UI.MenuButton.performed -= OnMenuPerformed;
        _uiInput.UI.DebugInfo.performed -= OnDebugPerformed;
        _uiInput.UI.GamepadArrows.performed -= OnGamepadArrowsPerformed;
    }

    private void ActionArrows(Vector2 input)
    {
        if(input ==  Vector2.zero) return;

        if(input == Vector2.up)
        {
            Debug.Log("Host lobby");
            HostGame();
        }
        if (input == Vector2.left)
        {
            Debug.Log("Connection");
            ConnectToGame();
        }
        if (input == Vector2.right)
        {
            Debug.Log("Disconnection");
            DisconectGame();
        }
        if (input == Vector2.down)
        {
            Debug.Log("Exit");
            ExitGame();
        }
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

    private void OnGamepadArrowsPerformed(InputAction.CallbackContext obj)
    {
        if (!_menuActive) return;

        Vector2 input = _uiInput.UI.GamepadArrows.ReadValue<Vector2>();

        ActionArrows(input);
    }

    public void HostGame()
    {
        //_steamLobby.HostLobby();
    }

    public void ConnectToGame()
    {
        //_steamLobby.ConnectToFriend();
    }

    public void DisconectGame()
    {
        //_steamLobby.LeaveLobby();
    }

    public void ExitGame()
    {
        //_steamLobby.LeaveLobby();
        Application.Quit();
    }
}
