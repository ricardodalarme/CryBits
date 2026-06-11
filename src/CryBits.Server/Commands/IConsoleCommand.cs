namespace CryBits.Server.Commands;

internal interface IConsoleCommand
{
    /// <summary>Execute the command. Properties are populated by CommandLineParser before this is called.</summary>
    void Execute();
}
