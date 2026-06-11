using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Editors.Graphics;

internal class Renderer
{
    public static Renderer Instance { get; } = new();

    public void Draw(IRenderTarget window, Texture texture, Rectangle source, Rectangle destiny,
        Color? color = null)
    {
        var tmpImage = new Sprite(texture)
        {
            TextureRect = new IntRect(new Vector2i(source.X, source.Y), new Vector2i(source.Width, source.Height)),
            Position = new Vector2f(destiny.X, destiny.Y),
            Scale = new Vector2f(destiny.Width / (float)source.Width, destiny.Height / (float)source.Height),
            Color = color ?? Color.White
        };

        window.Draw(tmpImage, RenderStates.Default);
    }

    public void Draw(IRenderTarget window, Texture texture, int x, int y, int sourceX, int sourceY,
        int sourceWidth, int sourceHeight, Color? color = null)
    {
        var source = new Rectangle(new Point(sourceX, sourceY), new Size(sourceWidth, sourceHeight));
        var destiny = new Rectangle(new Point(x, y), new Size(sourceWidth, sourceHeight));

        Draw(window, texture, source, destiny, color);
    }

    public void Draw(IRenderTarget window, Texture texture, Rectangle destiny, Color? color = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        Draw(window, texture, source, destiny, color);
    }

    public void Draw(IRenderTarget window, Texture texture, Point point, Color? color = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(point, texture.ToSize());

        Draw(window, texture, source, destiny, color);
    }

    public void DrawRectangle(IRenderTarget window, Rectangle rectangle, Color? color = null)
    {
        Draw(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, color);
        Draw(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, color);
        Draw(window, Textures.Grid, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, color);
        Draw(window, Textures.Grid, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, color);
    }

    public void DrawRectangle(IRenderTarget window, int x, int y, int width, int height, Color? color = null)
    {
        DrawRectangle(window, new Rectangle(x, y, width, height), color);
    }

    public void DrawBox(IRenderTarget window, Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        Draw(window, texture, new Rectangle(new Point(0), new Size(margin, textureWidth)),
            new Rectangle(position, new Size(margin, textureHeight)));
        Draw(window, texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        Draw(window, texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + margin, position.Y),
                new Size(size.Width - margin * 2, textureHeight)));
    }

    public void DrawText(IRenderTarget window, string text, int x, int y, Color color,
        TextAlign alignment = TextAlign.Left)
    {
        var tempText = new Text(Fonts.Default, text)
        {
            CharacterSize = 10,
            FillColor = color,
            OutlineColor = new Color(0, 0, 0, 70),
            OutlineThickness = 1
        };

        var width = (int)tempText.GetLocalBounds().Width;
        var drawX = alignment switch
        {
            TextAlign.Center => x - width / 2,
            TextAlign.Right => x - width,
            _ => x
        };

        tempText.Position = new Vector2f(drawX, y);
        window.Draw(tempText);
    }
}
