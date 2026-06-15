using CryBits.Client;
using System;

namespace CryBits.Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using var shell = new Game(args.Contains("--offline"));
        shell.Run();
    }
}
