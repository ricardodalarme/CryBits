using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Simulation.State;
using CryBits.Server.Systems.Shops;

namespace CryBits.Server.Network.Handlers;

internal sealed class ShopHandler(ShopSystem shopSystem)
{
    public static ShopHandler Instance { get; } = new(ShopSystem.Instance);

    [PacketHandler]
    internal void ShopBuy(EntityId entityId, ShopBuyPacket packet)
    {
        shopSystem.Buy(entityId, packet.Slot);
    }

    [PacketHandler]
    internal void ShopSell(EntityId entityId, ShopSellPacket packet)
    {
        shopSystem.Sell(entityId, (byte)packet.Slot, packet.Amount);
    }

    [PacketHandler]
    internal void ShopClose(EntityId entityId, ShopClosePacket _)
    {
        shopSystem.Leave(entityId);
    }
}
