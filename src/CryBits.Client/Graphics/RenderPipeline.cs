using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Systems;
using CryBits.Client.UI;
using CryBits.Definitions.Maps;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal sealed class RenderPipeline(
    Renderer renderer,
    CameraManager cameraManager,
    MapRenderer mapRenderer,
    SystemScheduler scheduler,
    UiContext uiContext)
{
    public void Present()
    {
        renderer.RenderWindow.Clear(Color.Black);

        InGame();

        DrawUI();

        renderer.RenderWindow.Display();
    }

    private void InGame()
    {
        if (uiContext.CurrentScreen != ScreenType.Game) return;

        cameraManager.BeginWorldDraw();

        mapRenderer.DrawPanorama();
        mapRenderer.DrawLayer(Layer.Ground);
        scheduler.Ground.Render();

        mapRenderer.DrawLayer(Layer.Fringe);

        scheduler.Fringe.Render();
    }

    private void DrawUI()
    {
        cameraManager.BeginUIDraw();

        uiContext.Draw();

        var uiTarget = uiContext.Target;
        if (uiTarget != null)
        {
            var sprite = new Sprite(uiTarget.Texture);
            renderer.RenderWindow.Draw(sprite);
        }

        uiContext.PostDraw?.Invoke();
    }
}
