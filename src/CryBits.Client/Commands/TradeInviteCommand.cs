using CryBits.Client.Network.Senders;
using SFML.Graphics;
using System;

namespace CryBits.Client.Commands;

/// <summary>Sends a trade invitation to another player. Usage: /trade &lt;name&gt;</summary>
internal sealed class TradeInviteCommand(TradeSender tradeSender, Action<string, Color> writeLine)
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

        tradeSender.TradeInvite(args[0]);
    }
}
