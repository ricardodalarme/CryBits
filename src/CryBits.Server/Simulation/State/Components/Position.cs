using CryBits.Definitions.Common;
using System;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class Position
{
    public Guid MapId { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public Direction Direction { get; set; }
}
