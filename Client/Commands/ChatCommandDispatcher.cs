using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace CryBits.Client.Commands;

/// <summary>
/// Routes slash-prefixed chat input to registered <see cref="IChatCommand"/> implementations.
/// </summary>
internal sealed class ChatCommandDispatcher(Action<string, Color> writeLine)
{
    private readonly Dictionary<string, IChatCommand> _commands =
        new(StringComparer.OrdinalIgnoreCase);

    public ChatCommandDispatcher Register(IChatCommand command)
    {
        _commands[command.Verb] = command;
        return this;
    }

    /// <summary>
    /// Tries to handle <paramref name="input"/> as a slash-command.
    /// </summary>
    /// <returns><see langword="true"/> if the input was consumed; <see langword="false"/> if it
    /// should be treated as a regular chat message.</returns>
    public bool TryDispatch(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input[0] != '/') return false;

        var parts = input[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return true;

        var verb = parts[0];

        if (verb.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            PrintHelp();
            return true;
        }

        if (!_commands.TryGetValue(verb, out var command))
        {
            writeLine($"Unknown command '/{verb}'. Type /help for a list of commands.", Color.Red);
            return true;
        }

        command.Execute(parts[1..]);
        return true;
    }

    private void PrintHelp()
    {
        writeLine("Available commands:", Color.Yellow);
        foreach (var cmd in _commands.Values)
            writeLine($"  /{cmd.Verb} — {cmd.HelpText}", Color.Yellow);
    }
}
