using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms.Login;
using CryBits.Editors.Forms.Maps;
using CryBits.Editors.Graphics;
using CryBits.Editors.Graphics.Renderers;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;

namespace CryBits.Editors.Core;

/// <summary>
/// Composition root for the map editor. Owns every shared infrastructure element and
/// hands each component the dependencies it needs through its constructor.
/// </summary>
internal sealed class EditorShell
{
    public EditorShell(DefinitionCatalog catalog, AudioManager audio, Connection connection,
        PackageSender sender, Renderer renderer)
    {
        Catalog = catalog;
        Audio = audio;
        Connection = connection;
        Sender = sender;
        Renderer = renderer;

        MapInstance = new MapInstance(audio, () => MapsWindow);
        TileRenderer = new TileRenderer(renderer);
        MapRenderer = new MapRenderer(renderer, MapInstance, () => MapsWindow);
        Loop = new Loop(this);
    }

    public DefinitionCatalog Catalog { get; }
    public AudioManager Audio { get; }
    public Connection Connection { get; }
    public PackageSender Sender { get; }
    public Renderer Renderer { get; }
    public TileRenderer TileRenderer { get; }
    public MapRenderer MapRenderer { get; }
    public MapInstance MapInstance { get; }
    public Loop Loop { get; }

    /// <summary>Active maps editor window, if any. Cross-thread mutable, signalled volatile.</summary>
    public volatile EditorMapsWindow? MapsWindow;

    /// <summary>Active login window, if any. Cross-thread mutable, signalled volatile.</summary>
    public volatile LoginWindow? LoginWindow;

    /// <summary>Latest measured frame rate, written by the editor loop, read by the UI.</summary>
    public volatile short Fps;

    /// <summary>Set to false to stop the editor loop.</summary>
    public volatile bool Working = true;

    public void Close()
    {
        var waitTimer = Environment.TickCount64;

        Connection.Disconnect();

        while (Connection.IsConnected && Environment.TickCount64 <= waitTimer + 1000)
            Thread.Sleep(10);
    }
}
