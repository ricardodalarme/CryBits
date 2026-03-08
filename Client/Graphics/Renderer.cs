using CryBits.Client.Framework.Graphics;
using CryBits.Client.Managers;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

/// <summary>
/// Central rendering service. Owns the <see cref="RenderWindow"/> and exposes
/// a thin API for drawing sprites and text.
/// </summary>
internal sealed class Renderer
{
    public static Renderer Instance { get; } = new();

    /// <summary>The SFML render window.</summary>
    public RenderWindow RenderWindow { get; private set; } = null!;

    // Reused across every Draw() call — avoids per-frame heap allocations.
    // SFML Sprite is a reference type whose state is fully overwritten before each draw.
    // Initialized lazily on the first Draw() call (Sprite requires a Texture argument).
    private Sprite? _spriteCache;

    // Reused across every DrawText() call for the same reason.
    // Initialized in Init() once the font is available.
    private Text _textCache = null!;

    /// <summary>
    /// Create the render window and wire up input / focus events.
    /// </summary>
    public void Init()
    {
        RenderWindow = new RenderWindow(
            new VideoMode(new Vector2u((uint)ScreenWidth, (uint)ScreenHeight)),
            Config.GameName,
            Styles.Titlebar | Styles.Close,
            State.Windowed
        );

        // VSync — prevents tearing and GPU spin.
        RenderWindow.SetVerticalSyncEnabled(true);

        // Pre-allocate the text cache now that the font is accessible.
        _textCache = new Text(Fonts.Default, string.Empty)
        {
            CharacterSize = 10,
            OutlineColor = new Color(0, 0, 0, 70),
            OutlineThickness = 1
        };

        RenderWindow.Closed += UI.Window.OnClosed;
        RenderWindow.LostFocus += (_, _) => InputManager.Instance.IsFocused = false;
        RenderWindow.GainedFocus += (_, _) => InputManager.Instance.IsFocused = true;

        InputManager.Instance.BindEvents(RenderWindow);
    }

    /// <summary>
    /// Draws a textured rectangle from a source region to a destination rectangle.
    /// </summary>
    /// <param name="texture">Source texture.</param>
    /// <param name="recSource">Region of the texture to draw.</param>
    /// <param name="recDestiny">Destination rectangle on screen.</param>
    /// <param name="color">Optional tint color.</param>
    public void Draw(Texture texture, IntRect recSource, IntRect recDestiny, Color? color = null)
    {
        // Lazy-initialize: Sprite ctor requires a Texture in SFML.Net 3+.
        _spriteCache ??= new Sprite(texture);
        _spriteCache.Texture = texture;
        _spriteCache.TextureRect = new IntRect(recSource.Position, recSource.Size);
        _spriteCache.Position = new Vector2f(recDestiny.Position.X, recDestiny.Position.Y);
        _spriteCache.Scale = new Vector2f(
            recDestiny.Size.X / (float)recSource.Size.X,
            recDestiny.Size.Y / (float)recSource.Size.Y);

        // Always reset colour — the cache is shared, so a previous tint would bleed through.
        _spriteCache.Color = color ?? Color.White;
        RenderWindow.Draw(_spriteCache);
    }

    public void Draw(Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth,
        int sourceHeight, Color? color = null)
    {
        var source = new IntRect(new Vector2i(sourceX, sourceY), new Vector2i(sourceWidth, sourceHeight));
        var destiny = new IntRect(new Vector2i(x, y), new Vector2i(sourceWidth, sourceHeight));

        Draw(texture, source, destiny, color);
    }

    public void Draw(Texture texture, IntRect destiny, Color? color = null)
    {
        var source = new IntRect(new Vector2i(0, 0), texture.ToSize());
        Draw(texture, source, destiny, color);
    }

    public void Draw(Texture texture, Vector2i position, Color? color = null)
    {
        var size = texture.ToSize();
        var source = new IntRect(new Vector2i(0, 0), size);
        var destiny = new IntRect(position, size);
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
        switch (alignment)
        {
            case TextAlign.Center: x -= MeasureString(text) / 2; break;
            case TextAlign.Right: x -= MeasureString(text); break;
        }

        _textCache.DisplayedString = text;
        _textCache.FillColor = color;
        _textCache.Position = new Vector2f(x, y);

        RenderWindow.Draw(_textCache);
    }

    /// <summary>
    /// Draw text and wrap it to a maximum width. Optionally cuts at word boundaries.
    /// </summary>
    public void DrawText(string text, int x, int y, Color color, int maxWidth, bool cut = true)
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
    public void DrawBox(Texture texture, byte margin, Vector2i position, Vector2i size)
    {
        var texSize = texture.ToSize();

        // Left cap
        Draw(texture, new IntRect(new Vector2i(0, 0), new Vector2i(margin, texSize.Y)),
            new IntRect(position, new Vector2i(margin, texSize.Y)));
        // Right cap
        Draw(texture, new IntRect(new Vector2i(texSize.X - margin, 0), new Vector2i(margin, texSize.Y)),
            new IntRect(new Vector2i(position.X + size.X - margin, position.Y), new Vector2i(margin, texSize.Y)));
        // Horizontal stretch (middle)
        Draw(texture, new IntRect(new Vector2i(margin, 0), new Vector2i(texSize.X - margin * 2, texSize.Y)),
            new IntRect(new Vector2i(position.X + margin, position.Y),
                new Vector2i(size.X - margin * 2, texSize.Y)));
    }
}
