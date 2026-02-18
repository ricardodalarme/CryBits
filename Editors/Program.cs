using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using CryBits.Editors.AvaloniaUI;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using CryBits.Client.Framework.Library.Repositories;
using EditorToolsRepository = CryBits.Editors.Library.Repositories.ToolsRepository;

namespace CryBits.Editors;

internal static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Medida de calculo do atraso do jogo
    public static short Fps;

    private static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();

        // Carrega as preferências
        OptionsRepository.Read();
        EditorToolsRepository.Read();

        // Inicializa todos os dispositivos
        Socket.Init();
        Sound.Load();

        // Start the game loop on a background thread.
        // It will block on AvaloniaRuntime.WaitUntilReady() until Avalonia is up.
        var loopThread = new Thread(() =>
        {
            // Wait until Avalonia is fully initialised on the main thread
            AvaloniaRuntime.WaitUntilReady();

            // Abre a janela de login e inicia o laço
            AvaloniaLoginLauncher.ShowLogin();
            Loop.Init();
        });
        loopThread.IsBackground = true;
        loopThread.Start();

        // Run Avalonia on the main thread (required by macOS/Cocoa and Linux/X11)
        AvaloniaRuntime.BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(Array.Empty<string>(), desktop =>
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            });
    }

    public static void Close()
    {
        var waitTimer = Environment.TickCount;

        // Desconecta da rede
        Socket.Disconnect();

        // Espera até que o jogador seja desconectado
        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Thread.Sleep(10);

        // Fecha a aplicação
        Working = false;
    }
}
