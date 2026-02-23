using System.Collections.Generic;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities.Shop;
using CryBits.Extensions;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ShopHandler
{
    [PacketHandler]
    internal static void Shops(ShopsPacket packet)
    {
        // Read shops dictionary
        Shop.List = packet.List;
    }

    [PacketHandler]
    internal static void ShopOpen(ShopOpenPacket packet)
    {
        // Open shop panel
        PanelsEvents.ShopOpen = Shop.List.GetValueOrDefault(packet.Id);
        Panels.Shop.Visible = PanelsEvents.ShopOpen != null;
    }
}
