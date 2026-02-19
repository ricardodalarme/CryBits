using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class AuthHandler
{
    internal static void Latency(Account account)
    {
        AuthSender.Latency(account);
    }

    internal static void Connect(Account account, NetDataReader data)
    {
        AuthSystem.Connect(account, data);
    }

    internal static void Register(Account account, NetDataReader data)
    {
        AuthSystem.Register(account, data);
    }
}
