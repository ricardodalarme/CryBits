using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Microsoft.Xna.Framework;

namespace CryBits.Client.Commands;

/// <summary>Sends a party invitation to another player. Usage: /party &lt;name&gt;</summary>
internal sealed class PartyInviteCommand(IntentSender intentSender, Action<string, Color> writeLine)
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

        intentSender.Send(new PartyInviteIntent(default, args[0]));
    }
}
