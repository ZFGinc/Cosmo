using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricalCircuit : MonoBehaviour
{
    [SerializeField] private LineRenderer _wire;

    private HashSet<WireConnections> _connections = new HashSet<WireConnections>();

    private HashSet<EdgeConsumer<IElectricalConnect, IElectricalConnect>> _edges = new HashSet<EdgeConsumer<IElectricalConnect, IElectricalConnect>>();

    public static ElectricalCircuit Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        _wire = GetComponent<LineRenderer>();
    }

    public void UpdateWireConnections()
    {
        _connections.Clear();
        var racksPower = FindObjectsByType<RackPower>(FindObjectsSortMode.InstanceID);
        var generators = FindObjectsByType<ElecticGenerator>(FindObjectsSortMode.InstanceID);

        foreach(var rack in racksPower)
        {
            _connections.Add(rack.GetWireConnections());
        }

        Debug.Log($"connections: {_connections.Count}");

        foreach(var connection in _connections)
        {

        }

    }

    public HashSet<IConsumer> GetConsumers(IElectricalConnect miner)
    {
        HashSet<IConsumer> consumers = new HashSet<IConsumer>();

        //поиск подсоединенных

        return consumers;
    }
}

public struct EdgeConsumer<T, V>
{
    public T First;
    public V Last;

    public EdgeConsumer(T first, V last)
    {
        First = first;
        Last = last;
    }

    public Type FirstType => typeof(T);
    public Type LastType => typeof(V);
}