using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Commands;
using CryBits.Server.Network;
using CryBits.Server.Systems;

namespace CryBits.Server.Logic;

internal static class Loop
{
    // Target simulation rate: 20 ticks per second (50ms per tick)
    private const int TicksPerSecond = 20;

    // Medida de quantos loops são executados por segundo
    public static int Cps;

    // Contagens
    private static long _timer500, _timer1000;
    public static long TimerRegeneration;
    public static long TimerMapItems;

    public static async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / TicksPerSecond));
        var cps = 0;

        while (await timer.WaitForNextTickAsync(ct))
        {
            // Manuseia os dados recebidos
            Socket.HandleData();

            var now = Environment.TickCount64;

            if (now > _timer500 + 500)
            {
                // Map logic
                foreach (var tempMap in TempMap.List.Values) tempMap.Logic();

                // Player vital regeneration
                foreach (var account in Account.List.Where(a => a.IsPlaying))
                    RegenerationSystem.Tick(account.Character);

                // Reinicia a contagem dos 500
                _timer500 = now;
            }

            // Reinicia algumas contagens
            if (now > TimerRegeneration + 5000) TimerRegeneration = now;
            if (now > TimerMapItems + 300000) TimerMapItems = now;

            // Calcula o CPS
            if (_timer1000 < now)
            {
                Cps = cps;
                cps = 0;
                _timer1000 = now + 1000;
            }
            else
                cps++;
        }
    }

    public static void Commands(CancellationToken ct)
    {
        var dispatcher = new CommandDispatcher()
            .Register<CpsCommand>()
            .Register<DefineAccessCommand>();

        // Laço para que seja possível a utilização de comandos pelo console
        while (!ct.IsCancellationRequested)
        {
            Console.Write("Execute: ");
            dispatcher.Dispatch(Console.ReadLine());
        }
    }
}