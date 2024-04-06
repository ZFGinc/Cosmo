using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(IControllable))]
[RequireComponent(typeof(IActionController))]
public class CharacterInputController: MonoBehaviour
{
    [SerializeField] private CameraFollower _cameraFollower;
    [SerializeField] private float _zoomMultiply = 10; 

    private GameInput _gameInput;
    private IControllable _controllable;
    private IActionController _actionConntroller;

    private void Awake()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();

        _controllable = GetComponent<IControllable>();
        _actionConntroller = GetComponent<IActionController>();

        if (_controllable == null)
        {
            throw new Exception("Чет какая-то хуйня получается");
        }
    }

    private void Update()
    {
        ReadMovement();
        ReadCameraZoom();
    }

    private void ReadMovement()
    {
        var inputDirection = _gameInput.Gameplay.Movement.ReadValue<Vector2>();
        var direction = new Vector3(inputDirection.x, 0, inputDirection.y);

        _controllable.Move(direction);
    }

    private void ReadCameraZoom()
    {
        if (_cameraFollower == null) return;

        var inputZoom = _gameInput.Gameplay.CameraZoom.ReadValue<float>();

        _cameraFollower.ApplyZoomScale(inputZoom/(1000/_zoomMultiply));
    }

    private void OnEnable()
    {
        _gameInput.Gameplay.Jump.performed += OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed += OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed += OnActionPerformed;

        _gameInput.Gameplay.ItemListingLeft.performed += OnItemListingLeftPerformed;
        _gameInput.Gameplay.ItemListingRight.performed += OnItemListingRightPerformed;
    }

    private void OnDestroy()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;

        _gameInput.Gameplay.ItemListingLeft.performed -= OnItemListingLeftPerformed;
        _gameInput.Gameplay.ItemListingRight.performed -= OnItemListingRightPerformed;
    }

    private void OnDisable()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;

        _gameInput.Gameplay.ItemListingLeft.performed -= OnItemListingLeftPerformed;
        _gameInput.Gameplay.ItemListingRight.performed -= OnItemListingRightPerformed;
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

    private void OnItemListingLeftPerformed(InputAction.CallbackContext obj)
    {
        _actionConntroller.LeftSwipe();
    }

    private void OnItemListingRightPerformed(InputAction.CallbackContext obj)
    {
        _actionConntroller.RightSwipe();
    }
}

