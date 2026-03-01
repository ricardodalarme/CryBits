using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class CharacterRenderer(Renderer renderer)
{
    public static CharacterRenderer Instance { get; } = new(Renderer.Instance);

    public void DrawCharacter(short textureNum, Point position, Direction direction, byte column, bool hurt = false)
    {
        Rectangle recSource = new();
        var size = Textures.Characters[textureNum].ToSize();
        var color = new Color(255, 255, 255);

        byte line = direction switch
        {
            Direction.Up => MovementUp,
            Direction.Down => MovementDown,
            Direction.Left => MovementLeft,
            Direction.Right => MovementRight,
            _ => 0
        };

        recSource.X = column * size.Width / AnimationAmountX;
        recSource.Y = line * size.Height / AnimationAmountY;
        recSource.Width = size.Width / AnimationAmountX;
        recSource.Height = size.Height / AnimationAmountY;
        var recDestiny = new Rectangle(position, recSource.Size);

        if (hurt) color = new Color(205, 125, 125);

        DrawShadow(textureNum, position);
        renderer.Draw(Textures.Characters[textureNum], recSource, recDestiny, color);
    }

    public void DrawShadow(short textureNum, Point position)
    {
        Rectangle recSource = new();
        var size = Textures.Characters[textureNum].ToSize();

        recSource.Width = size.Width / AnimationAmountX;
        recSource.Height = size.Height / AnimationAmountY;
        var recDestiny = new Rectangle(position, recSource.Size);

        renderer.Draw(Textures.Shadow, recDestiny.Location.X,
            recDestiny.Location.Y + size.Height / AnimationAmountY - Textures.Shadow.ToSize().Height + 5, 0, 0,
            size.Width / AnimationAmountX, Textures.Shadow.ToSize().Height);
    }
}
