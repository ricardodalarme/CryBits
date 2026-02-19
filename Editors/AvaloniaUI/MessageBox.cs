using System.Threading.Tasks;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CryBits.Editors.AvaloniaUI;

/// <summary>
/// Thin wrapper around MsBox.Avalonia so all existing call sites
/// can stay synchronous (fire-and-forget) while the actual dialog
/// is async under the hood.
/// </summary>
internal static class MessageBox
{
    /// <summary>
    /// Show an OK alert without blocking the caller.
    /// Safe to call from any synchronous event handler.
    /// </summary>
    public static void Show(string message) =>
        Dispatcher.UIThread.Post(() => _ = ShowAsync(message));

    /// <summary>
    /// Show an OK alert and await its dismissal.
    /// Use this when you need to wait for the user to acknowledge.
    /// </summary>
    public static async Task ShowAsync(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "CryBits Editor", message, ButtonEnum.Ok);
        await box.ShowAsync();
    }
}
