using CommandLine;
using CryBits.Definitions.Common;

namespace CryBits.Server.Commands;

[Verb("defineaccess", HelpText = "Sets the access level for an online player.")]
internal sealed class DefineAccessCommand : IConsoleCommand
{
    [Value(0, Required = true, MetaName = "playerName", HelpText = "The online player's username.")]
    public string PlayerName { get; set; } = string.Empty;

    [Value(1, Required = true, MetaName = "accessLevel",
        HelpText = "Numeric access level (0 = Player, 1 = Editor, 2 = Admin).")]
    public byte Access { get; set; }

    public void Execute()
    {
        var session = ServerContext.Host?.Sessions.Find(x => x.Account?.Username.Equals(PlayerName, StringComparison.OrdinalIgnoreCase) == true);
        if (session?.Account == null)
        {
            Console.WriteLine("This player is either offline or doesn't exist.");
            return;
        }

        session.Account.AccessLevel = (Access)Access;
        Console.WriteLine($"{(Access)Access} access granted to {PlayerName}.");
    }
}
