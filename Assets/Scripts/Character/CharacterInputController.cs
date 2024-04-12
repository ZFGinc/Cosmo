using System;
using Unity.VisualScripting;
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

    private Quaternion _rotation;
    private float _stepAngle = 45f;

    private void Awake()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();

        _controllable = GetComponent<IControllable>();
        _actionConntroller = GetComponent<IActionController>();

        _rotation = _cameraFollower.transform.rotation;

        if (_controllable == null)
        {
            throw new Exception("Чет какая-то хуйня получается");
        }
    }

    private void Update()
    {
        ReadMovement();
        ReadCameraZoom();

        RotateCamera();
    }

    //private 

    private void ReadMovement()
    {
        var inputDirection = _gameInput.Gameplay.Movement.ReadValue<Vector2>();

        inputDirection = new Vector2(

        );

        var direction = new Vector3(inputDirection.x, 0, inputDirection.y);


        _controllable.Move(direction);
    }

    private void ReadCameraZoom()
    {
        if (_cameraFollower == null) return;

        var inputZoom = _gameInput.Gameplay.CameraZoom.ReadValue<float>();

        _cameraFollower.ApplyZoomScale(inputZoom/(1000/_zoomMultiply));
    }

    private void RotateCamera()
    {
        _cameraFollower.transform.rotation = Quaternion.Lerp(_cameraFollower.transform.rotation, _rotation, Time.deltaTime * 10);
    }

    private void OnEnable()
    {
        _gameInput.Gameplay.Jump.performed += OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed += OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed += OnActionPerformed;

        _gameInput.Gameplay.CameraRotateLeft.performed += OnCameraRotateLeftPerformed;
        _gameInput.Gameplay.CameraRotateRight.performed += OnCameraRotateRightPerformed;
    }

    private void OnDestroy()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;

        _gameInput.Gameplay.CameraRotateLeft.performed -= OnCameraRotateLeftPerformed;
        _gameInput.Gameplay.CameraRotateRight.performed -= OnCameraRotateRightPerformed;
    }

    private void OnDisable()
    {
        _gameInput.Gameplay.Jump.performed -= OnJumpPerformed;
        _gameInput.Gameplay.PickUp.performed -= OnPickUpPerformed;
        _gameInput.Gameplay.Action.performed -= OnActionPerformed;

        _gameInput.Gameplay.CameraRotateLeft.performed -= OnCameraRotateLeftPerformed;
        _gameInput.Gameplay.CameraRotateRight.performed -= OnCameraRotateRightPerformed;
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

    private void OnCameraRotateLeftPerformed(InputAction.CallbackContext obj)
    {
        Vector3 euler = _rotation.eulerAngles;
        euler.y -= _stepAngle;
        _rotation = Quaternion.Euler(euler);
    }

    private void OnCameraRotateRightPerformed(InputAction.CallbackContext obj)
    {
        Vector3 euler = _rotation.eulerAngles;
        euler.y += _stepAngle;
        _rotation = Quaternion.Euler(euler);
    }
}

