namespace CryBits.Client.Components.Character;

/// <summary>Stores the server-assigned map NPC index on an NPC entity for ECS-native lookups.</summary>
internal struct NpcIndexComponent
{
    public byte Value;
}
