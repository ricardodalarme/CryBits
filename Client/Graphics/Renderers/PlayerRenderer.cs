using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal static class PlayerRenderer
{
    /// <summary>
    /// Draw the given player's shadow and health bar at their world position.
    /// Draws in world space — the SFML view handles panning.
    /// </summary>
    public static void PlayerCharacter(Player player)
    {
        PlayerBars(player);
        CharacterRenderer.CharacterShadow(player.TextureNum,
            new Point(player.PixelX, player.PixelY));
    }

    private static void PlayerBars(Player player)
    {
        var value = player.Vital[(byte)Vital.Hp];

        if (value <= 0 || value >= player.MaxVital[(byte)Vital.Hp]) return;

        var characterSize = Textures.Characters[player.TextureNum].ToSize();
        var fullWidth = characterSize.Width / AnimationAmountX;
        var width = value * fullWidth / player.MaxVital[(byte)Vital.Hp];

        var position = new Point
        {
            X = player.PixelX,
            Y = player.PixelY + characterSize.Height / AnimationAmountY + 4
        };

        Renderer.Draw(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renderer.Draw(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
