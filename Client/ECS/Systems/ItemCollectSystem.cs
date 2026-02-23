using System;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Sends an item-collect request to the server when the local player is standing
/// on a tile that contains a dropped item and has a free inventory slot.
///
/// Rate-limited to one attempt every 250 ms to match the original behaviour.
/// </summary>
internal sealed class ItemCollectSystem : IUpdateSystem
{
    private int _lastCollectTick;

    public void Update(GameContext ctx)
    {
        var localId = ctx.GetLocalPlayer();
        if (localId < 0) return;

        // Ignore collect requests when a text-box is focused.
        if (TextBox.Focused != null) return;

        if (!ctx.World.TryGet<TransformComponent>(localId, out var transform)) return;
        if (!ctx.World.TryGet<InventoryComponent>(localId, out var inventory)) return;

        // Check for an item on the current tile.
        var hasItem = false;
        foreach (var (_, item) in ctx.World.Query<MapItemComponent>())
            if (item.Item != null && item.TileX == transform.TileX && item.TileY == transform.TileY)
            { hasItem = true; break; }

        if (!hasItem) return;

        // Check for a free inventory slot.
        var hasSlot = false;
        foreach (var slot in inventory.Slots)
            if (slot?.Item == null) { hasSlot = true; break; }

        if (!hasSlot) return;
        if (Environment.TickCount <= _lastCollectTick + 250) return;

        PlayerSender.CollectItem();
        _lastCollectTick = Environment.TickCount;
    }
}
