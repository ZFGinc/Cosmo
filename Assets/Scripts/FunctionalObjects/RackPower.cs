using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PickableObject))]
public class RackPower : MonoBehaviour, IElectricalConnect
{
    [SerializeField] private DecalProjector _decalProjector;
    [SerializeField] private float _powerRadius = 5f;

    private bool _endFindConsumers = false;
    private bool _endFindRackPower = false;
    private bool _isGetConsumers = false;

    private PickableObject _pickableObject;
    private HashSet<IConsumer> _consumers = new HashSet<IConsumer>();
    private HashSet<RackPower> _racksPower = new HashSet<RackPower>();

    private void Start()
    {
        _pickableObject = GetComponent<PickableObject>();
        _decalProjector = transform.GetChild(0).gameObject.GetComponent<DecalProjector>();

        _decalProjector.size = new Vector3(_powerRadius, _powerRadius, 10);
    }

    private void Update()
    {
        _decalProjector.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        if (_pickableObject.IsHold)
        {
            _endFindConsumers = false;
            _endFindRackPower = false;
            _isGetConsumers = false;

            return;
        }

        if (!_isGetConsumers)
        {
            ElectricalCircuit.Instance.UpdateWireConnections();
            _isGetConsumers = true;
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
                if (connection.GetType() == typeof(ElecticGenerator)) continue;

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

    public WireConnections GetWireConnections()
    {
        if (_pickableObject.IsHold) return new WireConnections();

        return new WireConnections()
        {
            Connections = _racksPower,
            Consumers = _consumers
        };
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
