using CryBits.Server.Commands;
using System;
using System.Threading;

namespace CryBits.Server;

internal static class ConsoleLoop
{
    public static void Run(CommandDispatcher dispatcher, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Console.Write("Execute: ");
            dispatcher.Dispatch(Console.ReadLine());
        }
    }
}
