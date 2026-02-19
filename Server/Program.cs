using System;
using System.Threading;
using System.Threading.Tasks;
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
    private static async Task Main()
    {
        // Abre o servidor e define suas configurações
        Console.Title = "Server";
        Logo();
        Console.WriteLine("[Starting]");

        using var cts = new CancellationTokenSource();

        // Eventos de saída do console (cross-platform)
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\r\n[Shutting down...]");
            cts.Cancel();
        };
        AppDomain.CurrentDomain.ProcessExit += (_, _) => cts.Cancel();

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
        Console.WriteLine("Network started. Port: " + Globals.Config.Port);

        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Inicia o laço de comandos numa thread separada
        var consoleThread = new Thread(() => Loop.Commands(cts.Token)) { IsBackground = true };
        consoleThread.Start();

        // Inicia o laço principal e aguarda o cancelamento
        try
        {
            await Loop.MainAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }

        PerformShutdown();
    }

    private static void PerformShutdown()
    {
        // Salva os dados de todos os jogadores
        for (int i = 0; i < Account.List.Count; i++)
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
