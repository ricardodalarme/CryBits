using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Server.Network;

internal static class Send
{
    public static void ToPlayer(Account account, NetDataWriter data)
    {
        account.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToPlayer(Player player, NetDataWriter data)
    {
        ToPlayer(player.Account, data);
    }

    public static void ToAll(NetDataWriter data)
    {
        // Envia os dados para todos conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                ToPlayer(Account.List[i].Character, data);
    }

    public static void ToAllBut(Player player, NetDataWriter data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    ToPlayer(Account.List[i].Character, data);
    }

    public static void ToMap(TempMap map, NetDataWriter data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (Account.List[i].Character.Map == map)
                    ToPlayer(Account.List[i].Character, data);
    }

    public static void ToMapBut(TempMap map, Player player, NetDataWriter data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (Account.List[i].Character.Map == map)
                    if (player != Account.List[i].Character)
                        ToPlayer(Account.List[i].Character, data);
    }
}