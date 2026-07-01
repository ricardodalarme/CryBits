using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Iguina;

namespace CryBits.Editors.Forms.UI;

internal partial class EditorUIThemeWindow : Window
{
    private readonly IguinaEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private readonly EditorUIThemeViewModel _viewModel;
    private WriteableBitmap? _bitmap;

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
        SfmlRenderBlit.Blit(_preview.Target, ref _bitmap, imgPreview);
    }
}
