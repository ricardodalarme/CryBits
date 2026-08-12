using CryBits.Client.Framework.Constants;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Framework.Assets;

public static class Textures
{
    // Lazily-initialized texture caches. Use the lazy backing fields so the textures are
    // only loaded when first accessed, AFTER a GraphicsDevice has been registered.
    // Lists are `Texture2D?` so the [0] placeholder slot is null (was a `null!` before).
    private static List<Texture2D?>? _characters;
    private static List<Texture2D?>? _tiles;
    private static List<Texture2D?>? _faces;
    private static List<Texture2D?>? _panoramas;
    private static List<Texture2D?>? _fogs;
    private static List<Texture2D?>? _items;
    private static Texture2D? _weather;
    private static Texture2D? _blank;
    private static Texture2D? _shadow;
    private static Texture2D? _bars;
    private static Texture2D? _equipments;
    private static Texture2D? _blood;
    private static Texture2D? _partyBars;
    private static Texture2D? _directions;
    private static Texture2D? _transparent;
    private static Texture2D? _grid;

    // The active MonoGame GraphicsDevice. Set by the host Game via `Initialize(GraphicsDevice)`
    // before any texture is accessed; cleared by `Reset()` on graphics device teardown.
    private static GraphicsDevice? _device;

    /// <summary>True once a host Game has registered its GraphicsDevice.</summary>
    public static bool IsAvailable => _device != null;

    /// <summary>Register the active MonoGame GraphicsDevice. Safe to call multiple times.</summary>
    public static void Initialize(GraphicsDevice device) => _device = device;

    /// <summary>Drop the cached device. The next texture access will throw until
    /// <see cref="Initialize"/> is called again. Use this when the underlying
    /// GraphicsDevice is recreated (for example, on display mode change).</summary>
    public static void Reset() => _device = null;

    public static List<Texture2D?> Characters => _characters ??= LoadTextures(Directories.TexCharacters.FullName);
    public static List<Texture2D?> Tiles => _tiles ??= LoadTextures(Directories.TexTiles.FullName);
    public static List<Texture2D?> Faces => _faces ??= LoadTextures(Directories.TexFaces.FullName);
    public static List<Texture2D?> Panoramas => _panoramas ??= LoadTextures(Directories.TexPanoramas.FullName);
    public static List<Texture2D?> Fogs => _fogs ??= LoadTextures(Directories.TexFogs.FullName);
    public static List<Texture2D?> Items => _items ??= LoadTextures(Directories.TexItems.FullName);
    public static Texture2D Weather => Get(() => LoadTexture(Directories.TexWeather.FullName + Format), ref _weather);
    public static Texture2D Blank => Get(() => LoadTexture(Directories.TexBlank.FullName + Format), ref _blank);
    public static Texture2D Shadow => Get(() => LoadTexture(Directories.TexShadow.FullName + Format), ref _shadow);
    public static Texture2D Bars => Get(() => LoadTexture(Directories.TexBars.FullName + Format), ref _bars);
    public static Texture2D Equipments => Get(() => LoadTexture(Directories.TexEquipments.FullName + Format), ref _equipments);
    public static Texture2D Blood => Get(() => LoadTexture(Directories.TexBlood.FullName + Format), ref _blood);
    public static Texture2D PartyBars => Get(() => LoadTexture(Directories.TexPartyBars.FullName + Format), ref _partyBars);
    public static Texture2D Directions => Get(() => LoadTexture(Directories.TexDirections.FullName + Format), ref _directions);
    public static Texture2D Transparent => Get(() => LoadTexture(Directories.TexTransparent.FullName + Format), ref _transparent);
    public static Texture2D Grid => Get(() => LoadTexture(Directories.TexGrid.FullName + Format), ref _grid);

    // Texture file extension.
    private const string Format = ".png";

    private static Texture2D Get(Func<Texture2D> load, ref Texture2D? field)
    {
        if (field != null) return field;
        var tex = load();
        field = tex;
        return tex;
    }

    private static Texture2D LoadTexture(string path)
    {
        if (_device == null)
            throw new InvalidOperationException("GraphicsDevice not registered before texture load: " + path);
        return Texture2D.FromFile(_device, path);
    }

    private static List<Texture2D?> LoadTextures(string directory)
    {
        var tempTex = new List<Texture2D?>(1) { null };

        // Load sequentially numbered textures from the directory into the cache.
        for (short i = 1; File.Exists(Path.Combine(directory, i + Format)); i++)
            tempTex.Add(LoadTexture(Path.Combine(directory, i + Format)));

        return tempTex;
    }
}
