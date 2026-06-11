using CryBits.Client.Network.Senders;

namespace CryBits.Client.Commands;

/// <summary>Leaves the current party. Usage: /partyleave</summary>
internal sealed class PartyLeaveCommand(PartySender partySender) : IChatCommand
{
    public string Verb => "partyleave";
    public string HelpText => "Leave your current party.";

    public void Execute(string[] args) => partySender.PartyLeave();
}
