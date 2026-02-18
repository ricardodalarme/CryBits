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
        // Recebe os dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }

    internal static void ShopOpen(NetDataReader data)
    {
        // Abre a loja
        PanelsEvents.ShopOpen = Shop.List.Get(data.GetGuid());
        Panels.Shop.Visible = PanelsEvents.ShopOpen != null;
    }
}
