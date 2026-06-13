using CryBits.Definitions.Slots;
using System;

namespace CryBits.Simulation.Entities;

public class GroundItem(Guid itemId, short amount, byte x, byte y)
    : ItemSlot(itemId, amount)
{
    public byte X = x;
    public byte Y = y;
}
