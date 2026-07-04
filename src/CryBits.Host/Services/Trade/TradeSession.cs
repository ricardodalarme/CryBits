using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Services.Trade;


internal sealed class TradeSession(EntityId entityA, EntityId entityB)
{
    public EntityId EntityA { get; } = entityA;
    public EntityId EntityB { get; } = entityB;

    public TradeSlot[] OfferA { get; set; } = new TradeSlot[MaxInventory];
    public TradeSlot[] OfferB { get; set; } = new TradeSlot[MaxInventory];

    public TradeStatus StatusA { get; set; } = TradeStatus.Waiting;
    public TradeStatus StatusB { get; set; } = TradeStatus.Waiting;
}
