using CryBits.Client.Rendering.Camera;
using CryBits.Client.Rendering.Map;
using CryBits.Client.UI;
using CryBits.Definitions.Maps;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Rendering;

internal sealed class RenderPipeline(
    SpriteBatch spriteBatch,
    CameraManager cameraManager,
    TilemapRenderer tilemapRenderer,
    UiContext uiContext,
    IEnumerable<IRenderer> groundRenderers,
    IEnumerable<IRenderer> fringeRenderers)
{
    public void Present()
    {
        spriteBatch.RenderWindow.Clear(Color.Black);

        InGame();

        DrawUI();

        spriteBatch.RenderWindow.Display();
    }

    private void InGame()
    {
        if (uiContext.CurrentScreen != ScreenType.Game) return;

        cameraManager.BeginWorldDraw();

        tilemapRenderer.DrawPanorama();
        tilemapRenderer.DrawLayer(Layer.Ground);

        foreach (var renderer in groundRenderers) renderer.Render();

        tilemapRenderer.DrawLayer(Layer.Fringe);

        foreach (var renderer in fringeRenderers) renderer.Render();
    }

    private void DrawUI()
    {
        cameraManager.BeginUIDraw();

        uiContext.Draw();

        var uiTarget = uiContext.Target;
        if (uiTarget != null)
        {
            var sprite = new Sprite(uiTarget.Texture);
            spriteBatch.RenderWindow.Draw(sprite);
        }

        uiContext.PostDraw?.Invoke();
    }
}
