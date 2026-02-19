using System;
using System.Collections.Generic;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities.Shop;
using CryBits.Extensions;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class ShopHandler
{
    internal static void Shops(NetDataReader data)
    {
        // Read shops dictionary
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }

    internal static void ShopOpen(NetDataReader data)
    {
        // Open shop panel
        PanelsEvents.ShopOpen = Shop.List.Get(data.GetGuid());
        Panels.Shop.Visible = PanelsEvents.ShopOpen != null;
    }
}
