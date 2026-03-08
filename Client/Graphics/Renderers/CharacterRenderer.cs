using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class CharacterRenderer(Renderer renderer)
{
    public static CharacterRenderer Instance { get; } = new(Renderer.Instance);

    public void DrawFace(short textureNum, Vector2i position) =>
        renderer.Draw(Textures.Faces[textureNum], position);

    public void DrawCharacter(short textureNum, Vector2i position, Direction direction, byte column, bool hurt = false)
    {
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

        var srcW = size.X / AnimationAmountX;
        var srcH = size.Y / AnimationAmountY;
        var recSource = new IntRect(
            new Vector2i(column * srcW, line * srcH),
            new Vector2i(srcW, srcH));
        var recDestiny = new IntRect(position, recSource.Size);

        if (hurt) color = new Color(205, 125, 125);

        DrawShadow(textureNum, position);
        renderer.Draw(Textures.Characters[textureNum], recSource, recDestiny, color);
    }

    public void DrawShadow(short textureNum, Vector2i position)
    {
        var size = Textures.Characters[textureNum].ToSize();
        var frameW = size.X / AnimationAmountX;
        var frameH = size.Y / AnimationAmountY;
        var shadowH = Textures.Shadow.ToSize().Y;

        renderer.Draw(Textures.Shadow, position.X,
            position.Y + frameH - shadowH + 5, 0, 0,
            frameW, shadowH);
    }
}
