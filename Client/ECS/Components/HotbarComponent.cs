using CryBits.Entities.Slots;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Local player's hotbar slots. Only present on the entity with <see cref="LocalPlayerTag"/>.
/// </summary>
public sealed class HotbarComponent : IComponent
{
    public HotbarSlot[] Slots { get; set; } = new HotbarSlot[MaxHotbar];
}
