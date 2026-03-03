using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Hotbar;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Movement;
using CryBits.Client.Components.Player;
using CryBits.Client.Components.Trade;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates a new player entity.
/// </summary>
internal static class PlayerSpawner
{
    public static Entity Spawn(World world, Player player, byte x, byte y, Direction direction)
    {
        var texture = Textures.Characters[player.TextureNum];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var textColor = player == Player.Me ? Color.Yellow : Color.White;

        var vitalsComponent = new VitalsComponent();
        player.Vital.CopyTo(vitalsComponent.Current, 0);
        player.MaxVital.CopyTo(vitalsComponent.Max, 0);

        return world.Create(
            new TransformComponent(x * Globals.Grid, y * Globals.Grid),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond },
            new CharacterStateComponent { Direction = direction },
            new DamageTintComponent(),
            new ShadowComponent(Textures.Shadow),
            new PlayerTagComponent(),
            vitalsComponent,
            new InventoryComponent(),
            new HotbarComponent(),
            new LevelComponent(),
            new TradeComponent(),
            new TextComponent(player.Name, textColor, frameWidth / 2, -frameHeight / 2)
        );
    }
}
