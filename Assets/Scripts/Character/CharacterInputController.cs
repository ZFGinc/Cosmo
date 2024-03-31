using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputController: MonoBehaviour
{
    private GameInput _gameInput;
    private IControllable _controllable;

    private void Awake()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();

        _controllable = GetComponent<IControllable>();

        if (_controllable == null)
        {
            throw new Exception("Чет какая-то хуйня получается");
        }
    }

    private void Update()
    {
        ReadMovement();
    }

    private void ReadMovement()
    {
        var inputDirection = _gameInput.Gameplay.Movement.ReadValue<Vector2>();
        var direction = new Vector3(inputDirection.x, 0, inputDirection.y);

        _controllable.Move(direction);
    }

    private void OnEnable()
    {
        _gameInput.Gameplay.Jump.performed += OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed += OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed += OnActionPerformed;
    }

    private void OnDestroy()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;
    }

    private void OnDisable()
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
        _controllable.Action();
    }
}

