using System;
using System.Drawing;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Managers;
using CryBits.Client.Network;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using SFML.Window;

namespace CryBits.Client.UI;

/// <summary>
/// Handles window-level and UI-level input events.
/// Game-screen key bindings live in <see cref="Logic.GameInput"/>.
/// </summary>
internal static class Window
{
    private static readonly InputManager _inputManager = InputManager.Instance;
    private static readonly NetworkClient _networkClient = NetworkClient.Instance;
    private static readonly AudioManager _audioManager = AudioManager.Instance;

    /// <summary>Interval in milliseconds within which two clicks count as a double-click.</summary>
    private const int DoubleClickIntervalMs = 142;

    private static int _doubleClickTimer;

    /// <summary>Current mouse pointer position in screen coordinates.</summary>
    private static Point Mouse;

    public static void Bind()
    {
        _inputManager.MouseButtonPressed += OnMouseButtonPressed;
        _inputManager.MouseButtonReleased += OnMouseButtonReleased;
        _inputManager.MouseMoved += OnMouseMoved;
        _inputManager.KeyPressed += OnKeyPressed;
        _inputManager.KeyReleased += OnKeyReleased;
        _inputManager.TextEntered += OnTextEntered;
    }

    public static void OnClosed(object sender, EventArgs e)
    {
        if (Screen.Current == Screens.Game)
            _networkClient.Disconnect();
        else
            Program.Working = false;
    }

    private static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (Environment.TickCount < _doubleClickTimer + DoubleClickIntervalMs)
            Screen.Current?.MouseDoubleClick(e);
        else
            Screen.Current?.MouseDown(e);
    }

    private static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        _doubleClickTimer = Environment.TickCount;
        Screen.Current?.MouseUp();

        // Reset drag/move state.
        GameScreen.InventoryChange = 0;
        GameScreen.HotbarChange = -1;
    }

    private static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        Mouse.X = e.Position.X;
        Mouse.Y = e.Position.Y;
        InterfaceUtils.MyMouse = Mouse;
        Screen.Current?.MouseMoved();
    }

    private static void OnKeyPressed(object sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Tab:
                TextBox.ChangeFocus();
                return;
        }
    }

    private static void OnKeyReleased(object sender, KeyEventArgs e) =>
        Screen.Current?.KeyReleased(e);

    private static void OnTextEntered(object sender, TextEventArgs e) =>
        TextBox.Focused?.TextEntered(e);

    public static void OpenMenu()
    {
        // Play background music.
        _audioManager.StopAllSounds();
        if (Options.Musics) _audioManager.PlayMusic(Musics.Menu);

        LoginView.SaveUsernameCheckBox.Checked = Options.SaveUsername;
        if (Options.SaveUsername) LoginView.UsernameTextBox.Text = Options.Username;

        MenuScreen.CloseMenus();
        LoginView.LoginPanel.Visible = true;
        Screen.Current = Screens.Menu;
    }
}
