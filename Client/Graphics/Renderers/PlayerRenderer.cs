using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class PlayerRenderer(Renderer renderer, CharacterRenderer characterRenderer)
{
    public static PlayerRenderer Instance { get; } = new(Renderer.Instance, CharacterRenderer.Instance);

    public void PlayerCharacter(Player player)
    {
        DrawBars(player);
        characterRenderer.DrawShadow(player.TextureNum,
            new Point(player.PixelX, player.PixelY));
    }

    private void DrawBars(Player player)
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

        renderer.Draw(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        renderer.Draw(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
