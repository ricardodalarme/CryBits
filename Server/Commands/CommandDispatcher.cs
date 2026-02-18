using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace CryBits.Server.Commands;

internal sealed class CommandDispatcher
{
    private readonly List<Type> _commandTypes = new();
    private readonly Parser _parser;

    public CommandDispatcher()
    {
        _parser = new Parser(settings =>
        {
            settings.AutoVersion = false;
            settings.AutoHelp = false;
            settings.CaseSensitive = false;
            settings.CaseInsensitiveEnumValues = true;
            settings.HelpWriter = null; // suppress all auto-generated output
        });
    }

    public CommandDispatcher Register<T>() where T : IConsoleCommand, new()
    {
        _commandTypes.Add(typeof(T));
        return this;
    }

    public void Dispatch(string input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input)) return;

        var args = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0) return;

        if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            PrintHelp();
            return;
        }

        _parser
            .ParseArguments(args, _commandTypes.ToArray())
            .WithParsed<IConsoleCommand>(cmd => cmd.Execute())
            .WithNotParsed(errors => HandleErrors(args[0], errors));
    }

    private void HandleErrors(string verb, IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();

        // Unknown verb
        if (errorList.Any(e => e is BadVerbSelectedError))
        {
            Console.WriteLine($"Unknown command '{verb}'. Type 'help' to see available commands.");
            return;
        }

        // Missing required argument or bad value — show usage for the matched verb
        var type = _commandTypes.FirstOrDefault(t =>
        {
            var attr = (VerbAttribute)Attribute.GetCustomAttribute(t, typeof(VerbAttribute));
            return attr != null && attr.Name.Equals(verb, StringComparison.OrdinalIgnoreCase);
        });

        if (type != null)
            Console.WriteLine("Usage: " + BuildUsage(type));
    }

    private void PrintHelp()
    {
        Console.WriteLine("\r\n     Available commands:");
        Console.WriteLine($"     {"help",-20}  Lists all available commands.");
        foreach (var type in _commandTypes)
        {
            var verb = (VerbAttribute)Attribute.GetCustomAttribute(type, typeof(VerbAttribute));
            if (verb != null)
                Console.WriteLine($"     {verb.Name,-20}  {verb.HelpText}");
        }

        Console.WriteLine();
    }

    private static string BuildUsage(Type type)
    {
        var verb = (VerbAttribute)Attribute.GetCustomAttribute(type, typeof(VerbAttribute));
        var sb = new StringBuilder(verb!.Name);

        var props = type.GetProperties()
            .Select(p => new
            {
                Property = p,
                Value = p.GetCustomAttributes(typeof(ValueAttribute), false).FirstOrDefault() as ValueAttribute,
                Option = p.GetCustomAttributes(typeof(OptionAttribute), false).FirstOrDefault() as OptionAttribute
            })
            .OrderBy(x => x.Value?.Index ?? int.MaxValue);

        foreach (var p in props)
        {
            if (p.Value != null)
                sb.Append(p.Value.Required ? $" <{p.Value.MetaName}>" : $" [{p.Value.MetaName}]");
            else if (p.Option != null)
                sb.Append($" [--{p.Option.LongName} <{p.Option.MetaValue ?? p.Property.Name.ToLower()}>]");
        }

        return sb.ToString();
    }
}
