using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal static class Renders
{
    /// <summary>
    /// SFML render window used for drawing game and UI.
    /// </summary>
    public static RenderWindow RenderWindow;

    /// <summary>
    /// Initialize the render window and attach input event handlers.
    /// </summary>
    public static void Init()
    {
        RenderWindow = new RenderWindow(new VideoMode(new Vector2u(800, 608)), Config.GameName, Styles.Titlebar | Styles.Close,
            State.Windowed);
        RenderWindow.Closed += UI.Window.OnClosed;
        RenderWindow.MouseButtonPressed += UI.Window.OnMouseButtonPressed;
        RenderWindow.MouseMoved += UI.Window.OnMouseMoved;
        RenderWindow.MouseButtonReleased += UI.Window.OnMouseButtonReleased;
        RenderWindow.KeyPressed += UI.Window.OnKeyPressed;
        RenderWindow.KeyReleased += UI.Window.OnKeyReleased;
        RenderWindow.TextEntered += UI.Window.OnTextEntered;
    }

    /// <summary>
    /// Draws a textured rectangle from a source region to a destination rectangle.
    /// </summary>
    /// <param name="texture">Source texture.</param>
    /// <param name="recSource">Region of the texture to draw.</param>
    /// <param name="recDestiny">Destination rectangle on screen.</param>
    /// <param name="color">Optional tint color.</param>
    /// <param name="mode">Optional render state.</param>
    public static void Render(Texture texture, Rectangle recSource, Rectangle recDestiny, object color = null,
        object mode = null)
    {
        var tmpImage = new Sprite(texture)
        {
            TextureRect = new IntRect(new Vector2i(recSource.X, recSource.Y),
                new Vector2i(recSource.Width, recSource.Height)),
            Position = new Vector2f(recDestiny.X, recDestiny.Y),
            Scale = new Vector2f(recDestiny.Width / (float)recSource.Width, recDestiny.Height / (float)recSource.Height)
        };
        if (color != null) tmpImage.Color = (Color)color;

        mode ??= RenderStates.Default;
        RenderWindow.Draw(tmpImage, (RenderStates)mode);
    }

    public static void Render(Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth,
        int sourceHeight, object color = null)
    {
        var source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        var destiny = new Rectangle(x, y, sourceWidth, sourceHeight);

        Render(texture, source, destiny, color);
    }

    public static void Render(Texture texture, Rectangle destiny, object color = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        Render(texture, source, destiny, color);
    }

    public static void Render(Texture texture, Point position, object color = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(position, texture.ToSize());
        Render(texture, source, destiny, color);
    }

    /// <summary>
    /// Draw text at the specified position with optional alignment.
    /// </summary>
    /// <param name="text">Text to draw.</param>
    /// <param name="x">X position in pixels.</param>
    /// <param name="y">Y position in pixels.</param>
    /// <param name="color">Text color.</param>
    /// <param name="alignment">Horizontal alignment.</param>
    public static void DrawText(string text, int x, int y, Color color, TextAlign alignment = TextAlign.Left)
    {
        switch (alignment)
        {
            case TextAlign.Center: x -= MeasureString(text) / 2; break;
            case TextAlign.Right: x -= MeasureString(text); break;
        }

        var tempText = new Text(Fonts.Default, text)
        {
            CharacterSize = 10,
            FillColor = color,
            Position = new Vector2f(x, y),
            OutlineColor = new Color(0, 0, 0, 70),
            OutlineThickness = 1
        };

        RenderWindow.Draw(tempText);
    }

    /// <summary>
    /// Draw text and wrap it to a maximum width. Optionally cuts at word boundaries.
    /// </summary>
    public static void DrawText(string text, int x, int y, Color color, int maxWidth, bool cut = true)
    {
        int messageWidth = MeasureString(text), split = -1;

        if (messageWidth < maxWidth)
            DrawText(text, x, y, color);
        else
            for (var i = 0; i < text.Length; i++)
            {
                split = text[i] switch
                {
                    '-' or '_' or ' ' => i,
                    _ => split
                };

                var tempText = text.Substring(0, i);
                if (MeasureString(tempText) > maxWidth)
                {
                    if (cut && split != -1) tempText = text.Substring(0, split + 1);

                    DrawText(tempText, x, y, color);
                    DrawText(text.Substring(tempText.Length), x, y + 12, color, maxWidth);
                    return;
                }
            }
    }

    /// <summary>
    /// Draw a box using a texture with a fixed margin (nine-slice).
    /// </summary>
    /// <param name="texture">Box texture.</param>
    /// <param name="margin">Inner margin in pixels.</param>
    /// <param name="position">Top-left position.</param>
    /// <param name="size">Box size.</param>
    public static void Render_Box(Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        Render(texture, new Rectangle(new Point(0), new Size(margin, textureWidth)),
            new Rectangle(position, new Size(margin, textureHeight)));
        Render(texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        Render(texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + margin, position.Y),
                new Size(size.Width - margin * 2, textureHeight)));
    }
}
