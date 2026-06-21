using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Simulation.Components;
using SFML.Window;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Player;

internal sealed class ItemPickupSystem(
    GameContext context,
    InputManager inputManager,
    PlayerSender playerSender) : IClientSystem
{
    private const float ThrottleSecs = 0.250f;
    private float _cooldown;

    public void Update(float dt)
    {
        if (_cooldown > 0f)
            _cooldown -= dt;

        if (!inputManager.WasKeyReleased(Keyboard.Key.Space)) return;

        var entity = context.LocalPlayer.Entity;
        if (entity is null || !context.World.IsAlive(entity.Value)) return;
        if (_cooldown > 0f) return;

        var myTile = context.World.Get<MovementComponent>(entity.Value);
        if (myTile == null) return;

        var hasItem = false;
        foreach (var state in context.World.All)
        {
            var transform = state.Get<TransformComponent>();
            if (transform == null) continue;
            if (!state.Has<GroundItemComponent>()) continue;

            if (transform.X / Grid == myTile.TileX && transform.Y / Grid == myTile.TileY)
            {
                hasItem = true;
                break;
            }
        }

        if (!hasItem) return;

        var inventory = context.World.Get<InventoryState>(entity.Value);
        if (inventory == null) return;

        for (byte i = 0; i < MaxInventory; i++)
        {
            if (inventory.Slots[i].ItemId != Guid.Empty) continue;

            playerSender.CollectItem();
            _cooldown = ThrottleSecs;
            return;
        }
    }
}
