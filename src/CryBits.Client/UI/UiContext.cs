using AssetManagementBase;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using System.Diagnostics.CodeAnalysis;

namespace CryBits.Client.UI;

internal class UiContext
{
    public Desktop Desktop { get; }
    public Dictionary<string, Widget> Registry { get; } = new(StringComparer.OrdinalIgnoreCase);
    public ScreenType CurrentScreen { get; set; }
    public AssetManager AssetManager { get; }

    public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T entity) where T : Widget
    {
        if (Registry.TryGetValue(key, out var raw) && raw is T typed)
        {
            entity = typed;
            return true;
        }

        entity = null;
        return false;
    }

    public T Get<T>(string key) where T : Widget
    {
        if (TryGet<T>(key, out var entity))
            return entity;
        throw new KeyNotFoundException($"Widget with key '{key}' was not found in UiContext Registry.");
    }

    public UiContext(Microsoft.Xna.Framework.Game game, GraphicsDevice device, int width, int height)
    {
        MyraEnvironment.Game = game;
        AssetManager = AssetManager.CreateFileAssetManager(Directories.UiTheme.FullName);
        Desktop = new Desktop
        {
            BoundsFetcher = () => device.Viewport.Bounds
        };
    }

    public void ClearKeyboardFocus()
    {
        Desktop.FocusedKeyboardWidget = null;
    }

    public void LoadScreen(string screenName)
    {
        var themePath = Directories.UiTheme.FullName;
        var filePath = Path.Combine(themePath, $"{screenName}.xmmp");
        if (!File.Exists(filePath)) return;

        Clear();
        var xml = File.ReadAllText(filePath);
        var project = Project.LoadFromXml(xml, AssetManager);
        if (project.Root != null)
        {
            Desktop.Root = project.Root;
            PopulateRegistry(project.Root);
        }
    }

    private void PopulateRegistry(Widget widget)
    {
        if (!string.IsNullOrEmpty(widget.Id))
        {
            Registry[widget.Id] = widget;
        }

        if (widget is Container container)
        {
            foreach (var child in container.Widgets)
            {
                PopulateRegistry(child);
            }
        }
    }

    public void Render()
    {
        Desktop.Render();
    }

    public void Clear()
    {
        ClearKeyboardFocus();
        Registry.Clear();
        Desktop.Root = null;
    }
}
