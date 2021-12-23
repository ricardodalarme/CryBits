using CryBits.Entities;
using CryBits.Entities.Slots;

namespace CryBits.Server.Entities.TempMap;

internal class TempMapItems : ItemSlot
{
    public byte X;
    public byte Y;

    public TempMapItems(Item item, short amount, byte x, byte y) : base(item, amount)
    {
        X = x;
        Y = y;
    }
}