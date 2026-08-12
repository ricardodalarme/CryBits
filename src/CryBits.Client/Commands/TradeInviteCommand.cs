using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Microsoft.Xna.Framework;

namespace CryBits.Client.Commands;

/// <summary>Sends a trade invitation to another player. Usage: /trade &lt;name&gt;</summary>
internal sealed class TradeInviteCommand(IntentSender intentSender, Action<string, Color> writeLine)
    : IChatCommand
{
    public string Verb => "trade";
    public string HelpText => "Invite a player to trade. Usage: /trade <name>";

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            writeLine("Usage: /trade <name>", Color.Red);
            return;
        }

        intentSender.Send(new TradeInviteIntent(default, args[0]));
    }
}
