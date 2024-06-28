using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IControllable))]
[RequireComponent(typeof(IActionController))]
public class CharacterInputController: MonoBehaviour
{
    [SerializeField] private Transform _cameraTransfrom;
    [SerializeField] private Transform _orientation;

    private GameInput _gameInput;
    private IControllable _controllable;
    private IActionController _actionConntroller;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Initialize();
        SubscripbeInputActions();
    }

    private void Update()
    {
        ReadMovement();
    }

    private Vector2 RotateVector2(Vector2 v, float delta)
    {
        float sin = Mathf.Sin(delta * Mathf.Deg2Rad);
        float cos = Mathf.Cos(delta * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    private void Initialize()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();

        _controllable = GetComponent<IControllable>();
        _actionConntroller = GetComponent<IActionController>();

        if (_actionConntroller == null || _controllable == null)
        {
            throw new Exception("Чет какая-то хуйня получается");
        }
    }

    private void ReadMovement()
    {
        Vector2 inputDirection = _gameInput.Gameplay.Movement.ReadValue<Vector2>();
        _orientation.forward = (transform.position - _cameraTransfrom.position).normalized;

        Vector3 tempDirection = inputDirection.y * _orientation.forward + inputDirection.x * _orientation.right;
        Vector3 direction = new Vector3(tempDirection.x, 0, tempDirection.z);

        _controllable.Move(direction);
    }

    private void OnDestroy()
    {
        UnscripbeInputActions();
    }

    private void OnDisable()
    {
        UnscripbeInputActions();
    }

    private void SubscripbeInputActions()
    {
        _gameInput.Gameplay.Jump.performed += OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed += OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed += OnActionPerformed;
    }

    private void UnscripbeInputActions()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        _controllable.Jump();
    }

    private void OnPickUpPerformed(InputAction.CallbackContext obj)
    {
        _controllable.PickUp();
    }

    private void OnActionPerformed(InputAction.CallbackContext obj)
    {
        _actionConntroller.Action();
    }
}

