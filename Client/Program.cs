using System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Client.UI.Events;

namespace CryBits.Client;

internal static class Program
{
    /// <summary>
    /// Indicates whether the application main loop is running.
    /// </summary>
    public static bool Working = true;

    [STAThread]
    private static void Main()
    {
        Directories.Create();

        ToolsRepository.Read();
        OptionsRepository.Read();

        CheckBoxEvents.Bind();
        ButtonsEvents.Bind();
        PanelsEvents.Bind();
        TextBoxesEvents.Bind();
        Window.Bind();

        Window.OpenMenu();

        Socket.Init();
        PacketDispatcher.Register();
        Sound.Load();
        Renders.Init();

        Loop.Init();
    }

    /// <summary>
    /// Disconnects from the server and exits the application.
    /// </summary>
    public static void Close()
    {
        var waitTimer = Environment.TickCount;

        Socket.Disconnect();

        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Socket.HandleData();

        Working = false;
        Environment.Exit(0);
    }
}
