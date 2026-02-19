using CryBits.Server.Entities;
using CryBits.Server.Systems;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class AccountHandler
{
    internal static void CreateCharacter(Account account, NetDataReader data)
    {
        CharacterSystem.Create(account, data);
    }

    internal static void CharacterUse(Account account, NetDataReader data)
    {
        CharacterSystem.Use(account, data.GetInt());
    }

    internal static void CharacterCreate(Account account)
    {
        CharacterSystem.OpenCreation(account);
    }

    internal static void CharacterDelete(Account account, NetDataReader data)
    {
        CharacterSystem.Delete(account, data.GetInt());
    }
}
