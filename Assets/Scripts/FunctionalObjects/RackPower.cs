using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class RackPower : MonoBehaviour
{
    [SerializeField] private float _powerRadius = 5f;
    [SerializeField] private uint _electricityCopacity = 2;  

    private bool _endFindConsumers = false;
    private bool _endFindRackPower = false;

    private PickableObject _pickableObject;
    private HashSet<IConsumer> _consumers = new HashSet<IConsumer>();
    private HashSet<RackPower> _racksPower = new HashSet<RackPower>();

    private void Start()
    {
        _pickableObject = GetComponent<PickableObject>();   
    }

    private void Update()
    {
        if (_pickableObject.IsHold)
        {
            _endFindConsumers = false;
            _endFindRackPower = false;

            return;
        }

        if (!_endFindConsumers) GetConnectionsConsumers();
        if (!_endFindRackPower) GetRacksPower();
    }

    private void GetConnectionsConsumers()
    {
        _consumers = new HashSet<IConsumer>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _powerRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out IConsumer connection))
            {
                if (connection == this) continue;

                _consumers.Add(connection);
            }
        }

        _endFindConsumers = true;
    }

    private void GetRacksPower()
    {
        _racksPower = new HashSet<RackPower>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _powerRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out RackPower connection))
            {
                if (connection == this) continue;
                _racksPower.Add(connection);
            }
        }

        _endFindRackPower = true;
    }
}
