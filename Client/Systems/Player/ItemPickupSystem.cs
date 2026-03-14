using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Map;
using CryBits.Client.Components.Movement;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Player;

/// <summary>
/// Handles the local player picking up ground items when Space is released.
/// </summary>
internal sealed class ItemPickupSystem(GameContext context, InputManager inputManager, PlayerSender playerSender) : BaseSystem<World, float>(context.World)
{
    private readonly QueryDescription _groundItemQuery = new QueryDescription().WithAll<GroundItemComponent, TransformComponent>();

    private const float ThrottleSecs = 0.250f;

    private readonly GameContext _context = context;
    private readonly PlayerSender _playerSender = playerSender;

    /// <summary>Set by the key-release event; consumed and cleared in <see cref="Update"/>.</summary>
    private bool _pickupRequested;

    /// <summary>Remaining cooldown in seconds before the next pickup can be sent.</summary>
    private float _cooldown;

    public override void Initialize()
    {
        inputManager.KeyReleased += OnKeyReleased;
    }

    private void OnKeyReleased(object? sender, KeyEventArgs e)
    {
        if (e.Code != Keyboard.Key.Space) return;
        if (TextBox.Focused != null) return;

        _pickupRequested = true;
    }

    public override void Update(in float dt)
    {
        if (_cooldown > 0f)
            _cooldown -= dt;

        if (!_pickupRequested) return;
        _pickupRequested = false;

        var entity = _context.LocalPlayer.Entity;
        if (entity == Entity.Null || !World.IsAlive(entity)) return;
        if (_cooldown > 0f) return;

        // Confirm there is a ground item on the local player's current tile.
        var myTile = World.Get<MovementComponent>(entity);
        var hasItem = false;
        World.Query(in _groundItemQuery, (ref TransformComponent transform) =>
        {
            if (transform.X / Grid == myTile.TileX && transform.Y / Grid == myTile.TileY)
                hasItem = true;
        });

        if (!hasItem) return;

        // Confirm the local player has at least one free inventory slot.
        ref var inventory = ref World.Get<InventoryComponent>(entity);
        for (byte i = 0; i < MaxInventory; i++)
        {
            if (inventory.Slots[i]?.Item != null) continue;

            // Free slot found — send the packet and start the cooldown.
            _playerSender.CollectItem();
            _cooldown = ThrottleSecs;
            return;
        }
    }

    public override void Dispose()
    {
        inputManager.KeyReleased -= OnKeyReleased;
    }
}
