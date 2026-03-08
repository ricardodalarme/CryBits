using System;
using System.Drawing;
using Arch.Core;
using CryBits.Client.Components.Core;
using CryBits.Client.Graphics;
using CryBits.Client.Worlds;
using CryBits.Entities.Map;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;

namespace CryBits.Client.Managers;

/// <summary>
/// Manages the game's 2D camera using an SFML <see cref="View"/>.
///
/// <para>
/// Follows a target ECS entity (defaults to the local player). Setting
/// <see cref="Target"/> to any entity with a <see cref="TransformComponent"/>
/// lets you spectate players, NPCs, or run cutscenes with zero call-site changes.
/// </para>
///
/// <para>
/// <b>How rendering works with this:</b>
/// <list type="number">
///   <item>Call <see cref="BeginWorldDraw"/> – applies the game view so all subsequent
///         draws happen in world space (SFML handles panning automatically).</item>
///   <item>Draw everything in world coordinates.</item>
///   <item>Call <see cref="BeginUIDraw"/> – restores the default view so UI draws
///         at fixed screen positions.</item>
/// </list>
/// </para>
/// </summary>
internal class CameraManager(RenderWindow renderWindow, GameContext context)
{
    public static CameraManager Instance { get; } = new(Renderer.Instance.RenderWindow, GameContext.Instance);

    /// <summary>
    /// The entity the camera follows. Set to <see cref="Entity.Null"/> to
    /// fall back to the local player automatically.
    /// </summary>
    private Entity Target { get; set; } = Entity.Null;

    /// <summary>The SFML view used for world rendering.</summary>
    private View GameView { get; } = new(new FloatRect(new Vector2f(0, 0), new Vector2f(ScreenWidth, ScreenHeight)));

    /// <summary> The render window used for rendering.</summary>
    private RenderWindow RenderWindow = renderWindow;

    /// <summary>
    /// The range of tile indices currently visible, used for culling.
    /// (X/Y = first tile column/row, Width/Height = last tile column/row inclusive)
    /// </summary>
    public Rectangle TileSight { get; private set; }

    /// <summary>
    /// Update the SFML view to track the target entity.
    /// Call once per frame, before <see cref="BeginWorldDraw"/>.
    /// </summary>
    public void Update()
    {
        var world = context.World;

        // Resolve target — fall back to local player.
        var target = Target != Entity.Null ? Target : context.LocalPlayer.Entity;

        if (target == Entity.Null || !world.IsAlive(target)) return;

        ref var transform = ref world.Get<TransformComponent>(target);

        // Centre the view on the target's world-pixel position.
        // Clamp so the view never shows outside the map bounds.
        const float halfW = ScreenWidth / 2f;
        const float halfH = ScreenHeight / 2f;
        const int mapPixelW = Map.Width * Grid;
        const int mapPixelH = Map.Height * Grid;

        var cx = Math.Clamp(transform.X + Grid / 2f, halfW, mapPixelW - halfW);
        var cy = Math.Clamp(transform.Y + Grid / 2f, halfH, mapPixelH - halfH);

        GameView.Center = new Vector2f(cx, cy);

        // Compute visible tile range for culling (used by MapRenderer).
        var left = (int)Math.Max(0, (cx - halfW) / Grid);
        var top = (int)Math.Max(0, (cy - halfH) / Grid);
        var right = (int)Math.Min(Map.Width - 1, (cx + halfW) / Grid);
        var bottom = (int)Math.Min(Map.Height - 1, (cy + halfH) / Grid);

        TileSight = new Rectangle(left, top, right, bottom);
    }

    /// <summary>
    /// Apply the game world view to the render window.
    /// All draws after this call use world coordinates.
    /// </summary>
    public void BeginWorldDraw() => RenderWindow.SetView(GameView);

    /// <summary>
    /// Restore the default view on the render window.
    /// All draws after this call use screen coordinates (for UI, HUD, etc.).
    /// </summary>
    public void BeginUIDraw() => RenderWindow.SetView(RenderWindow.DefaultView);

    /// <summary>
    /// Convert a screen pixel position (e.g. mouse cursor) to world coordinates.
    /// Useful for world-space click detection.
    /// </summary>
    public Vector2f ScreenToWorld(int pixelX, int pixelY) =>
        RenderWindow.MapPixelToCoords(new Vector2i(pixelX, pixelY), GameView);

    /// <summary>
    /// Reset the camera target back to the local player.
    /// </summary>
    public void ResetTarget() => Target = Entity.Null;
}
