using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Extensions;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class ItemHandler
{
  internal static void Items(NetDataReader data)
  {
    // Recebe os dados
    Item.List = (Dictionary<Guid, Item>)data.ReadObject();
  }
}
