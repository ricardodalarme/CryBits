using System;
using System.Threading;
using CryBits.Entities.Map;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Library;
using CryBits.Server.Library.Repositories;
using CryBits.Server.Logic;
using CryBits.Server.Network;

namespace CryBits.Server;

internal static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    private static void Main()
    {
        // Abre o servidor e define suas configurações
        Console.Title = "Server";
        Logo();
        Console.WriteLine("[Starting]");

        // Eventos de saída do console (cross-platform)
        Console.CancelKeyPress += OnCancelKeyPress;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();
        Console.WriteLine("Directories created.");

        // Carrega todos os dados necessários
        DataLoader.LoadAll();

        // Cria os mapas temporários
        Console.WriteLine("Creating temporary maps.");
        foreach (var map in Map.List.Values) TempMap.CreateTemporary(map, true);

        // Cria os dispositivos da rede
        Socket.Init();
        Console.WriteLine("Network started. Port: " + Globals.Port);

        // Calcula quanto tempo demorou para inicializar o servidor
        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Inicia os laços
        var consoleLoop = new Thread(Loop.Commands);
        consoleLoop.Start();
        Loop.Main();
    }

    private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        // Previne o encerramento imediato para permitir cleanup
        e.Cancel = true;
        Working = false;

        Console.WriteLine("\r\n[Shutting down...]");
        PerformShutdown();

        // Força a saída do processo após cleanup
        Environment.Exit(0);
    }

    private static void OnProcessExit(object sender, EventArgs e)
    {
        // Chamado quando o processo está sendo encerrado
        PerformShutdown();
    }

    private static void PerformShutdown()
    {
        // Salva os dados de todos os jogadores
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                CharacterRepository.Write(Account.List[i]);

        // Fecha o servidor
        Socket.Device.Stop();
    }

    private static void Logo()
    {
        Console.WriteLine(@"  ______              _____     _
 |   ___|            |     \   | |
 |  |     _ ____   _ |   __/ _ | |_  ___
 |  |    | '__/\\ // |   \_ | || __|/ __|
 |  |___ | |    | |  |     \| || |_ \__ \
 |______||_|    |_|  |_____/|_| \__||___/
                          2D orpg engine" + "\r\n");
    }
}
