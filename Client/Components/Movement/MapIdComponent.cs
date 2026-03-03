using System;

namespace CryBits.Client.Components.Movement;

/// <summary>Tracks which map a player entity currently belongs to.</summary>
internal struct MapIdComponent
{
    public Guid Value;
}
