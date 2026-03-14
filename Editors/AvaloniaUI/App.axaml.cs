using Avalonia;
using Avalonia.Markup.Xaml;
using System.Threading;

namespace CryBits.Editors.AvaloniaUI;

internal sealed class App : Application
{
    private static readonly ManualResetEventSlim AppReady = new(false);

    /// <summary>Blocks the calling thread until Avalonia is fully initialised.</summary>
    public static void WaitUntilReady() => AppReady.Wait();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        AppReady.Set();
    }
}
