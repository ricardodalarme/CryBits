using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class AccountHandler
{
    [PacketHandler(ClientPacket.CreateCharacter)]
    internal static void CreateCharacter(Account account, CreateCharacterPacket packet)
    {
        CharacterSystem.Create(account, packet);
    }

    [PacketHandler(ClientPacket.CharacterUse)]
    internal static void CharacterUse(Account account, CharacterUsePacket packet)
    {
        CharacterSystem.Use(account, packet.CharacterIndex);
    }

    [PacketHandler(ClientPacket.CharacterCreate)]
    internal static void CharacterCreate(Account account, CharacterCreatePacket packet)
    {
        CharacterSystem.OpenCreation(account);
    }

    [PacketHandler(ClientPacket.CharacterDelete)]
    internal static void CharacterDelete(Account account, CharacterDeletePacket packet)
    {
        CharacterSystem.Delete(account, packet.CharacterIndex);
    }
}
