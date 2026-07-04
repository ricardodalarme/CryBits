using CryBits.Client.Framework;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen
{
    private readonly BackgroundView _backgroundView;
    private readonly LoginView _loginView;
    private readonly RegisterView _registerView;
    private readonly OptionsView _optionsPanel;
    private readonly SelectCharacterView _selectCharacterView;
    private readonly CreateCharacterView _createCharacterView;

    private readonly AudioManager _audio;
    private readonly UiContext _uiContext;

    internal MenuScreen(AudioManager audio, UiContext uiContext, AuthSender authSender, AccountSender accountSender,
        PortraitRenderer characterRenderer, DefinitionCatalog catalog, Connection connection)
    {
        _audio = audio;
        _uiContext = uiContext;
        _backgroundView = new(uiContext, connection, this);
        _loginView = new(uiContext, authSender, connection, this);
        _registerView = new(uiContext, authSender, connection, this);
        _optionsPanel = new(uiContext, audio, connection, this);
        _selectCharacterView = new(uiContext, accountSender, characterRenderer);
        _createCharacterView = new(uiContext, accountSender, characterRenderer, catalog, this);
    }

    public void Open()
    {
        _audio.StopAllSounds();
        if (Options.Instance.Musics) _audio.PlayMusic(Musics.Menu);

        _uiContext.LoadScreen("Menu");
        _backgroundView.Bind();

        ShowLogin();

        _uiContext.CurrentScreen = ScreenType.Menu;
    }

    public void ShowLogin()
    {
        CloseAllViews();
        _loginView.Open();
    }

    public void ShowRegister()
    {
        CloseAllViews();
        _registerView.Open();
    }

    public void ShowOptions()
    {
        CloseAllViews();
        _optionsPanel.Open();
    }

    public void ShowSelectCharacter(SelectCharacterView.TempCharacter[] characters)
    {
        CloseAllViews();
        _selectCharacterView.Open(characters);
    }

    public void ShowCreateCharacter()
    {
        CloseAllViews();
        _createCharacterView.Open();
    }

    public void UpdateClassLabels()
    {
        _createCharacterView.UpdateClassLabels();
    }

    public void CloseAllViews()
    {
        _loginView.Close();
        _registerView.Close();
        _optionsPanel.Close();
        _selectCharacterView.Close();
        _createCharacterView.Close();
    }

    public void Unbind()
    {
        _backgroundView.Unbind();
        CloseAllViews();
    }
}
