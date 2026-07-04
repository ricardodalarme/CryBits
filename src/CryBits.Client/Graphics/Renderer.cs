using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Network;
using CryBits.Client.Managers;
using CryBits.Definitions.Common;
using CryBits.Client.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Drawing;
using static CryBits.Definitions.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

/// <summary>
/// Central rendering service. Owns the <see cref="RenderWindow"/> and exposes
/// a thin API for drawing sprites and text.
/// </summary>
internal sealed class Renderer(InputManager inputManager)
{
    private UiContext? _uiContext;
    internal Connection? Connection { get; set; }

    public RenderWindow RenderWindow { get; private set; } = null!;

    private Sprite? _spriteCache;
    private Text _textCache = null!;

    public void Init(UiContext uiContext)
    {
        _uiContext = uiContext;

        RenderWindow = new RenderWindow(
            new VideoMode(new Vector2u((uint)ScreenWidth, (uint)ScreenHeight)),
            Config.GameName,
            Styles.Titlebar | Styles.Close,
            State.Windowed
        );

        RenderWindow.SetVerticalSyncEnabled(true);

        _textCache = new Text(Fonts.Default, string.Empty)
        {
            CharacterSize = 10,
            OutlineColor = new Color(0, 0, 0, 70),
            OutlineThickness = 1
        };

        RenderWindow.Closed += (_, _) =>
        {
            if (_uiContext.CurrentScreen == ScreenType.Game)
                Connection?.Disconnect();
            else
                Game.Working = false;
        };
        RenderWindow.LostFocus += (_, _) => inputManager.IsFocused = false;
        RenderWindow.GainedFocus += (_, _) => inputManager.IsFocused = true;

        inputManager.BindEvents(RenderWindow);
    }

    /// <summary>
    /// Draws a textured rectangle from a source region to a destination rectangle.
    /// </summary>
    /// <param name="texture">Source texture.</param>
    /// <param name="recSource">Region of the texture to draw.</param>
    /// <param name="recDestiny">Destination rectangle on screen.</param>
    /// <param name="color">Optional tint color.</param>
    public void Draw(Texture texture, Rectangle recSource, Rectangle recDestiny, Color? color = null)
    {
        // Lazy-initialize: Sprite ctor requires a Texture in SFML.Net 3+.
        _spriteCache ??= new Sprite(texture);
        _spriteCache.Texture = texture;
        _spriteCache.TextureRect = new IntRect(
            new Vector2i(recSource.X, recSource.Y),
            new Vector2i(recSource.Width, recSource.Height));
        _spriteCache.Position = new Vector2f(recDestiny.X, recDestiny.Y);
        _spriteCache.Scale = new Vector2f(
            recDestiny.Width / (float)recSource.Width,
            recDestiny.Height / (float)recSource.Height);

        // Always reset colour — the cache is shared, so a previous tint would bleed through.
        _spriteCache.Color = color ?? Color.White;
        RenderWindow.Draw(_spriteCache);
    }

    public void Draw(Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth,
        int sourceHeight, Color? color = null)
    {
        var source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        var destiny = new Rectangle(x, y, sourceWidth, sourceHeight);

        Draw(texture, source, destiny, color);
    }

    public void Draw(Texture texture, Point position, Color? color = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(position, texture.ToSize());
        Draw(texture, source, destiny, color);
    }

    /// <summary>
    /// Draw text at the specified position with optional alignment.
    /// </summary>
    /// <param name="text">Text to draw.</param>
    /// <param name="x">X position in pixels.</param>
    /// <param name="y">Y position in pixels.</param>
    /// <param name="color">Text color.</param>
    /// <param name="alignment">Horizontal alignment.</param>
    public void DrawText(string text, int x, int y, Color color, TextAlign alignment = TextAlign.Left)
    {
        _textCache.DisplayedString = text;

        switch (alignment)
        {
            case TextAlign.Center: x -= (int)_textCache.GetLocalBounds().Width / 2; break;
            case TextAlign.Right: x -= (int)_textCache.GetLocalBounds().Width; break;
        }

        _textCache.FillColor = color;
        _textCache.Position = new Vector2f(x, y);

        RenderWindow.Draw(_textCache);
    }
}
