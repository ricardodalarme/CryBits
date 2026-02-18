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
    // Detecção de duplo clique
    private static int _doubleClickTimer;

    // Posição do ponteiro do mouse
    public static Point Mouse;

    public static void Bind()
    {
        Screens.Game.OnKeyReleased += OnKeyReleased_Game;
    }

    public static void OnClosed(object sender, EventArgs e)
    {
        // Fecha o jogo
        if (Screen.Current == Screens.Game)
            Socket.Disconnect();
        else
            Program.Working = false;
    }

    public static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        // Clique
        if (Environment.TickCount < _doubleClickTimer + 142)
            Screen.Current?.MouseDoubleClick(e);
        else
            Screen.Current?.MouseDown(e);
    }

    public static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        // Contagem do clique duplo
        _doubleClickTimer = Environment.TickCount;
        Screen.Current?.MouseUp();

        // Reseta a movimentação
        PanelsEvents.InventoryChange = 0;
        PanelsEvents.HotbarChange = -1;
    }

    public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        // Define a posição do mouse à váriavel
        Mouse.X = e.Position.X;
        Mouse.Y = e.Position.Y;
        InterfaceUtils.MyMouse = Mouse;
        Screen.Current?.MouseMoved();
    }

    public static void OnKeyPressed(object sender, KeyEventArgs e)
    {
        // Define se um botão está sendo pressionado
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
        // Executa os eventos
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
        // Reproduz a música de fundo
        Sound.StopAll();
        if (Options.Musics) Music.Play(Musics.Menu);

        // Nome do usuário salvo
        CheckBoxes.ConnectSaveUsername.Checked = Options.SaveUsername;
        if (Options.SaveUsername) TextBoxes.ConnectUsername.Text = Options.Username;

        // Traz o jogador de volta ao menu
        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
        Screen.Current = Screens.Menu;
    }
}