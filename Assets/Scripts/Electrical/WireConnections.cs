using System.Collections.Generic;

public struct WireConnections
{
    public RackPower This;
    public List<RackPower> Connections;
    public List<IConsumer> Consumers;
}

