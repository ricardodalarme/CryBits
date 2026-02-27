using Arch.Core;
using CryBits.Client.Components;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates a new player entity.
/// </summary>
internal static class PlayerSpawner
{
    public static Entity Spawn(World world, Player player)
    {
        var texture = Textures.Characters[player.TextureNum];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var textColor = player == Player.Me ? Color.Yellow : Color.White;

        return world.Create(
            new TransformComponent(player.PixelX, player.PixelY),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new CharacterStateComponent { Direction = player.Direction },
            new DamageTintComponent(),
            new TextComponent(player.Name, textColor, frameWidth / 2, -frameHeight / 2)
        );
    }
}
