using CryBits.Client.Framework.Constants;
using Iguina;
using Iguina.Drivers.Sfml;
using Iguina.Entities;
using SFML.Graphics;
using SFML.System;

namespace CryBits.Editors.Iguina;

internal sealed class IguinaEditorPreview : IDisposable
{
    private readonly SfmlRenderer _renderer;
    private Entity? _loadedEntity;

    public RenderTexture Target { get; }

    public UISystem UISystem { get; }

    public IguinaEditorPreview(int width, int height)
    {
        Target = new RenderTexture(new Vector2u((uint)width, (uint)height));
        var themePath = Directories.UITheme.FullName;
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Graphics", "Fonts", "Georgia.ttf");
        _renderer = new SfmlRenderer(Target, themePath, new Font(fontPath));
        var input = new StubInputProvider();

        var sPath = Path.Combine(themePath, "SystemStyle.json");
        UISystem = new UISystem(sPath, _renderer, input);
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
        Target.Clear(Color.Black);
        UISystem.Draw();
        Target.Display();
    }

    public void Dispose()
    {
        Target.Dispose();
    }
}
