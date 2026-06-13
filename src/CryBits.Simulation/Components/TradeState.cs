using CryBits.Definitions.Slots;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Components;

public sealed class TradeState
{
    public EntityId? Partner { get; set; }
    public string Request { get; set; } = string.Empty;
    public TradeSlot[]? Offer { get; set; }
}
