using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;

namespace CryBits.Server.Entities;

internal class MapItemInstance(Item item, short amount, byte x, byte y)
    : ItemSlot(item, amount)
{
    public byte X = x;
    public byte Y = y;
}
