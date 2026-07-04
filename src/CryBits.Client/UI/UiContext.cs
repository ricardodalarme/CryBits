using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Framework.UI;
using Iguina;
using Iguina.Drivers.Sfml;
using Iguina.Entities;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics.CodeAnalysis;
namespace CryBits.Client.UI;

public enum ScreenType
{
    Menu,
    Game
}

internal sealed class UiContext : IDisposable
{
    private Entity? _currentScreen;

    public RenderTexture? Target { get; private set; }
    public UISystem? UISystem { get; private set; }
    public Dictionary<string, Entity> Registry { get; } = [];
    public ScreenType CurrentScreen { get; set; }

    public Action? PostDraw;

    public T Get<T>(string key) where T : Entity => (T)Registry[key];

    public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T entity) where T : Entity
    {
        if (Registry.TryGetValue(key, out var raw) && raw is T typed)
        {
            entity = typed;
            return true;
        }
        entity = null;
        return false;
    }

    public void Initialize(uint width, uint height, RenderWindow window)
    {
        var themePath = Directories.UITheme.FullName;
        if (!Directory.Exists(themePath)) return;

        Target = new RenderTexture(new Vector2u(width, height));

        var fontPath = Path.Combine(AppContext.BaseDirectory, "Graphics", "Fonts", "Georgia.ttf");
        var renderer = new SfmlRenderer(Target, themePath, new Font(fontPath));

        var input = new SfmlInputProvider(window);
        var sPath = Path.Combine(themePath, "SystemStyle.json");

        UISystem = new UISystem(sPath, renderer, input);
    }

    public void LoadScreen(string screenName)
    {
        if (UISystem == null) return;

        var config = InterfaceRepository.Load(Path.Combine(Directories.UITheme.FullName, "Layout.json"));
        var screenElement = config.Screens.FirstOrDefault(s => s.Name == screenName);
        if (screenElement == null) return;

        Clear();
        var (panel, reg) = LayoutBuilder.BuildScreen(UISystem, screenElement);
        UISystem.Root.AddChild(panel);
        _currentScreen = panel;

        Registry.Clear();
        foreach (var (k, v) in reg)
            Registry[k] = v;
        CurrentScreen = screenName switch
        {
            "Menu" => ScreenType.Menu,
            "Game" => ScreenType.Game,
            _ => ScreenType.Menu
        };
    }

    public void Clear()
    {
        _currentScreen?.RemoveSelf();
        _currentScreen = null;
        Registry.Clear();
    }

    public void Update(float deltaTime)
    {
        UISystem?.Update(deltaTime);
    }

    public void Draw()
    {
        if (Target == null || UISystem == null) return;
        Target.Clear(new Color(0, 0, 0, 0));
        UISystem.Draw();
        Target.Display();
    }

    public void Dispose()
    {
        Target?.Dispose();
    }
}
