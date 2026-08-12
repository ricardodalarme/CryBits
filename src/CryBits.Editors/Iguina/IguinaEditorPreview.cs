using CryBits.Client.Framework.Constants;
using CryBits.Editors.Graphics;
using Iguina.Drivers.MonoGame;
using FontStashSharp;
using Iguina;
using Iguina.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Editors.Iguina;

/// <summary>
/// Editor Iguina preview. Renders an Iguina UI into a <see cref="RenderTarget2D"/> using the
/// MonoGame driver and a non-interactive input provider. The texture is then exposed to the
/// editor through <see cref="Target"/>.
/// </summary>
internal sealed class IguinaEditorPreview : IDisposable
{
    private readonly MonoGameRenderer _renderer;
    private readonly FontSystem _fonts = new();
    private readonly SpriteBatch _spriteBatch;
    private Entity? _loadedEntity;

    public RenderTarget2D Target { get; }

    public UISystem UISystem { get; }

    public IguinaEditorPreview(int width = 800, int height = 600, GraphicsDevice? device = null)
    {
        var activeDevice = device ?? EditorGraphics.Device;
        Target = new RenderTarget2D(activeDevice, width, height);
        _spriteBatch = new SpriteBatch(activeDevice);

        var themePath = Directories.UiTheme.FullName;
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Graphics", "Fonts", "Georgia.ttf");
        _fonts.AddFont(File.ReadAllBytes(fontPath));

        _renderer = new MonoGameRenderer(activeDevice, _spriteBatch, _fonts, themePath);
        // Use a stub input (non-interactive preview).
        var input = new StubInputProvider();
        UISystem = new UISystem(Path.Combine(themePath, "SystemStyle.json"), _renderer, input);
    }

    public void LoadEntity(Entity entity)
    {
        _loadedEntity?.RemoveSelf();
        UISystem.Root.AddChild(entity);
        _loadedEntity = entity;
    }

    public void Clear()
    {
        _loadedEntity?.RemoveSelf();
        _loadedEntity = null;
    }

    public void Draw()
    {
        Target.GraphicsDevice.SetRenderTarget(Target);
        Target.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        _renderer.StartFrame();
        UISystem.Draw();
        _renderer.EndFrame();
        Target.GraphicsDevice.SetRenderTarget(null);
    }

    public void Dispose()
    {
        Target.Dispose();
        _spriteBatch.Dispose();
    }
}
