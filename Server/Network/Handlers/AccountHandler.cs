using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class AccountHandler
{
    [PacketHandler]
    internal static void CreateCharacter(Account account, CreateCharacterPacket packet)
    {
        CharacterSystem.Create(account, packet);
    }

    [PacketHandler]
    internal static void CharacterUse(Account account, CharacterUsePacket packet)
    {
        CharacterSystem.Use(account, packet.CharacterIndex);
    }

    [PacketHandler]
    internal static void CharacterCreate(Account account, CharacterCreatePacket packet)
    {
        CharacterSystem.OpenCreation(account);
    }

    [PacketHandler]
    internal static void CharacterDelete(Account account, CharacterDeletePacket packet)
    {
        CharacterSystem.Delete(account, packet.CharacterIndex);
    }
}
