using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricalCircuit : MonoBehaviour
{
    [SerializeField] private LineRenderer _prefabWire;

    private bool _isConsumersCached = false;

    private List<LineRenderer> _wires = new();

    private List<WireConnections> _cacheConnections = new();
    private List<ElecticGenerator> _cacheElectricGenerators = new();

    private List<Tuple<IElectricalConnect, IElectricalConnect>> _startEdges = new();
    private List<Tuple<IElectricalConnect, IElectricalConnect>> _rackEdges = new();
    private Dictionary<IElectricalConnect, List<IConsumer>> _cacheConsumersForElectricGenerators = new();

    public static ElectricalCircuit Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void UpdateWireConnections()
    {
        CacheWireConnections();
        CalculateEdges();
        CorrectEdges();
        ShowWires();
    }


    private void CacheWireConnections()
    {
        var racksPower = FindObjectsByType<RackPower>(FindObjectsSortMode.InstanceID).ToList();
        _cacheElectricGenerators = new();

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
                    _startEdges.Add(new Tuple<IElectricalConnect, IElectricalConnect>(_cacheElectricGenerators[index], connection.This));
                    i--;
                }

                //edge(me, consumer)
                else
                {
                    if (consumer.GetType() == typeof(RackPower)) continue;
                    _rackEdges.Add(new Tuple<IElectricalConnect, IElectricalConnect>(connection.This, consumer.TryGetIElectricalConnect()));
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
                Tuple<IElectricalConnect, IElectricalConnect> edge;

                //edge(b;a)
                if (rack.GetWireConnections().Connections.Contains(connection.This))
                {
                    rack.GetWireConnections().Connections.Remove(connection.This);
                    connection.Connections.Remove(rack);
                    edge = new Tuple<IElectricalConnect, IElectricalConnect>(rack, connection.This);
                }
                //edge(a;b)
                else
                {
                    connection.Connections.Remove(rack);
                    edge = new Tuple<IElectricalConnect, IElectricalConnect>(connection.This, rack);
                }
                i--;
                TryAddRackEdge(edge);
            }
        }
    }

    private void TryAddRackEdge(Tuple<IElectricalConnect, IElectricalConnect> edge = null)
    {
        if (edge == null) return;

        for(int i = 0; i < _rackEdges.Count; i++)
        {
            if (_rackEdges[i].Item2 == edge.Item2) return;
        }
        _rackEdges.Add(edge);
    }

    private void CorrectEdges()
    {
        int countMaxFindParent = 2;

        for (int i = 0; i < _rackEdges.Count; i++)
        {
            if (countMaxFindParent == 0)
            {
                countMaxFindParent = 2;
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
                countMaxFindParent = 2;
                continue;
            }

            _rackEdges[i] = new Tuple<IElectricalConnect, IElectricalConnect>(_rackEdges[i].Item2, _rackEdges[i].Item1);
            countMaxFindParent--;
            i--;
        }
    }

    private void ShowWires()
    {
        ClearWires();
        
    }

    private void ClearWires()
    {
        if (_wires.Count == 0) return;

        int i = 0;
        while (i < _wires.Count)
        {
            if (_wires[i] != null)
            {
                Destroy(_wires[i]);
                continue;
            }
            i++;
        }
        _wires = new List<LineRenderer>();
    }

    public List<IConsumer> GetConsumers(IElectricalConnect miner)
    {
        if(!_isConsumersCached) UpdateWireConnections();

        return new List<IConsumer>() { };

        if(_cacheConsumersForElectricGenerators.ContainsKey(miner))
            return _cacheConsumersForElectricGenerators[miner];

    }
}