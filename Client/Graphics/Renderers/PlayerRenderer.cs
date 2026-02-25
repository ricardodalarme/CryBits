using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Utils;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal static class PlayerRenderer
{
    /// <summary>
    /// Draw the given player's name and status bars (Sprite is rendered via ECS).
    /// </summary>
    /// <param name="player">Player to render.</param>
    public static void PlayerCharacter(Player player)
    {
        PlayerBars(player);
        CharacterRenderer.CharacterShadow(player.TextureNum,
            new Point(CameraUtils.ConvertX(player.PixelX), CameraUtils.ConvertY(player.PixelY)));
    }

    private static void PlayerBars(Player player)
    {
        var value = player.Vital[(byte)Vital.Hp];

        // No bar needed when full or dead.
        if (value <= 0 || value >= player.MaxVital[(byte)Vital.Hp]) return;

        // Compute the bar width.
        var characterSize = Textures.Characters[player.TextureNum].ToSize();
        var fullWidth = characterSize.Width / AnimationAmountX;
        var width = value * fullWidth / player.MaxVital[(byte)Vital.Hp];

        // Bar position.
        var position = new Point
        {
            X = CameraUtils.ConvertX(player.PixelX),
            Y = CameraUtils.ConvertY(player.PixelY) + characterSize.Height / AnimationAmountY + 4
        };

        Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
