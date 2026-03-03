using System;
using Arch.Core;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Components.Movement;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Entities.Slots;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.Entities;

internal class Me(string name) : Player(name)
{
    public ItemSlot[] Inventory = new ItemSlot[MaxInventory];
    public HotbarSlot[] Hotbar = new HotbarSlot[MaxHotbar];
    public ItemSlot[]? TradeOffer;
    public ItemSlot[]? TradeTheirOffer;
    public Player[] Party = [];
    public int Experience;
    public int ExpNeeded;
    public short Points;
    private int _collectTimer;

    public void Logic()
    {
        if (Entity == Entity.Null) return;
        CheckMovement();
        CheckAttack();
    }

    public void CheckMovement()
    {
        ref var movement = ref GameContext.Instance.World.Get<MovementComponent>(Entity);
        if (movement.MovementState != Movement.Stopped) return;

        // Handle movement key input
        if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Up)) Move(Direction.Up, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Down)) Move(Direction.Down, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Left)) Move(Direction.Left, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Right)) Move(Direction.Right, ref movement);
    }

    private void Move(Direction direction, ref MovementComponent movement)
    {
        // Always sync facing direction (also updates server via the combined packet below).
        Direction = direction;
        movement.Direction = direction;

        // Determine locomotion mode before blocking check so the server knows intent.
        var desired = InputManager.Instance.IsKeyPressed(Keyboard.Key.LShift)
            ? Movement.Moving
            : Movement.Walking;

        // Notify server with a single intent-only packet (no X/Y — server is authoritative).
        PlayerSender.Instance.PlayerMove(direction, desired);

        // Cancel if next tile is blocked (client does not step, server will agree).
        if (MapInstance.TileBlocked(X, Y, direction)) return;

        // Apply speed for local interpolation immediately (no network round-trip needed).
        movement.MovementState = desired;
        movement.SpeedPixelsPerSecond = desired == Movement.Moving
            ? RunSpeedPixelsPerSecond
            : WalkSpeedPixelsPerSecond;

        // Step to the target tile and prime the starting pixel offset so the
        // movement system interpolates it back to zero every rendered frame.
        switch (direction)
        {
            case Direction.Up:
                movement.OffsetY = Grid;
                Y--;
                movement.TileY = Y;
                break;
            case Direction.Down:
                movement.OffsetY = -Grid;
                Y++;
                movement.TileY = Y;
                break;
            case Direction.Right:
                movement.OffsetX = -Grid;
                X++;
                movement.TileX = X;
                break;
            case Direction.Left:
                movement.OffsetX = Grid;
                X--;
                movement.TileX = X;
                break;
        }
    }

    public void CheckAttack()
    {
        ref var state = ref GameContext.Instance.World.Get<CharacterStateComponent>(Entity);

        // Reset attack state if cooldown expired.
        if (state.AttackTimer + AttackSpeed < Environment.TickCount)
        {
            state.AttackTimer = 0;
            state.IsAttacking = false;
        }

        // Only proceed if attack key pressed and player may attack.
        if (!InputManager.Instance.IsKeyPressed(Keyboard.Key.LControl)) return;
        if (state.AttackTimer > 0) return;
        if (TradeView.Panel.Visible) return;
        if (ShopView.Panel.Visible) return;

        state.AttackTimer = Environment.TickCount;
        state.IsAttacking = true;
        PlayerSender.Instance.PlayerAttack();
    }

    public void CollectItem()
    {
        bool hasItem = false, hasSlot = false;

        // Ignore collect when a textbox is focused.
        if (TextBox.Focused != null) return;

        // Check for an item at the player's tile.
        var world = GameContext.Instance.World;
        var itemQuery = new QueryDescription().WithAll<GroundItemComponent, TransformComponent>();
        world.Query(in itemQuery, (ref GroundItemComponent _, ref TransformComponent transform) =>
        {
            if (transform.X / Grid == X && transform.Y / Grid == Y)
                hasItem = true;
        });

        // Check for a free inventory slot.
        for (byte i = 0; i < MaxInventory; i++)
            if (Inventory[i].Item == null)
                hasSlot = true;

        if (!hasItem) return;
        if (!hasSlot) return;
        if (Environment.TickCount <= _collectTimer + 250) return;

        // Request item pickup.
        PlayerSender.Instance.CollectItem();
        _collectTimer = Environment.TickCount;
    }

    public void Leave()
    {
        // Clear local player state.
        List.Clear();
        Me = null;
    }
}
