using CryBits.Client.Framework.Assets;
using CryBits.Client.Rendering.Entities;
using CryBits.Definitions.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Rendering.UI;

internal sealed class PortraitRenderer(SpriteBatch spriteBatch)
{
    public void DrawFace(short textureNum, Vector2 position) =>
        spriteBatch.Draw(Textures.Faces[textureNum], position, Color.White);

    public void DrawCharacter(short textureNum, Vector2 position, Direction direction, byte column, bool hurt = false)
    {
        var sheet = SpriteSheet.Default;

        if (Textures.Characters[textureNum] is not { } texture) return;
        var frameW = sheet.FrameW(texture.Width);
        var frameH = sheet.FrameH(texture.Height);
        var line = sheet.RowForDirection(direction);

        var recSource = new Rectangle(
            column * frameW,
            line * frameH,
            frameW,
            frameH);

        var recDestiny = new Rectangle((int)position.X, (int)position.Y, recSource.Width, recSource.Height);
        var color = hurt ? new Color(205, 125, 125) : new Color(255, 255, 255);

        DrawShadow(position, frameW, frameH);
        spriteBatch.Draw(texture, recDestiny, recSource, color);
    }

    public void DrawShadow(Vector2 position, int frameW, int frameH)
    {
        var shadow = Textures.Shadow;

        spriteBatch.Draw(shadow,
            new Rectangle((int)position.X, (int)position.Y + frameH - shadow.Height + 5, frameW, shadow.Height),
            Color.White);
    }
}
