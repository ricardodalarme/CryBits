using CryBits.Interfaces;

namespace CryBits.Entities.Slots;

public class TradeSlot : ISlot
{
    public short SlotNum { get; set; }
    public short Amount { get; set; }
}