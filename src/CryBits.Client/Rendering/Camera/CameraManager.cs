using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Rendering.Camera;

/// <summary>
/// Holds the camera transform and the visible tile range used by tilemap culling. The
/// transform is applied to the wrapped SpriteBatch via <see cref="ApplyFrame"/>.
/// </summary>
internal class CameraManager(SpriteBatch spriteBatch)
{
    /// <summary>
    /// The range of tile indices currently visible, used for culling by <see cref="Map.TilemapRenderer"/>.
    /// (X/Y = first tile column/row, Width/Height = last tile column/row inclusive)
    /// </summary>
    public Rectangle TileSight { get; private set; }

    /// <summary>Current world transform; applied by <see cref="RenderPipeline"/>.</summary>
    public Matrix WorldTransform { get; private set; } = Matrix.Identity;

    /// <summary>
    /// Apply a computed camera frame. Called once per tick by <see cref="Systems.Core.CameraSystem"/>.
    /// </summary>
    public void ApplyFrame(Vector2 center, Rectangle tileSight)
    {
        TileSight = tileSight;
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        WorldTransform = Matrix.CreateTranslation(
            -center.X + viewport.Width / 2f,
            -center.Y + viewport.Height / 2f,
            0f);
    }
}
