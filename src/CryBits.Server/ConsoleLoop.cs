using CryBits.Server.Commands;

namespace CryBits.Server;

internal static class ConsoleLoop
{
    public static void Run(CommandDispatcher dispatcher, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Console.Write("Execute: ");
            dispatcher.Dispatch(Console.ReadLine() ?? string.Empty);
        }
    }
}
