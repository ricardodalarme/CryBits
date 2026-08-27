using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CryBits.Client.Framework.Constants;
using CryBits.Editors.Forms.UI.ViewModels;
using CryBits.Editors.Preview;
using Myra.Graphics2D.UI;
using System.Collections.ObjectModel;
using AvWindow = Avalonia.Controls.Window;
using AvMenuItem = Avalonia.Controls.MenuItem;
using MyraWidget = Myra.Graphics2D.UI.Widget;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUILayoutWindow : AvWindow
{
    public static void Open(AvWindow owner)
    {
        owner.Hide();
        var window = new EditorUILayoutWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly MyraEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private readonly EditorUILayoutViewModel _viewModel;

    public EditorUILayoutWindow()
    {
        InitializeComponent();

        _viewModel = new EditorUILayoutViewModel();
        DataContext = _viewModel;
        _viewModel.RequestRefresh += RebuildTree;

        _preview = new MyraEditorPreview(Program.SharedDevice!, 800, 608);

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
        Graphics.EditorGraphics.Tick();
        _preview.Render();
        imgPreview.BlitRenderTarget(_preview.Target);
    }

    private void SelectScreen()
    {
        var project = _viewModel.CurrentProject;
        if (project == null || _viewModel.SelectedScreen == null) return;

        _preview.Desktop.Root = project.Root;
        _viewModel.SelectedNode = null;
        propertyGrid.DataContext = null;

        if (project.Root == null)
        {
            treEntities.ItemsSource = null;
            return;
        }

        var rootNode = MakeWidgetNode(project.Root);
        treEntities.ItemsSource = new ObservableCollection<WidgetNode> { rootNode };
    }

    private void RebuildTree()
    {
        if (_viewModel.SelectedScreen != null) SelectScreen();
    }

    private static WidgetNode MakeWidgetNode(MyraWidget widget)
    {
        var typeName = widget.GetType().Name;
        var id = !string.IsNullOrEmpty(widget.Id) ? widget.Id : typeName;
        var node = new WidgetNode { Header = $"[{typeName}] {id}", Widget = widget };

        if (widget is Container container)
        {
            foreach (var child in container.Widgets)
            {
                node.Children.Add(MakeWidgetNode(child));
            }
        }

        return node;
    }

    private void treEntities_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        _viewModel.SelectedNode = e.AddedItems.Count > 0 ? e.AddedItems[0] as WidgetNode : null;

        if (_viewModel.SelectedNode?.Widget is { } widget)
        {
            propertyGrid.DataContext = WidgetPropertiesFactory.Create(widget);
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
            ("Panel", "Panel"), ("Button", "Button"), ("CheckButton", "Check Button"), ("RadioButton", "Radio Button"),
            ("TextBox", "Text Box"), ("SpinButton", "Spin Button"), ("Label", "Label"),
            ("ProgressBar", "Progress Bar"), ("Slider", "Slider"),
            ("ListView", "List View"), ("ComboView", "Combo View"), ("Grid", "Grid")
        };

        var flyout = new MenuFlyout();
        foreach (var (key, label) in types)
        {
            var item = new AvMenuItem { Header = label, Tag = key };
            item.Click += (_, _) => AddElement(key);
            flyout.Items.Add(item);
        }

        FlyoutBase.SetAttachedFlyout(butAdd, flyout);
    }

    private void AddElement(string widgetType)
    {
        var project = _viewModel.CurrentProject;
        if (project == null || project.Root is not Container container) return;

        MyraWidget newWidget = widgetType switch
        {
            "Button" => new Myra.Graphics2D.UI.Button { Width = 100, Height = 30 },
            "CheckButton" => new Myra.Graphics2D.UI.CheckButton(),
            "RadioButton" => new Myra.Graphics2D.UI.RadioButton(),
            "TextBox" => new Myra.Graphics2D.UI.TextBox { Width = 140 },
            "SpinButton" => new Myra.Graphics2D.UI.SpinButton { Width = 80 },
            "Label" => new Myra.Graphics2D.UI.Label { Text = "Label" },
            "ProgressBar" => new Myra.Graphics2D.UI.HorizontalProgressBar { Width = 150, Height = 16 },
            "Slider" => new Myra.Graphics2D.UI.HorizontalSlider { Width = 150 },
            "ListView" => new Myra.Graphics2D.UI.ListView { Width = 120, Height = 100 },
            "ComboView" => new Myra.Graphics2D.UI.ComboView { Width = 120 },
            "Grid" => new Myra.Graphics2D.UI.Grid { Width = 200, Height = 200 },
            _ => new Myra.Graphics2D.UI.Panel { Width = 100, Height = 100 }
        };

        newWidget.Id = $"New{widgetType}";
        container.Widgets.Add(newWidget);
        RebuildTree();
    }

    private void butAdd_Click(object? s, RoutedEventArgs e)
    {
        FlyoutBase.ShowAttachedFlyout(butAdd);
    }
}
