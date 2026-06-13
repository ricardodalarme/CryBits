using CryBits.Definitions.Slots;
using System;
using System.Linq;
using static CryBits.Definitions.Globals;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class InventoryState
{
    public ItemSlot[] Slots { get; set; } = new ItemSlot[MaxInventory];

    public ItemSlot? Find(Guid itemId) => Slots.FirstOrDefault(x => x.ItemId == itemId);

    public byte TotalFree => (byte)Slots.Count(x => x.ItemId == Guid.Empty);
}
