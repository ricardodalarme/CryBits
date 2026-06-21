using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;

namespace CryBits.Client.Commands;

/// <summary>Leaves the current party. Usage: /partyleave</summary>
internal sealed class PartyLeaveCommand(IntentSender intentSender) : IChatCommand
{
    public string Verb => "partyleave";
    public string HelpText => "Leave your current party.";

    public void Execute(string[] args) => intentSender.Send(new PartyLeaveIntent(default));
}
