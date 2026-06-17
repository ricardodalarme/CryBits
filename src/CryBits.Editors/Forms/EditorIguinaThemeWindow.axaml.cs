using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Iguina;
using Iguina.Defs;
using IguPanel = Iguina.Entities.Panel;
using IguButton = Iguina.Entities.Button;

namespace CryBits.Editors.Forms;

internal partial class EditorIguinaThemeWindow : Window
{
    private readonly IguinaEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private WriteableBitmap? _bitmap;
    private string? _themeDir;
    private string? _currentFilePath;
    private string? _fileContent;

    public EditorIguinaThemeWindow()
    {
        InitializeComponent();

        _preview = new IguinaEditorPreview(400, 300);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();

        LoadFileList();
    }

    private void LoadFileList()
    {
        _themeDir = ResolveThemeDir();
        if (_themeDir == null) return;

        lstFiles.Items.Clear();
        // Add all JSON files from Styles folder and the system file
        var stylesDir = System.IO.Path.Combine(_themeDir, "Styles");
        if (Directory.Exists(stylesDir))
        {
            foreach (var f in Directory.GetFiles(stylesDir, "*.json").OrderBy(f => System.IO.Path.GetFileName(f)))
                lstFiles.Items.Add(System.IO.Path.GetFileName(f));
        }
        // Add system_style.json
        if (File.Exists(System.IO.Path.Combine(_themeDir, "system_style.json")))
            lstFiles.Items.Add("system_style.json");
        // Add menu_config.json
        if (File.Exists(System.IO.Path.Combine(_themeDir, "menu_config.json")))
            lstFiles.Items.Add("menu_config.json");
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        _preview.Dispose();
        base.OnClosed(e);
    }

    private static string? ResolveThemeDir()
    {
        var dir = AppContext.BaseDirectory;
        var p = System.IO.Path.Combine(dir, "IguinaTheme");
        if (Directory.Exists(p)) return p;
        p = System.IO.Path.Combine(dir, "..", "..", "..", "..", "src", "CryBits.Client", "IguinaTheme");
        return Directory.Exists(p) ? p : null;
    }

    private void OnRenderTick(object? s, EventArgs e)
    {
        _preview.Draw();
        SfmlRenderBlit.Blit(_preview.Target, ref _bitmap, imgPreview);
    }

    private void UpdatePreview()
    {
        _preview.Clear();
        var pnl = new IguPanel(_preview.UISystem);
        pnl.Size.SetPixels(200, 150);
        pnl.Anchor = global::Iguina.Defs.Anchor.Center;
        pnl.Offset.SetPixels(0, 0);
        pnl.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/1.png",
            SourceRect = new Rectangle { Width = 200, Height = 150 }
        };
        var btn = new IguButton(_preview.UISystem);
        btn.Size.SetPixels(100, 40);
        btn.Anchor = global::Iguina.Defs.Anchor.Center;
        btn.Paragraph.Text = "Sample";
        pnl.AddChild(btn);
        _preview.LoadEntity(pnl);
    }

    private void lstFiles_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        if (lstFiles.SelectedItem == null || _themeDir == null) return;

        var fileName = lstFiles.SelectedItem.ToString() ?? "";
        string filePath;
        if (fileName == "system_style.json")
            filePath = System.IO.Path.Combine(_themeDir, "system_style.json");
        else if (fileName == "menu_config.json")
            filePath = System.IO.Path.Combine(_themeDir, "menu_config.json");
        else
            filePath = System.IO.Path.Combine(_themeDir, "Styles", fileName);

        if (!File.Exists(filePath)) return;

        _currentFilePath = filePath;
        _fileContent = File.ReadAllText(filePath);
        txtEditor.Text = _fileContent;
        lblFileName.Text = fileName;
    }

    private void txtEditor_TextChanged(object? s, TextChangedEventArgs e) { }

    private void butRefreshPreview_Click(object? s, RoutedEventArgs e)
    {
        UpdatePreview();
    }

    private void butRevertFile_Click(object? s, RoutedEventArgs e)
    {
        if (_fileContent != null)
            txtEditor.Text = _fileContent;
    }

    private void butSave_Click(object? s, RoutedEventArgs e)
    {
        if (_currentFilePath == null) return;
        File.WriteAllText(_currentFilePath, txtEditor.Text ?? "");
    }
}
