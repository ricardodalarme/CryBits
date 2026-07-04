using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Input;
using CryBits.Client.Network.Senders;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using SFML.Window;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Player;

internal sealed class ItemPickupSystem(
    GameContext context,
    InputManager inputManager,
    IntentSender intentSender) : IClientSystem
{
    private const float ThrottleSecs = 0.250f;
    private float _cooldown;

    public void Update(float dt)
    {
        if (_cooldown > 0f)
            _cooldown -= dt;

        if (!inputManager.WasKeyReleased(Keyboard.Key.Space)) return;

        var entity = context.LocalPlayerEntity;
        if (entity is null || !context.World.IsAlive(entity.Value)) return;
        if (_cooldown > 0f) return;

        var myTile = context.World.Get<MovementComponent>(entity.Value);
        if (myTile == null) return;

        var playerPos = context.World.Get<Position>(entity.Value);
        if (playerPos == null) return;

        var hasItem = ChunkGrid.FindAt<GroundItem>(context.World, playerPos.MapId, myTile.TileX, myTile.TileY).HasValue;

        if (!hasItem) return;

        var inventory = context.World.Get<InventoryState>(entity.Value);
        if (inventory == null) return;

        for (byte i = 0; i < MaxInventory; i++)
        {
            if (inventory.Slots[i].ItemId != Guid.Empty) continue;

            intentSender.Send(new CollectItemIntent(default));
            _cooldown = ThrottleSecs;
            return;
        }
    }
}
