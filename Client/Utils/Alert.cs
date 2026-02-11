using System;

namespace CryBits.Client.Utils;

/// <summary>
/// Cross-platform alert utility to replace System.Windows.Forms.MessageBox.
/// Outputs messages to the console since native dialogs are platform-specific.
/// </summary>
internal static class Alert
{
    public static void Show(string message)
    {
        // TODO: Implement platform-specific dialogs if needed
        Console.WriteLine($"[CryBits] {message}");
    }
}
