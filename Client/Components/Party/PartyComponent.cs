using Arch.Core;

namespace CryBits.Client.Components.Party;

/// <summary>Party membership for the local player.</summary>
internal struct PartyComponent
{
    /// <summary>ECS entities of the active party members (excluding the local player).</summary>
    public Entity[] Members = [];

    public PartyComponent() { }
}
