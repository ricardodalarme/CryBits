using CryBits.Client.Network.Senders;
using SFML.Graphics;
using System;

namespace CryBits.Client.Commands;

/// <summary>Sends a party invitation to another player. Usage: /party &lt;name&gt;</summary>
internal sealed class PartyInviteCommand(PartySender partySender, Action<string, Color> writeLine)
    : IChatCommand
{
    public string Verb => "party";
    public string HelpText => "Invite a player to your party. Usage: /party <name>";

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            writeLine("Usage: /party <name>", Color.Red);
            return;
        }

        partySender.PartyInvite(args[0]);
    }
}
