using CryBits.Client.Framework.Constants;
using Iguina;
using Iguina.Drivers.Sfml;
using Iguina.Entities;
using SFML.Graphics;
using SFML.System;

namespace CryBits.Editors.Iguina;

internal sealed class IguinaEditorPreview : IDisposable
{
    private readonly UISystem _uiSystem;
    private readonly SfmlRenderer _renderer;
    private readonly RenderTexture _target;
    private Entity? _loadedEntity;

    public RenderTexture Target => _target;
    public UISystem UISystem => _uiSystem;

    public IguinaEditorPreview(int width, int height)
    {
        _target = new RenderTexture(new Vector2u((uint)width, (uint)height));
        var themePath = Directories.UITheme.FullName;
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Graphics", "Fonts", "Georgia.ttf");
        _renderer = new SfmlRenderer(_target, themePath, new Font(fontPath));
        var input = new StubInputProvider();

        var sPath = Path.Combine(themePath, "SystemStyle.json");
        _uiSystem = new UISystem(sPath, _renderer, input);
    }

    public void LoadEntity(Entity entity)
    {
        _loadedEntity?.RemoveSelf();
        _uiSystem.Root.AddChild(entity);
        _loadedEntity = entity;
    }

    public void Clear()
    {
        _loadedEntity?.RemoveSelf();
        _loadedEntity = null;
    }

    public void Draw()
    {
        _target.Clear(Color.Black);
        _uiSystem.Draw();
        _target.Display();
    }

    public void Dispose()
    {
        _target.Dispose();
    }
}
