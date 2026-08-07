using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Dtos;
using CryBits.Client.Framework.UI;
using CryBits.Editors.Forms.UI.ViewModels;
using CryBits.Editors.Iguina;
using CryBits.Editors.Utils;
using Iguina.Entities;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUILayoutWindow : Window
{
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorUILayoutWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly IguinaEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private readonly EditorUILayoutViewModel _viewModel;

    public EditorUILayoutWindow()
    {
        InitializeComponent();

        _viewModel = new EditorUILayoutViewModel();
        DataContext = _viewModel;
        _viewModel.RequestRefresh += RebuildTree;
        _viewModel.RequestOpenTheme += () => new EditorUIThemeWindow().Show();

        _preview = new IguinaEditorPreview(800, 608);

        var themeDir = ResolveThemeDir();
        if (themeDir != null)
            _viewModel.Load(themeDir);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();

        BuildAddFlyout();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        _preview.Dispose();
        base.OnClosed(e);
    }

    private static string? ResolveThemeDir() =>
        Directory.Exists(Directories.UiTheme.FullName) ? Directories.UiTheme.FullName : null;

    private void OnRenderTick(object? s, EventArgs e)
    {
        _preview.Draw();
        imgPreview.Blit(_preview.Target);
    }

    private void SelectScreen()
    {
        var config = _viewModel.CurrentLayout;
        if (config == null || _viewModel.SelectedScreen == null) return;

        _preview.Clear();
        _viewModel.SelectedNode = null;
        propertyGrid.DataContext = null;

        var screen = config.Screens.FirstOrDefault(s => s.Name == _viewModel.SelectedScreen);
        if (screen == null) return;

        var (panel, reg) = LayoutBuilder.BuildScreen(_preview.UISystem, screen);
        _preview.LoadEntity(panel);

        var rootNode = new EntityNode { Header = $"[Screen] {screen.Name}", Entity = panel };
        foreach (var el in screen.Children)
            rootNode.Children.Add(MakeElementNode(el, reg));
        treEntities.ItemsSource = new ObservableCollection<EntityNode> { rootNode };
    }

    private void RebuildTree()
    {
        if (_viewModel.SelectedScreen != null) SelectScreen();
    }

    private static EntityNode MakeElementNode(Element el, Dictionary<string, Entity> reg)
    {
        var typeDiscriminator = ElementViewModelFactory.GetDiscriminator(el);
        var node = new EntityNode { Header = $"[{typeDiscriminator}] {el.Name}", ConfigElement = el };
        if (reg.TryGetValue(el.Name, out var entity))
            node.Entity = entity;
        foreach (var child in el.Children)
            node.Children.Add(MakeElementNode(child, reg));
        return node;
    }

    private void treEntities_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        _viewModel.SelectedNode = e.AddedItems.Count > 0 ? e.AddedItems[0] as EntityNode : null;

        if (_viewModel.SelectedNode is { Entity: not null, ConfigElement: not null })
        {
            var vm = ElementViewModelFactory.Create(_viewModel.SelectedNode.ConfigElement,
                _viewModel.SelectedNode.Entity);
            propertyGrid.DataContext = vm;
        }
        else
        {
            propertyGrid.DataContext = null;
        }
    }

    private void BuildAddFlyout()
    {
        var types = new[]
        {
            ("Panel", "Panel"), ("Button", "Button"), ("Checkbox", "Checkbox"), ("RadioButton", "Radio Button"),
            ("TextInput", "Text Input"), ("NumericInput", "Numeric Input"), ("Label", "Label"), ("Title", "Title"),
            ("Paragraph", "Paragraph"), ("ProgressBar", "Progress Bar"), ("Slider", "Slider"),
            ("Picture", "Picture"), ("SlotGrid", "Slot Grid"), ("ListBox", "List Box"), ("DropDown", "Drop Down")
        };

        var flyout = new MenuFlyout();
        foreach (var (key, label) in types)
        {
            var item = new MenuItem { Header = label, Tag = key };
            item.Click += (_, _) => AddElement(key);
            flyout.Items.Add(item);
        }

        FlyoutBase.SetAttachedFlyout(butAdd, flyout);
    }

    private void AddElement(string discriminator)
    {
        var config = _viewModel.CurrentLayout;
        if (config == null || _viewModel.SelectedScreen == null) return;
        var screen = config.Screens.FirstOrDefault(s => s.Name == _viewModel.SelectedScreen);
        if (screen == null) return;

        var el = ElementViewModelFactory.CreateDefault(discriminator);
        el.Name = $"New{discriminator}";
        el.X = 0;
        el.Y = 0;
        el.Width = 100;
        el.Height = 24;
        _viewModel.AddElement(el);
    }

    private void butAdd_Click(object? s, RoutedEventArgs e)
    {
        FlyoutBase.ShowAttachedFlyout(butAdd);
    }
}
