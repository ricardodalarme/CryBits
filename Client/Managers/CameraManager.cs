using CryBits.Client.Graphics;
using SFML.Graphics;
using SFML.System;
using System.Drawing;
using static CryBits.Globals;

namespace CryBits.Client.Managers;

/// <summary>
/// Owns the SFML <see cref="View"/> that controls world-space rendering.
/// </summary>
internal class CameraManager(RenderWindow renderWindow)
{
    public static CameraManager Instance { get; } = new(Renderer.Instance.RenderWindow);

    /// <summary>The SFML view used for world rendering.</summary>
    private readonly View _gameView = new(new FloatRect(new Vector2f(0, 0), new Vector2f(ScreenWidth, ScreenHeight)));

    /// <summary>The render window used for rendering.</summary>
    private readonly RenderWindow _renderWindow = renderWindow;

    /// <summary>
    /// The range of tile indices currently visible, used for culling by <see cref="Graphics.Renderers.MapRenderer"/>.
    /// (X/Y = first tile column/row, Width/Height = last tile column/row inclusive)
    /// </summary>
    public Rectangle TileSight { get; private set; }

    /// <summary>
    /// Apply a computed camera frame. Called once per tick by
    /// <see cref="Systems.Core.CameraSystem"/>.
    /// </summary>
    public void ApplyFrame(Vector2f center, Rectangle tileSight)
    {
        _gameView.Center = center;
        TileSight = tileSight;
    }

    /// <summary>
    /// Apply the game world view to the render window.
    /// All draws after this call use world coordinates.
    /// </summary>
    public void BeginWorldDraw() => _renderWindow.SetView(_gameView);

    /// <summary>
    /// Restore the default view on the render window.
    /// All draws after this call use screen coordinates (for UI, HUD, etc.).
    /// </summary>
    public void BeginUIDraw() => _renderWindow.SetView(_renderWindow.DefaultView);
}
