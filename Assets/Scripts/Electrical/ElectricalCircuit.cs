using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricalCircuit : MonoBehaviour
{
    [SerializeField] private LineRenderer _prefabWire;

    private bool _isConsumersCached = false;

    private List<LineRenderer> _wires = new();

    private List<WireConnections> _cacheConnections = new();
    private List<ElecticGenerator> _cacheElectricGenerators = new();

    private List<Tuple<ElecticGenerator, RackPower>> _startEdges = new();
    private List<Tuple<RackPower, RackPower>> _rackEdges = new();
    private List<Tuple<RackPower, IConsumer>> _endEdges = new();

    private Dictionary<ElecticGenerator, HashSet<IConsumer>> _cacheConsumersForElectricGenerators = new();

    public static ElectricalCircuit Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        UpdateWireConnections();
    }

    private void CacheWireConnections()
    {
        _cacheElectricGenerators.Clear();
        _cacheConnections.Clear();

        var racksPower = FindObjectsByType<RackPower>(FindObjectsSortMode.InstanceID).ToList();

        foreach (var rack in racksPower)
        {
            _cacheConnections.Add(rack.GetWireConnections());

            List<ElecticGenerator> temp = new();
            rack.GetElectricGenerators(out temp);

            _cacheElectricGenerators.AddRange(temp);
        }
    }

    private void CalculateEdges()
    {
        _startEdges.Clear();
        _rackEdges.Clear();
        _endEdges.Clear();

        CalculateStartEdges();
        CalculateRackEdges();
    }

    private void CalculateStartEdges()
    {
        foreach (WireConnections connection in _cacheConnections)
        {
            for (int i = 0; i < connection.Consumers.Count; i++)
            {
                var consumer = connection.Consumers[i];
                bool isElectricGenerator = false;
                int index = 0;

                //check if electric generator
                for (int j = 0; j < _cacheElectricGenerators.Count; j++)
                {
                    if (_cacheElectricGenerators[j] == consumer)
                    {
                        isElectricGenerator = true;
                        index = j; 
                        break;
                    }
                }

                //edge(generator;me)
                if (isElectricGenerator)
                {
                    connection.Consumers.Remove(consumer);
                    _startEdges.Add(new Tuple<ElecticGenerator, RackPower>(_cacheElectricGenerators[index], connection.This));
                    i--;
                }

                //edge(me, consumer)
                else
                {
                    if (consumer.GetType() == typeof(RackPower)) continue;
                    _endEdges.Add(new Tuple<RackPower, IConsumer>(connection.This, consumer));
                }
            }
        }
    }

    private void CalculateRackEdges()
    {
        foreach (WireConnections connection in _cacheConnections)
        {
            for (int i = 0; i < connection.Connections.Count; i++)
            {
                RackPower rack = connection.Connections[i];
                Tuple<RackPower, RackPower> edge;

                //edge(b;a)
                if (rack.GetWireConnections().Connections.Contains(connection.This))
                {
                    rack.GetWireConnections().Connections.Remove(connection.This);
                    connection.Connections.Remove(rack);
                    edge = new Tuple<RackPower, RackPower>(rack, connection.This);
                }
                //edge(a;b)
                else
                {
                    connection.Connections.Remove(rack);
                    edge = new Tuple<RackPower, RackPower>(connection.This, rack);
                }
                i--;
                _rackEdges.Add(edge);
            }
        }
    }

    private void CorrectRackEdges()
    {
        int maxTryingFindParent = 3;
        int currnetTrying = maxTryingFindParent;

        for (int i = 0; i < _rackEdges.Count; i++)
        {
            if (currnetTrying == 0)
            {
                currnetTrying = maxTryingFindParent;
                continue;
            }

            bool isHaveParentEdge = false;
            for (int j = 0; j < _startEdges.Count; j++)
            {
                if (_startEdges[j].Item2 == _rackEdges[i].Item1)
                {
                    isHaveParentEdge = true;
                    break;
                }
            }
            if (!isHaveParentEdge)
            {
                for (int j = 0; j < _rackEdges.Count; j++)
                {
                    if (_rackEdges[j].Item2 == _rackEdges[i].Item1)
                    {
                        isHaveParentEdge = true;
                        break;
                    }
                }
            }

            if (isHaveParentEdge)
            {
                currnetTrying = maxTryingFindParent;
                continue;
            }

            _rackEdges[i] = new Tuple<RackPower, RackPower>(_rackEdges[i].Item2, _rackEdges[i].Item1);
            currnetTrying--;
            i--;
        }
    }

    private void ShowWires()
    {
        ClearWires();
        
        foreach(var startWire in _startEdges)
        {
            InstantiateWire(startWire.Item1.GetPosition(), startWire.Item2.GetPosition());
        }

        foreach (var rackWire in _rackEdges)
        {
            InstantiateWire(rackWire.Item1.GetPosition(), rackWire.Item2.GetPosition());
        }

        foreach (var endWire in _endEdges)
        {;
            InstantiateWire(endWire.Item1.GetPosition(), endWire.Item2.GetPosition());
        }
    }

    private void InstantiateWire(Vector3 positionItem1, Vector3 positionItem2)
    {
        var currentWire = Instantiate(_prefabWire, Vector3.zero, Quaternion.identity, transform);

        currentWire.positionCount = 2;
        currentWire.SetPosition(0, positionItem1);
        currentWire.SetPosition(1, positionItem2);

        _wires.Add(currentWire);
    }

    private void ClearWires()
    {
        if (_wires.Count == 0) return;

        int i = 0;
        while (i < _wires.Count)
        {
            if (_wires[i] != null)
            {
                Destroy(_wires[i].gameObject);
                _wires.RemoveAt(i);
                continue;
            }
            i++;
        }

        _wires = new List<LineRenderer>();
    }

    private void CacheConsumers()
    {
        _cacheConsumersForElectricGenerators.Clear();

        foreach(var start in _startEdges)
        {
            if (_cacheConsumersForElectricGenerators.ContainsKey(start.Item1)) continue;

            _cacheConsumersForElectricGenerators.Add(start.Item1, new HashSet<IConsumer>() { });
            List<IConsumer> list = new();

            var copyList = new Tuple<RackPower, RackPower>[_rackEdges.Count];
            var visible = new List<Tuple<RackPower, RackPower>>();

            foreach (var end in _endEdges)
            {
                if (end.Item1 != start.Item2) continue;

                list.Add(end.Item2);
            }

            foreach (var rack in _rackEdges)
            {
                if (rack.Item1 != start.Item2) continue;

                _rackEdges.CopyTo(copyList);
                list.AddRange(GetPath(ref visible, copyList.ToList(), rack));
            }

            _cacheConsumersForElectricGenerators[start.Item1].AddRange(list);
        }
    }

    private List<IConsumer> GetPath(ref List<Tuple<RackPower, RackPower>> visible, List<Tuple<RackPower, RackPower>> rackEdges, Tuple<RackPower, RackPower> rack)
    {
        rackEdges.Remove(rack);
        visible.Add(rack);

        var copyList = new Tuple<RackPower, RackPower>[rackEdges.Count];
        var list = new List<IConsumer>();
        var temp = new List<IConsumer>();

        rack.Item2.GetConnectionsConsumers(out temp);
        list.AddRange(temp);

        foreach (var edge in rackEdges)
        {
            if (edge.Item1 != rack.Item2) continue;
            if (visible.Contains(edge)) continue;

            rackEdges.CopyTo(copyList);
            list.AddRange(GetPath(ref visible, copyList.ToList(), edge));
        }

        return list;
    }

    private void UpdateWireConnections()
    {
        CacheWireConnections();
        CalculateEdges();
        CorrectRackEdges();
        ShowWires();
        CacheConsumers();
    }

    public HashSet<IConsumer> GetConsumers(ElecticGenerator miner)
    {
        if(_cacheConsumersForElectricGenerators.ContainsKey(miner))
            return _cacheConsumersForElectricGenerators[miner];

        return new HashSet<IConsumer>() { };
    }
}
