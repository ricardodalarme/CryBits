namespace CryBits.Client;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using var shell = new ClientShell(args.Contains("--offline"));
        shell.Run();
    }
}
