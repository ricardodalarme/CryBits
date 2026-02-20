using System;
using CommandLine;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Persistence.Repositories;

namespace CryBits.Server.Commands;

[Verb("defineaccess", HelpText = "Sets the access level for an online player.")]
internal sealed class DefineAccessCommand : IConsoleCommand
{
    [Value(0, Required = true, MetaName = "playerName", HelpText = "The online player's username.")]
    public string PlayerName { get; set; }

    [Value(1, Required = true, MetaName = "accessLevel",
        HelpText = "Numeric access level (0 = Player, 1 = Editor, 2 = Admin).")]
    public byte Access { get; set; }

    public void Execute()
    {
        var account = Account.List.Find(x => x.User.Equals(PlayerName, StringComparison.OrdinalIgnoreCase));
        if (account == null)
        {
            Console.WriteLine("This player is either offline or doesn't exist.");
            return;
        }

        account.Access = (Access)Access;
        AccountRepository.Write(account);
        Console.WriteLine($"{(Access)Access} access granted to {PlayerName}.");
    }
}
