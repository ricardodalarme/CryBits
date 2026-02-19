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
    // Read items dictionary
    Item.List = (Dictionary<Guid, Item>)data.ReadObject();
  }
}
