using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Reads keyboard input and translates it into movement and direction changes
/// for the entity tagged with <see cref="InputControlledTag"/>.
///
/// Sending movement/direction packets to the server also happens here so the
/// whole "player wants to move" concern is in one place.
/// </summary>
internal sealed class PlayerInputSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        var localId = ctx.GetLocalPlayer();
        if (localId < 0) return;

        // Only process input when the game screen is active.
        if (Screen.Current != Screens.Game) return;

        if (!ctx.World.TryGet<TransformComponent>(localId, out var transform)) return;
        if (!ctx.World.TryGet<MovementComponent>(localId, out var movement)) return;

        // Movement input is only consumed when no move is already in progress.
        if (movement.Current != Movement.Stopped) return;
        if (!Renders.RenderWindow.HasFocus()) return;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) TryMove(ctx, localId, transform, movement, Direction.Up);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) TryMove(ctx, localId, transform, movement, Direction.Down);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) TryMove(ctx, localId, transform, movement, Direction.Left);
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) TryMove(ctx, localId, transform, movement, Direction.Right);
    }

    private static void TryMove(
        GameContext ctx,
        int entityId,
        TransformComponent transform,
        MovementComponent movement,
        Direction direction)
    {
        // Notify server of direction change before tile-blocking check.
        if (transform.Direction != direction)
        {
            transform.Direction = direction;
            PlayerSender.PlayerDirection(direction);
        }

        if (ctx.CurrentMap == null) return;
        if (ctx.CurrentMap.TileBlocked(ctx, transform.TileX, transform.TileY, direction)) return;

        movement.Current = Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Renders.RenderWindow.HasFocus()
            ? Movement.Moving
            : Movement.Walking;

        // Capture the pre-move position — this is what the server expects to receive
        // so it can validate that the player is still at that tile before moving.
        var oldX = transform.TileX;
        var oldY = transform.TileY;

        // Client-side prediction: advance the local tile and set pixel offset for the slide.
        switch (direction)
        {
            case Direction.Up:
                transform.PixelOffsetY = Grid;
                transform.TileY--;
                break;
            case Direction.Down:
                transform.PixelOffsetY = Grid * -1;
                transform.TileY++;
                break;
            case Direction.Right:
                transform.PixelOffsetX = Grid * -1;
                transform.TileX++;
                break;
            case Direction.Left:
                transform.PixelOffsetX = Grid;
                transform.TileX--;
                break;
        }

        // Send the OLD position so the server can match its own state and call Move().
        PlayerSender.PlayerMove(oldX, oldY, movement.Current);
    }
}
