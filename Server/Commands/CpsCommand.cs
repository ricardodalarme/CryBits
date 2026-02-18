using System;
using CommandLine;
using CryBits.Server.Logic;

namespace CryBits.Server.Commands;

[Verb("cps", HelpText = "Shows the current server cycles per second.")]
internal sealed class CpsCommand : IConsoleCommand
{
    public void Execute() => Console.WriteLine("CPS: " + Loop.Cps);
}
