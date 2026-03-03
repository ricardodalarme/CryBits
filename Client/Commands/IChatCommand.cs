namespace CryBits.Client.Commands;

internal interface IChatCommand
{
    /// <summary>The slash-less verb that triggers this command (e.g. "party").</summary>
    string Verb { get; }

    /// <summary>Short description shown in /help.</summary>
    string HelpText { get; }

    /// <summary>Execute the command with the tokenised arguments that follow the verb.</summary>
    void Execute(string[] args);
}
