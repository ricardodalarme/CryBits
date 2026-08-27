using CryBits.Client.Framework.Assets;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Editors.Graphics;

/// <summary>
/// Manages the single off-screen <see cref="GraphicsDevice"/> and <see cref="SpriteBatch"/>
/// shared by all editor windows in the <c>CryBits.Editors</c> process.
/// </summary>
internal static class EditorGraphics
{
    private static OffscreenGameHost? _hostGame;

    public static GraphicsDevice Device { get; private set; } = null!;
    public static SpriteBatch SpriteBatch { get; private set; } = null!;
    public static Renderer Renderer { get; private set; } = null!;
    public static FontSystem Fonts { get; private set; } = null!;

    public static void Initialize()
    {
        if (_hostGame != null) return;

        _hostGame = new OffscreenGameHost();
        _hostGame.InitializeHost();

        Device = _hostGame.GraphicsDevice;
        SpriteBatch = new SpriteBatch(Device);

        Fonts = new FontSystem();
        var fontPath = Path.Combine(AppContext.BaseDirectory, "Graphics", "Fonts", "Georgia.ttf");
        if (File.Exists(fontPath))
            Fonts.AddFont(File.ReadAllBytes(fontPath));

        Renderer = new Renderer();
        Renderer.Attach(SpriteBatch, Fonts.GetFont(10));

        Program.SharedDevice = Device;
        Textures.Initialize(Device);
    }

    public static void Tick()
    {
        _hostGame?.RunOneFrame();
    }

    private sealed class OffscreenGameHost : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        public OffscreenGameHost()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1,
                PreferredBackBufferHeight = 1
            };
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
        }

        public void InitializeHost()
        {
            RunOneFrame();
        }
    }
}
