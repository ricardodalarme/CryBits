using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class ShopService()
{
    public static ShopService Instance { get; } = new();

    [PacketHandler]
    internal void ShopBuy(EntityId entityId, ShopBuyPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new ShopBuyIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void ShopSell(EntityId entityId, ShopSellPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new ShopSellIntent(entityId, (byte)packet.Slot, packet.Amount));
    }

    [PacketHandler]
    internal void ShopClose(EntityId entityId, ShopClosePacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new ShopCloseIntent(entityId));
    }
}
