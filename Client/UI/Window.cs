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

internal static class Window
{
    private static int _doubleClickTimer;

    /// <summary>
    /// Current mouse pointer position (in screen coordinates).
    /// </summary>
    public static Point Mouse;

    public static void Bind()
    {
        Screens.Game.OnKeyReleased += OnKeyReleased_Game;
    }

    public static void OnClosed(object sender, EventArgs e)
    {
        if (Screen.Current == Screens.Game)
            Socket.Disconnect();
        else
            Program.Working = false;
    }

    public static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (Environment.TickCount < _doubleClickTimer + 142)
            Screen.Current?.MouseDoubleClick(e);
        else
            Screen.Current?.MouseDown(e);
    }

    public static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        // Record double-click timestamp.
        _doubleClickTimer = Environment.TickCount;
        Screen.Current?.MouseUp();

        // Reset drag/move state
        PanelsEvents.InventoryChange = 0;
        PanelsEvents.HotbarChange = -1;
    }

    public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        Mouse.X = e.Position.X;
        Mouse.Y = e.Position.Y;
        InterfaceUtils.MyMouse = Mouse;
        Screen.Current?.MouseMoved();
    }

    public static void OnKeyPressed(object sender, KeyEventArgs e)
    {
        // Handle key press actions.
        switch (e.Code)
        {
            case Keyboard.Key.Tab: TextBox.ChangeFocus(); return;
        }
    }

    public static void OnKeyReleased(object sender, KeyEventArgs e)
    {
        Screen.Current?.KeyReleased(e);
    }

    public static void OnTextEntered(object sender, TextEventArgs e)
    {
        // Dispatch to focused textbox.
        TextBox.Focused?.TextEntered(e);
    }

    public static void OnKeyReleased_Game(KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Enter: Chat.Type(); break;
            case Keyboard.Key.Space: Player.Me.CollectItem(); break;
            case Keyboard.Key.Num1: PlayerSender.HotbarUse(1); break;
            case Keyboard.Key.Num2: PlayerSender.HotbarUse(2); break;
            case Keyboard.Key.Num3: PlayerSender.HotbarUse(3); break;
            case Keyboard.Key.Num4: PlayerSender.HotbarUse(4); break;
            case Keyboard.Key.Num5: PlayerSender.HotbarUse(5); break;
            case Keyboard.Key.Num6: PlayerSender.HotbarUse(6); break;
            case Keyboard.Key.Num7: PlayerSender.HotbarUse(7); break;
            case Keyboard.Key.Num8: PlayerSender.HotbarUse(8); break;
            case Keyboard.Key.Num9: PlayerSender.HotbarUse(9); break;
            case Keyboard.Key.Num0: PlayerSender.HotbarUse(0); break;
        }
    }

    public static void OpenMenu()
    {
        // Play background music.
        Sound.StopAll();
        if (Options.Musics) Music.Play(Musics.Menu);

        // Restore saved username option.
        CheckBoxes.ConnectSaveUsername.Checked = Options.SaveUsername;
        if (Options.SaveUsername) TextBoxes.ConnectUsername.Text = Options.Username;

        // Return player to menu.
        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
        Screen.Current = Screens.Menu;
    }
}