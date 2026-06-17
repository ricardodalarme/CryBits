using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics;
using CryBits.Client.Iguina;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;
using Iguina;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu;

internal sealed class MenuScreen
{
    private enum ScreenType { None, Login, Register, Options, SelectCharacter, CreateCharacter }

    private readonly UISystem _ui;
    private readonly MenuConfig _config;

    private Panel? _rootPanel;
    private ScreenType _currentScreen;
    private bool _isVisible;

    private readonly LoginView _loginView;
    private readonly RegisterView _registerView;
    private readonly OptionsView _optionsView;
    private readonly SelectCharacterView _selectCharacterView;
    private readonly CreateCharacterView _createCharacterView;

    public bool IsVisible => _isVisible;

    public MenuScreen(UISystem ui, Renderer renderer, CharacterRenderer characterRenderer,
        DefinitionCatalog catalog)
    {
        _ui = ui;
        _config = MenuLoader.Load(
            System.IO.Path.Combine(Directories.IguinaTheme.FullName, "menu_config.json"));

        _loginView = new LoginView(ui);
        _registerView = new RegisterView(ui);
        _optionsView = new OptionsView(ui);
        _selectCharacterView = new SelectCharacterView(ui, characterRenderer);
        _createCharacterView = new CreateCharacterView(ui, characterRenderer, catalog);

        WireNavigation();
        WireEvents();
        _currentScreen = ScreenType.None;
    }

    public void ShowLogin() => SwitchTo(ScreenType.Login);
    public void ShowRegister() => SwitchTo(ScreenType.Register);
    public void ShowOptions() => SwitchTo(ScreenType.Options);
    public void ShowSelectCharacter() => SwitchTo(ScreenType.SelectCharacter);
    public void ShowCreateCharacter() => SwitchTo(ScreenType.CreateCharacter);

    public void Hide()
    {
        _currentScreen = ScreenType.None;
        _isVisible = false;
        _rootPanel?.RemoveSelf();
        _rootPanel = null;
    }

    private void WireNavigation()
    {
        _loginView.RegisterRequested += () => SwitchTo(ScreenType.Register);
        _registerView.LoginRequested += () => SwitchTo(ScreenType.Login);
        _optionsView.LoginRequested += () => SwitchTo(ScreenType.Login);
        _selectCharacterView.CreateRequested += () => SwitchTo(ScreenType.CreateCharacter);
        _createCharacterView.SelectCharacterRequested += () => SwitchTo(ScreenType.SelectCharacter);
    }

    private void WireEvents()
    {
        MenuEvents.ConnectSucceeded += () => SwitchTo(ScreenType.SelectCharacter);
        MenuEvents.AlertReceived += msg => _ui.MessageBoxes.ShowInfoMessageBox("Server Alert", msg, null, "OK");
        MenuEvents.CharacterCreateOpened += () => SwitchTo(ScreenType.CreateCharacter);
        MenuEvents.CharactersUpdated += () => _selectCharacterView?.Refresh();
        MenuEvents.JoinGame += () => Hide();
        MenuEvents.ClassesUpdated += () =>
        {
            if (_currentScreen == ScreenType.CreateCharacter)
                _createCharacterView.Refresh();
        };
    }

    private void SwitchTo(ScreenType type)
    {
        if (_currentScreen == type && _isVisible) return;

        // Destroy current view
        DestroyCurrentView();

        // Build root if not present
        if (_rootPanel == null)
        {
            _rootPanel = MenuLoader.BuildRoot(_ui, _config);
            var optionsBtn = MenuLoader.BuildOptionsButton(_ui, _config);
            optionsBtn.Events.OnClick += _ =>
            {
                if (_currentScreen == ScreenType.Options) return;
                SwitchTo(ScreenType.Options);
            };
            _rootPanel.AddChild(optionsBtn);
        }

        // Build the requested screen
        _currentScreen = type;
        var screenConfig = _config.Screens.First(s => s.Name == type.ToString());
        BuildViewFor(type, screenConfig);
        _isVisible = true;
    }

    private void BuildViewFor(ScreenType type, ScreenData config)
    {
        switch (type)
        {
            case ScreenType.Login: _loginView.Build(_rootPanel!, config); break;
            case ScreenType.Register: _registerView.Build(_rootPanel!, config); break;
            case ScreenType.Options: _optionsView.Build(_rootPanel!, config); break;
            case ScreenType.SelectCharacter: _selectCharacterView.Build(_rootPanel!, config); break;
            case ScreenType.CreateCharacter: _createCharacterView.Build(_rootPanel!, config); break;
        }
    }

    private void DestroyCurrentView()
    {
        switch (_currentScreen)
        {
            case ScreenType.Login: _loginView.Destroy(); break;
            case ScreenType.Register: _registerView.Destroy(); break;
            case ScreenType.Options: _optionsView.Destroy(); break;
            case ScreenType.SelectCharacter: _selectCharacterView.Destroy(); break;
            case ScreenType.CreateCharacter: _createCharacterView.Destroy(); break;
        }
    }
}
