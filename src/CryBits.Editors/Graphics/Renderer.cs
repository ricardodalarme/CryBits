using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Common;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SysRect = System.Drawing.Rectangle;
using SysPoint = System.Drawing.Point;
using Color = Microsoft.Xna.Framework.Color;

namespace CryBits.Editors.Graphics;

/// <summary>
/// Editor drawing helper. Mirrors the legacy CryBits SFML <c>Renderer</c> surface but operates
/// against a MonoGame <see cref="SpriteBatch"/> and <see cref="Texture2D"/> provided by the
/// hosted game in the editor.
/// </summary>
public class Renderer
{
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _whiteTexture = null!;
    private SpriteFontBase _font = null!;

    public void Attach(SpriteBatch spriteBatch, SpriteFontBase font)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        _whiteTexture.SetData([Color.White]);
    }

    public void Draw(Texture2D texture, SysRect source, SysRect destiny, Color? color = null) =>
        _spriteBatch.Draw(texture, ToMgRect(destiny), ToMgRect(source), color ?? Color.White);

    public void Draw(Texture2D texture, int x, int y, int sourceX, int sourceY,
        int sourceWidth, int sourceHeight, Color? color = null) =>
        Draw(texture,
            new SysRect(sourceX, sourceY, sourceWidth, sourceHeight),
            new SysRect(x, y, sourceWidth, sourceHeight),
            color);

    public void Draw(Texture2D texture, SysRect destiny, Color? color = null) =>
        _spriteBatch.Draw(texture, ToMgRect(destiny), null, color ?? Color.White);

    public void Draw(Texture2D texture, SysPoint point, Color? color = null) =>
        Draw(texture, new SysRect(point.X, point.Y, texture.Width, texture.Height), color);

    public void DrawRectangle(SysRect rectangle, Color? color = null)
    {
        var c = color ?? Color.White;
        var texture = Textures.Grid;
        Draw(texture, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, c);
        Draw(texture, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, c);
        Draw(texture, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, c);
        Draw(texture, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, c);
    }

    public void DrawRectangle(int x, int y, int width, int height, Color? color = null) =>
        DrawRectangle(new SysRect(x, y, width, height), color);

    public void DrawText((int x, int y) position, string text, Color color, TextAlign alignment = TextAlign.Center)
    {
        if (_font == null) return;
        var size = _font.MeasureString(text);
        var drawX = alignment switch
        {
            TextAlign.Center => position.x - size.X / 2,
            TextAlign.Right => position.x - size.X,
            _ => position.x
        };
        var drawY = position.y - size.Y / 2;
        _spriteBatch.DrawString(_font, text, new Vector2(drawX, drawY), color);
    }

    public void DrawTransparentBackground()
    {
        var texture = Textures.Transparent;
        var viewport = _spriteBatch.GraphicsDevice.Viewport;
        for (var x = 0; x <= viewport.Width / texture.Width; x++)
            for (var y = 0; y <= viewport.Height / texture.Height; y++)
                Draw(texture, new SysPoint(texture.Width * x, texture.Height * y));
    }

    private static Rectangle ToMgRect(SysRect r) =>
        new(r.X, r.Y, r.Width, r.Height);
}
