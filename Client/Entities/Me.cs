using System;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Slots;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.Entities;

internal class Me(string name) : Player(name)
{
    public ItemSlot?[] Inventory = new ItemSlot[MaxInventory];
    public HotbarSlot[] Hotbar = new HotbarSlot[MaxHotbar];
    public ItemSlot?[] TradeOffer;
    public ItemSlot?[] TradeTheirOffer;
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
        if (Movement > 0 || !Renders.RenderWindow.HasFocus()) return;

        // Handle movement key input.
        if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) Move(Direction.Up);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) Move(Direction.Down);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) Move(Direction.Left);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) Move(Direction.Right);
    }

    public void Move(Direction direction)
    {
        // Return if player cannot move.
        if (Movement != Movement.Stopped) return;

        // Update facing direction and notify server.
        if (Direction != direction)
        {
            Direction = direction;
            PlayerSender.PlayerDirection();
        }

        // Cancel if next tile is blocked.
        if (MapInstance.TileBlocked(X, Y, direction)) return;

        // Choose movement speed (walk/run).
        if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Renders.RenderWindow.HasFocus())
            Movement = Movement.Moving;
        else
            Movement = Movement.Walking;

        // Notify server of movement.
        PlayerSender.PlayerMove();

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
        if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl) || !Renders.RenderWindow.HasFocus()) return;
        if (AttackTimer > 0) return;
        if (Panels.Trade.Visible) return;
        if (Panels.Shop.Visible) return;

        AttackTimer = Environment.TickCount;
        PlayerSender.PlayerAttack();
    }

    public void CollectItem()
    {
        bool hasItem = false, hasSlot = false;

        // Ignore collect when a textbox is focused.
        if (TextBox.Focused != null) return;

        // Check for an item at the player's tile.
        for (byte i = 0; i < MapInstance.Current.Item.Length; i++)
            if (MapInstance.Current.Item[i].X == X && MapInstance.Current.Item[i].Y == Y)
                hasItem = true;

        // Check for a free inventory slot.
        for (byte i = 0; i < MaxInventory; i++)
            if (Inventory[i].Item == null)
                hasSlot = true;

        if (!hasItem) return;
        if (!hasSlot) return;
        if (Environment.TickCount <= _collectTimer + 250) return;

        // Request item pickup.
        PlayerSender.CollectItem();
        _collectTimer = Environment.TickCount;
    }

    public void Leave()
    {
        // Clear local player state.
        List.Clear();
        Me = null;
    }
}
