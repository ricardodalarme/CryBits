using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Managers;
using CryBits.Client.UI.Game;
using SFML.Window;

namespace CryBits.Client.UI;

internal class Window(InputManager inputManager, AudioManager audioManager)
{
    public static Window Instance { get; } = new(InputManager.Instance, AudioManager.Instance);

    private const int DoubleClickIntervalMs = 142;
    private long _doubleClickTimer;

    public void Bind()
    {
        inputManager.MouseButtonPressed += OnMouseButtonPressed;
        inputManager.MouseButtonReleased += OnMouseButtonReleased;
        inputManager.MouseMoved += OnMouseMoved;
        inputManager.KeyReleased += OnKeyReleased;
    }

    public void OnClosed(object sender, EventArgs e)
    {
        if (GameState.CurrentScreen == ScreenType.Game)
            Connection.Instance.Disconnect();
        else
            Client.Game.Working = false;
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        _doubleClickTimer = Environment.TickCount64 + DoubleClickIntervalMs;
    }

    private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        _doubleClickTimer = Environment.TickCount64;
        GameScreen.InventoryChange = 0;
        GameScreen.HotbarChange = -1;
    }

    private void OnMouseMoved(object sender, MouseMoveEventArgs e) { }

    private void OnKeyReleased(object sender, KeyEventArgs e)
    {
        if (GameState.CurrentScreen == ScreenType.Game)
            GameState.FireGameKeyReleased(e);
    }

    public void OpenMenu()
    {
        audioManager.StopAllSounds();
        if (Options.Instance.Musics) audioManager.PlayMusic(Musics.Menu);
        GameState.CurrentScreen = ScreenType.Menu;
    }
}
