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

    public override void Logic()
    {
        Me.CheckMovement();
        Me.CheckAttack();
        base.Logic();
    }

    public void CheckMovement()
    {
        // Entity is spawned on the first Logic() call; input cannot arrive before that.
        if (Entity == Entity.Null) return;

        ref var movement = ref GameContext.Instance.World.Get<CharacterMovementComponent>(Entity);
        if (movement.MovementState != Movement.Stopped) return;

        // Handle movement key input
        if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Up)) Move(Direction.Up, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Down)) Move(Direction.Down, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Left)) Move(Direction.Left, ref movement);
        else if (InputManager.Instance.IsScancodePressed(Keyboard.Scancode.Right)) Move(Direction.Right, ref movement);
    }

    private void Move(Direction direction, ref CharacterMovementComponent movement)
    {
        // Update facing direction and notify server.
        if (Direction != direction)
        {
            Direction = direction;
            movement.Direction = direction;
            PlayerSender.Instance.PlayerDirection();
        }

        // Cancel if next tile is blocked.
        if (MapInstance.TileBlocked(X, Y, direction)) return;

        // Choose movement speed (walk/run).
        movement.MovementState = InputManager.Instance.IsKeyPressed(Keyboard.Key.LShift)
            ? Movement.Moving
            : Movement.Walking;

        // Notify server of movement.
        PlayerSender.Instance.PlayerMove(movement.MovementState);

        // Step to the target tile and set the starting pixel offset so the
        // movement system can interpolate back to zero.
        switch (direction)
        {
            case Direction.Up:
                movement.OffsetY = Grid;
                Y--;
                movement.TileY = Y;
                break;
            case Direction.Down:
                movement.OffsetY = (short)(Grid * -1);
                Y++;
                movement.TileY = Y;
                break;
            case Direction.Right:
                movement.OffsetX = (short)(Grid * -1);
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
        // Reset attack state if cooldown expired.
        if (AttackTimer + AttackSpeed < Environment.TickCount)
        {
            AttackTimer = 0;
            Attacking = false;
        }

        // Only proceed if attack key pressed and player may attack.
        if (!InputManager.Instance.IsKeyPressed(Keyboard.Key.LControl)) return;
        if (AttackTimer > 0) return;
        if (TradeView.Panel.Visible) return;
        if (ShopView.Panel.Visible) return;

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
