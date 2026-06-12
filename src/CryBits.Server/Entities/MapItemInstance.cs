using CryBits.Definitions.Slots;
using System;

namespace CryBits.Server.Entities;

internal class MapItemInstance(Guid itemId, short amount, byte x, byte y)
    : ItemSlot(itemId, amount)
{
    public byte X = x;
    public byte Y = y;
}
