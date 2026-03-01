using System;
using Arch.Core;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Input;
using CryBits.Client.Network.Senders;
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
    public ItemSlot[] TradeOffer;
    public ItemSlot[] TradeTheirOffer;
    public Player[] Party = Array.Empty<Player>();
    public int Experience;
    public int ExpNeeded;
    public short Points;
    private int _collectTimer;

    public override void Logic()
    {
        Me.CheckMovement();
        Me.CheckAttack();
        base.Logic();
    }

    public void CheckMovement()
    {
        if (Movement > 0) return;

        // Handle movement key input
        if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Up)) Move(Direction.Up);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Down)) Move(Direction.Down);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Left)) Move(Direction.Left);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Right)) Move(Direction.Right);
    }

    public void Move(Direction direction)
    {
        // Return if player cannot move.
        if (Movement != Movement.Stopped) return;

        // Update facing direction and notify server.
        if (Direction != direction)
        {
            Direction = direction;
            PlayerSender.Instance.PlayerDirection();
        }

        // Cancel if next tile is blocked.
        if (MapInstance.TileBlocked(X, Y, direction)) return;

        // Choose movement speed (walk/run).
        if (InputManager.Instance.IsKeyPressed(Keyboard.Key.LShift))
            Movement = Movement.Moving;
        else
            Movement = Movement.Walking;

        // Notify server of movement.
        PlayerSender.Instance.PlayerMove();

        // Set pixel offset for smooth movement.
        switch (direction)
        {
            case Direction.Up:
                Y2 = Grid;
                Y--;
                break;
            case Direction.Down:
                Y2 = Grid * -1;
                Y++;
                break;
            case Direction.Right:
                X2 = Grid * -1;
                X++;
                break;
            case Direction.Left:
                X2 = Grid;
                X--;
                break;
        }
    }

    public void CheckAttack()
    {
        // Reset attack state if cooldown expired.
        if (AttackTimer + AttackSpeed < Environment.TickCount)
        {
            AttackTimer = 0;
            Attacking = false;
        }

        // Only proceed if attack key pressed and player may attack.
        if (!InputManager.Instance.IsKeyPressed(Keyboard.Key.LControl)) return;
        if (AttackTimer > 0) return;
        if (Panels.Trade.Visible) return;
        if (Panels.Shop.Visible) return;

        AttackTimer = Environment.TickCount;
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
