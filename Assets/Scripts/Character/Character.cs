using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PickUpController))]
public sealed class Character : MonoBehaviour, IControllable
{
    [Header("Настройки управления")]
    [SerializeField, Range(5,10)] private float _speedMovement = 5f;
    [SerializeField, Range(1,5)] private float _jumpHeight = 3f;
    [Header("Поля для отслеживания косания с землей")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckerPivot;

    private CharacterController _characterController;
    private PickUpController _pickUpController;
    private Vector3 _moveDirection;
    private float _velocity;
    private float _gravity;
    private bool _isGrounded;

    private const float RADIUS_CHECKER = 0.4f;
    private const float SPEED_ROTATION = 500f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _pickUpController = GetComponent<PickUpController>();
    }

    private void FixedUpdate()
    {
        _gravity = Physics.gravity.y;
        _isGrounded = IsOnTheGround();

        if (_isGrounded && _velocity < 0f) _velocity = -1f;

        MoveInternal();
        RotateInternal();
        Gravity();
    }

    private void RotateInternal()
    {
        if (_moveDirection.sqrMagnitude == 0) return;

        Quaternion rotationDirection = Quaternion.LookRotation(_moveDirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationDirection, Time.fixedDeltaTime * SPEED_ROTATION);
    }

    private void MoveInternal()
    {
        _characterController.Move(_moveDirection * _speedMovement * Time.fixedDeltaTime);
    }

    private void Gravity()
    {
        _velocity += _gravity * Time.fixedDeltaTime;

        _characterController.Move(Vector3.up * _velocity * Time.fixedDeltaTime);
    }

    private bool IsOnTheGround()
    {
        return Physics.CheckSphere(_groundCheckerPivot.position, RADIUS_CHECKER, _groundLayer);
    }

    public void Move(Vector3 direction)
    { 
        _moveDirection = direction;
    }

    public void Jump()
    {
        if (!_isGrounded) return;

        _velocity = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    }

    public void PickUp()
    {
        _pickUpController.TryPickUpObject();
    }

    public void Action()
    {

    }
}
