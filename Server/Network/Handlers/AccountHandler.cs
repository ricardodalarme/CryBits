using CryBits.Packets.Client;
using CryBits.Server.Systems;
using CryBits.Server.World;

namespace CryBits.Server.Network.Handlers;

internal sealed class AccountHandler(CharacterSystem characterSystem)
{
    public static AccountHandler Instance { get; } = new(CharacterSystem.Instance);

    [PacketHandler]
    internal void CreateCharacter(GameSession session, CreateCharacterPacket packet)
    {
        characterSystem.Create(session, packet);
    }

    [PacketHandler]
    internal void CharacterUse(GameSession session, CharacterUsePacket packet)
    {
        characterSystem.Use(session, packet.CharacterIndex);
    }

    [PacketHandler]
    internal void CharacterCreate(GameSession session, CharacterCreatePacket packet)
    {
        characterSystem.OpenCreation(session);
    }

    [PacketHandler]
    internal void CharacterDelete(GameSession session, CharacterDeletePacket packet)
    {
        characterSystem.Delete(session, packet.CharacterIndex);
    }
}
