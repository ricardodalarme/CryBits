using CryBits.Definitions.Slots;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class TradeState
{
    public EntityId? Partner { get; set; }
    public string Request { get; set; } = string.Empty;
    public TradeSlot[]? Offer { get; set; }
}
