using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PickableObject))]
public class RackPower : MonoBehaviour, IElectricalConnect
{
    [SerializeField] private DecalProjector _decalProjector;
    [SerializeField] private float _powerRadius = 5f;

    private bool _isGetConsumers = true;

    private PickableObject _pickableObject;

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
            _isGetConsumers = false;

            return;
        }

        if (!_isGetConsumers)
        {
            ElectricalCircuit.Instance.UpdateWireConnections();
            _isGetConsumers = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _powerRadius);
    }

    private void GetConnectionsConsumers(out List<IConsumer> list)
    {
        List<IConsumer> consumers = new();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _powerRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out IConsumer connection) && !collider.gameObject.TryGetComponent(out RackPower rackPower))
            {
                if (connection == this) continue;
                if (connection.GetType() == typeof(ElecticGenerator)) continue;

                consumers.Add(connection);
            }
        }

        list = consumers;
    }

    private void GetRacksPower(out List<RackPower> list)
    {
        List<RackPower> racksPower = new();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _powerRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out RackPower connection))
            {
                if (connection == this) continue;
                racksPower.Add(connection);
            }
        }

        list = racksPower;
    }

    public void GetElectricGenerators(out List<ElecticGenerator> list)
    {
        List<ElecticGenerator> generators = new();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _powerRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out IElectricalGenerator connection))
            {
                if (connection == this) continue;
                generators.Add(connection.GetElectricalGeneratorType());
            }
        }

        list = generators;
    }

    public WireConnections GetWireConnections()
    {
        if (_pickableObject.IsHold) return new WireConnections();

        List<IConsumer> consumers = new List<IConsumer>();
        List<RackPower> racksPower = new List<RackPower>();

        GetConnectionsConsumers(out consumers);
        GetRacksPower(out  racksPower);

        return new WireConnections()
        {
            This = this,
            Connections = racksPower,
            Consumers = consumers
        };
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool TryApplyElectricity(uint value)
    {
        throw new System.NotImplementedException();
    }

    public IElectricalConnect TryGetIElectricalConnect()
    {
        return this;
    }
}
