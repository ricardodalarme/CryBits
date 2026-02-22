using CryBits.Packets.Client;
using CryBits.Server.Systems;
using CryBits.Server.World;

namespace CryBits.Server.Network.Handlers;

internal static class AccountHandler
{
    [PacketHandler]
    internal static void CreateCharacter(GameSession session, CreateCharacterPacket packet)
    {
        CharacterSystem.Create(session, packet);
    }

    [PacketHandler]
    internal static void CharacterUse(GameSession session, CharacterUsePacket packet)
    {
        CharacterSystem.Use(session, packet.CharacterIndex);
    }

    [PacketHandler]
    internal static void CharacterCreate(GameSession session, CharacterCreatePacket packet)
    {
        CharacterSystem.OpenCreation(session);
    }

    [PacketHandler]
    internal static void CharacterDelete(GameSession session, CharacterDeletePacket packet)
    {
        CharacterSystem.Delete(session, packet.CharacterIndex);
    }
}
