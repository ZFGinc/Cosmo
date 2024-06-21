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

    private float _currentAngle = 0;
    private float _stepAngle = 45f;

    private void Start()
    {
        Initialize();
        SubscripbeInputActions();
    }

    private void Update()
    {

        ReadMovement();
        ReadCameraZoom();

        RotateCamera();
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

        _cameraFollower.gameObject.SetActive(true);
    }

    private void ReadMovement()
    {
        Vector2 inputDirection = _gameInput.Gameplay.Movement.ReadValue<Vector2>();
        inputDirection = RotateVector2(inputDirection, -_currentAngle);

        Vector3 direction = new Vector3(inputDirection.x, 0, inputDirection.y);

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
        _cameraFollower.transform.rotation = Quaternion.Lerp(_cameraFollower.transform.rotation, Quaternion.Euler(0, _currentAngle, 0), Time.deltaTime * 5);
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

        _gameInput.Gameplay.CameraRotateLeft.performed += OnCameraRotateLeftPerformed;
        _gameInput.Gameplay.CameraRotateRight.performed += OnCameraRotateRightPerformed;
    }

    private void UnscripbeInputActions()
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
        _currentAngle -= _stepAngle;
        CorrectCurrentAngle();
    }

    private void OnCameraRotateRightPerformed(InputAction.CallbackContext obj)
    {
        _currentAngle += _stepAngle;
        CorrectCurrentAngle();
    }

    private void CorrectCurrentAngle()
    {
        if (_currentAngle > 180) _currentAngle -= 360;
        if (_currentAngle < -180) _currentAngle += 360;
    }
}

