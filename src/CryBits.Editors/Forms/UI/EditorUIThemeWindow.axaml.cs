using Avalonia.Controls;
using Avalonia.Threading;
using CryBits.Editors.Iguina;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUIThemeWindow : Window
{
    private readonly IguinaEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private readonly EditorUIThemeViewModel _viewModel;

    public EditorUIThemeWindow()
    {
        InitializeComponent();

        _viewModel = new EditorUIThemeViewModel();
        DataContext = _viewModel;
        _viewModel.LoadFileList();

        _preview = new IguinaEditorPreview(400, 300);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        _preview.Dispose();
        base.OnClosed(e);
    }

    private void OnRenderTick(object? s, EventArgs e)
    {
        _preview.Draw();
        imgPreview.BlitRenderTarget(_preview.Target);
    }
}
