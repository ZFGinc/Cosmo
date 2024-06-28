using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [SerializeField] private Transform _newPosition;

    private Vector3 _position => _newPosition.position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IPortable>(out var portable))
        {
            portable.TeleportTo(_position);
        }
    }
}
