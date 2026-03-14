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
using System;
using System.Drawing;

namespace CryBits.Client.UI;

/// <summary>
/// Handles window-level and UI-level input events.
/// Game-screen key bindings live in <see cref="Logic.GameInput"/>.
/// </summary>
internal class Window(InputManager inputManager, NetworkClient networkClient, AudioManager audioManager)
{
    public static Window Instance { get; } = new(InputManager.Instance, NetworkClient.Instance, AudioManager.Instance);

    /// <summary>Interval in milliseconds within which two clicks count as a double-click.</summary>
    private const int DoubleClickIntervalMs = 142;

    private int _doubleClickTimer;

    /// <summary>Current mouse pointer position in screen coordinates.</summary>
    private Point Mouse;

    public void Bind()
    {
        inputManager.MouseButtonPressed += OnMouseButtonPressed;
        inputManager.MouseButtonReleased += OnMouseButtonReleased;
        inputManager.MouseMoved += OnMouseMoved;
        inputManager.KeyPressed += OnKeyPressed;
        inputManager.KeyReleased += OnKeyReleased;
        inputManager.TextEntered += OnTextEntered;
    }

    public void OnClosed(object sender, EventArgs e)
    {
        if (Screen.Current == Screens.Game)
            networkClient.Disconnect();
        else
            Program.Working = false;
    }

    private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (Environment.TickCount < _doubleClickTimer + DoubleClickIntervalMs)
            Screen.Current?.MouseDoubleClick(e);
        else
            Screen.Current?.MouseDown(e);
    }

    private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        _doubleClickTimer = Environment.TickCount;
        Screen.Current?.MouseUp();

        // Reset drag/move state.
        GameScreen.InventoryChange = 0;
        GameScreen.HotbarChange = -1;
    }

    private void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        Mouse.X = e.Position.X;
        Mouse.Y = e.Position.Y;
        InterfaceUtils.MyMouse = Mouse;
        Screen.Current?.MouseMoved();
    }

    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Tab:
                TextBox.ChangeFocus();
                return;
        }
    }

    private void OnKeyReleased(object sender, KeyEventArgs e) =>
        Screen.Current?.KeyReleased(e);

    private void OnTextEntered(object sender, TextEventArgs e) =>
        TextBox.Focused?.TextEntered(e);

    public void OpenMenu()
    {
        // Play background music.
        audioManager.StopAllSounds();
        if (Options.Instance.Musics) audioManager.PlayMusic(Musics.Menu);

        LoginView.SaveUsernameCheckBox.Checked = Options.Instance.SaveUsername;
        if (Options.Instance.SaveUsername) LoginView.UsernameTextBox.Text = Options.Instance.Username;

        MenuScreen.CloseMenus();
        LoginView.LoginPanel.Visible = true;
        Screen.Current = Screens.Menu;
    }
}
