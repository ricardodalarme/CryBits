using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.World;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Handlers;

internal sealed class ShopHandler()
{
    public static ShopHandler Instance { get; } = new();

    [PacketHandler]
    internal void ShopBuy(EntityId entityId, ShopBuyPacket packet)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new ShopBuyIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void ShopSell(EntityId entityId, ShopSellPacket packet)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(
            new ShopSellIntent(entityId, (byte)packet.Slot, packet.Amount));
    }

    [PacketHandler]
    internal void ShopClose(EntityId entityId, ShopClosePacket _)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new ShopCloseIntent(entityId));
    }
}
