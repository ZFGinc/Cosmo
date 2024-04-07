using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PickableObject))]
public class RackPower : MonoBehaviour
{
    [SerializeField] private DecalProjector _decalProjector;
    [SerializeField] private float _powerRadius = 5f;

    private PickableObject _pickableObject;

    private void Start()
    {
        _pickableObject = GetComponent<PickableObject>();
        _decalProjector = transform.GetChild(0).gameObject.GetComponent<DecalProjector>();

        _decalProjector.size = new Vector3(_powerRadius*2, _powerRadius*2, 10);
    }

    private void Update()
    {
        _decalProjector.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        _decalProjector.gameObject.SetActive(_pickableObject.IsHold);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _powerRadius);
    }

    public void GetConnectionsConsumers(out List<IConsumer> list)
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

    public void GetRacksPower(out List<RackPower> list)
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
            if (collider.gameObject.TryGetComponent(out ElecticGenerator connection))
            {
                if (connection == this) continue;
                generators.Add(connection);
            }
        }

        list = generators;
    }

    public WireConnections GetWireConnections()
    {
        List<IConsumer> consumers = new List<IConsumer>();
        List<RackPower> racksPower = new List<RackPower>();

        GetConnectionsConsumers(out consumers);
        GetRacksPower(out racksPower);

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
}
