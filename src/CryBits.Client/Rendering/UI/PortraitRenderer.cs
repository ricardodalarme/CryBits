using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Rendering.UI;

internal sealed class PortraitRenderer(SpriteBatch spriteBatch)
{
    public void DrawFace(short textureNum, Point position) =>
        spriteBatch.Draw(Textures.Faces[textureNum], position);

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

        DrawShadow(position, frameW, frameH);
        spriteBatch.Draw(Textures.Characters[textureNum], recSource, recDestiny, color);
    }

    public void DrawShadow(Point position, int frameW, int frameH)
    {
        var shadowSize = Textures.Shadow.ToSize();
        spriteBatch.Draw(Textures.Shadow, position.X,
            position.Y + frameH - shadowSize.Height + 5, 0, 0,
            frameW, shadowSize.Height);
    }
}
