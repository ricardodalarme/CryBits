using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class CharacterRenderer(Renderer renderer)
{
    public void DrawFace(short textureNum, Point position) =>
        renderer.Draw(Textures.Faces[textureNum], position);

    public void DrawCharacter(short textureNum, Point position, Direction direction, byte column, bool hurt = false)
    {
        var sheet = SpriteSheet.Default;
        var size = Textures.Characters[textureNum].ToSize();
        var frameW = sheet.FrameW(size.Width);
        var frameH = sheet.FrameH(size.Height);
        var line = sheet.RowForDirection(direction);

        var recSource = new Rectangle(
            column * frameW,
            line * frameH,
            frameW,
            frameH);

        var recDestiny = new Rectangle(position, recSource.Size);
        var color = hurt ? new Color(205, 125, 125) : new Color(255, 255, 255);

        DrawShadow(textureNum, position, frameW, frameH);
        renderer.Draw(Textures.Characters[textureNum], recSource, recDestiny, color);
    }

    public void DrawShadow(short textureNum, Point position, int frameW, int frameH)
    {
        var shadowSize = Textures.Shadow.ToSize();
        renderer.Draw(Textures.Shadow, position.X,
            position.Y + frameH - shadowSize.Height + 5, 0, 0,
            frameW, shadowSize.Height);
    }
}
