using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using SFML.Window;

namespace CryBits.Client.UI;

/// <summary>
/// Handles window-level and UI-level input events.
/// Game-screen key bindings live in <see cref="Logic.GameInput"/>.
/// </summary>
internal static class Window
{
    /// <summary>Interval in milliseconds within which two clicks count as a double-click.</summary>
    private const int DoubleClickIntervalMs = 142;

    private static int _doubleClickTimer;

    /// <summary>Current mouse pointer position in screen coordinates.</summary>
    public static Point Mouse;

    public static void Bind()
    {
        Input.InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
        Input.InputManager.Instance.MouseButtonReleased += OnMouseButtonReleased;
        Input.InputManager.Instance.MouseMoved += OnMouseMoved;
        Input.InputManager.Instance.KeyPressed += OnKeyPressed;
        Input.InputManager.Instance.KeyReleased += OnKeyReleased;
        Input.InputManager.Instance.TextEntered += OnTextEntered;
    }

    public static void OnClosed(object sender, EventArgs e)
    {
        if (Screen.Current == Screens.Game)
            NetworkClient.Disconnect();
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
        PanelsEvents.InventoryChange = 0;
        PanelsEvents.HotbarChange = -1;
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
        AudioManager.Instance.StopAllSounds();
        if (Options.Musics) AudioManager.Instance.PlayMusic(Musics.Menu);

        CheckBoxes.ConnectSaveUsername.Checked = Options.SaveUsername;
        if (Options.SaveUsername) TextBoxes.ConnectUsername.Text = Options.Username;

        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
        Screen.Current = Screens.Menu;
    }
}
