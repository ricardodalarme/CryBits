using System;

namespace CryBits.Client.Components.Character;

/// <summary>
/// Stable network identity for a character entity (player or NPC).
/// </summary>
internal readonly struct NetworkIdComponent(Guid value)
{
    public readonly Guid Value = value;
}
