using Avalonia;
using Avalonia.Markup.Xaml;

namespace CryBits.Editors.AvaloniaUI;

internal sealed partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
