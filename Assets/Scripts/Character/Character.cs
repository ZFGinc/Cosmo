using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PickUpController))]
public class Character : MonoBehaviour, IControllable, IPortable
{
    [Header("Настройки управления")]
    [SerializeField, Range(1,10)] private float _speedMovement = 5f;
    [SerializeField, Range(1,5)] private float _jumpHeight = 3f;
    [SerializeField, Range(0f, 1f)] private float _slideFriction = 0.5f;
    [Header("Поля для отслеживания косания с землей")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckerPivot;
    [Header("Скин с анимацией игрока")]
    [SerializeField] private Animator _skinWithanimator;

    private CharacterController _characterController;
    private PickUpController _pickUpController;

    private Vector3 _moveDirection;
    private Vector3 _hitNormal;

    private float _velocity;
    private float _gravity;

    private bool _isGrounded;
    private bool _isSlopeAngle;

    private const float RADIUS_CHECKER = 0.4f;
    private const float SPEED_ROTATION = 500f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _pickUpController = GetComponent<PickUpController>();
        _gravity = Physics.gravity.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckerPivot.position, RADIUS_CHECKER);
    }

    private void FixedUpdate()
    {
        _isGrounded = IsOnTheGround();
        _isSlopeAngle = IsSlopeAngle();
        _skinWithanimator.SetBool("isGrounded", _isGrounded);
        _skinWithanimator.SetBool("isGrab", _pickUpController.IsPickableObject);

        MoveInternal();
        RotateInternal();
        Gravity();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _hitNormal = hit.normal;
    }

    private void RotateInternal()
    {
        if (_moveDirection.sqrMagnitude == 0) return;

        Quaternion rotationDirection = Quaternion.LookRotation(_moveDirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationDirection, Time.fixedDeltaTime * SPEED_ROTATION);
    }

    private void MoveInternal()
    {
        //Скольжение по отвеным поверхностям
        if (_isSlopeAngle)
        {
            _moveDirection.x += (1f - _hitNormal.y) * _hitNormal.x * _slideFriction;
            _moveDirection.z += (1f - _hitNormal.y) * _hitNormal.z * _slideFriction;
        }

        _characterController.Move(_moveDirection * _speedMovement * Time.fixedDeltaTime);

        _skinWithanimator.SetFloat("moveX", _moveDirection.x);
        _skinWithanimator.SetFloat("moveZ", _moveDirection.z);
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

    private bool IsSlopeAngle()
    {
        return Vector3.Angle(Vector3.up, _hitNormal) >= _characterController.slopeLimit;
    }

    public void Move(Vector3 direction)
    { 
        _moveDirection = direction;
    }

    public void Jump()
    {
        if (!_isGrounded) return;

        _velocity = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        _skinWithanimator.SetTrigger("jump");
    }

    public void PickUp()
    {
        _pickUpController.TryPickUpObject();
    }

    public void TeleportTo(Vector3 globalPosition)
    {
        _characterController.enabled = false;
        transform.position = globalPosition;
        _characterController.enabled = true;
    }
}
