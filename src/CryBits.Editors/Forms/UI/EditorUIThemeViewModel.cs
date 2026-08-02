using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Client.Framework.Constants;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUIThemeViewModel : ObservableObject
{
    [ObservableProperty] private string _fileName = "Select a file";

    [ObservableProperty] private string _fileContent = string.Empty;

    [ObservableProperty] private string? _currentFilePath;

    [ObservableProperty] private string? _selectedFileName;

    private string? _themeDir;
    private string? _savedContent;

    public ObservableCollection<string> FileList { get; } = [];

    partial void OnSelectedFileNameChanged(string? value) => SelectFile(value);

    public void LoadFileList()
    {
        FileList.Clear();
        _themeDir = Directories.UiTheme.FullName;
        if (!Directory.Exists(_themeDir)) return;

        var stylesDir = Path.Combine(_themeDir, "Styles");
        if (Directory.Exists(stylesDir))
            foreach (var f in Directory.GetFiles(stylesDir, "*.json")
                         .OrderBy(f => Path.GetFileName(f)))
                FileList.Add(Path.GetFileName(f));

        foreach (var name in new[] { "SystemStyle.json", "Layout.json" })
            if (File.Exists(Path.Combine(_themeDir, name)))
                FileList.Add(name);
    }

    public void SelectFile(string? fileName)
    {
        if (fileName == null || _themeDir == null) return;

        string filePath;
        if (fileName == "SystemStyle.json" || fileName == "Layout.json")
            filePath = Path.Combine(_themeDir, fileName);
        else
            filePath = Path.Combine(_themeDir, "Styles", fileName);

        if (!File.Exists(filePath)) return;

        CurrentFilePath = filePath;
        _savedContent = File.ReadAllText(filePath);
        FileContent = _savedContent;
        FileName = fileName;
    }

    [RelayCommand]
    private void RefreshPreview()
    {
        // Preview drawing is handled externally via the timer
    }

    [RelayCommand]
    private void RevertFile()
    {
        if (_savedContent != null)
            FileContent = _savedContent;
    }

    [RelayCommand]
    private void Save()
    {
        if (CurrentFilePath == null) return;
        File.WriteAllText(CurrentFilePath, FileContent ?? "");
        _savedContent = FileContent;
    }
}
