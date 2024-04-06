using System.Collections.Generic;
using UnityEngine;

public struct WireConnections
{
    public RackPower This;
    public List<RackPower> Connections;
    public List<IConsumer> Consumers;
}

