using CryBits.Client.Framework.Persistence.Repositories;

namespace CryBits.Editors.ViewModels;

/// <remarks>
/// All static members are accessed exclusively on the UI thread (Avalonia <c>DispatcherTimer</c>
/// and event handlers), matching the original window-static pattern they replace.
/// </remarks>
internal sealed class EditorTilesViewModel : ViewModelBase
{
    /// <summary>Currently selected tile sheet index. Consumed by <c>Renders.EditorTileRT()</c>.</summary>
    public static int ScrollTile { get; set; } = 1;

    /// <summary>Horizontal tile sheet scroll offset. Consumed by <c>Renders.EditorTileRT()</c>.</summary>
    public static int ScrollX { get; set; }

    /// <summary>Vertical tile sheet scroll offset. Consumed by <c>Renders.EditorTileRT()</c>.</summary>
    public static int ScrollY { get; set; }

    /// <summary>
    /// Whether the tile editor is in Attributes mode (true) or Dir-Block mode (false).
    /// Consumed by <c>Renders.EditorTileRT()</c>.
    /// </summary>
    public static bool ModeAttributes { get; set; } = true;

    public void Save()
    {
        TileRepository.WriteAll();
    }
}
