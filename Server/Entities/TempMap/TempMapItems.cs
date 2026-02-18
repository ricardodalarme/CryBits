using CryBits.Entities;
using CryBits.Entities.Slots;

namespace CryBits.Server.Entities.TempMap;

internal class TempMapItems(Item item, short amount, byte x, byte y)
    : ItemSlot(item, amount)
{
    public byte X = x;
    public byte Y = y;
}