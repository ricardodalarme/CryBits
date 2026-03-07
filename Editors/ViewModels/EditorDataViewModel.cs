using CryBits.Editors.Network;
using static CryBits.Globals;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorDataViewModel : ViewModelBase
{
    private string _gameName;
    private string _welcomeMessage;
    private short _port;
    private byte _maxPlayers;
    private byte _maxCharacters;
    private byte _maxPartyMembers;
    private byte _maxMapItems;
    private byte _numPoints;
    private byte _minNameLength;
    private byte _maxNameLength;
    private byte _minPasswordLength;
    private byte _maxPasswordLength;

    public string GameName
    {
        get => _gameName;
        set => SetProperty(ref _gameName, value);
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public short Port
    {
        get => _port;
        set => SetProperty(ref _port, value);
    }

    public byte MaxPlayers
    {
        get => _maxPlayers;
        set => SetProperty(ref _maxPlayers, value);
    }

    public byte MaxCharacters
    {
        get => _maxCharacters;
        set => SetProperty(ref _maxCharacters, value);
    }

    public byte MaxPartyMembers
    {
        get => _maxPartyMembers;
        set => SetProperty(ref _maxPartyMembers, value);
    }

    public byte MaxMapItems
    {
        get => _maxMapItems;
        set => SetProperty(ref _maxMapItems, value);
    }

    public byte NumPoints
    {
        get => _numPoints;
        set => SetProperty(ref _numPoints, value);
    }

    public byte MinNameLength
    {
        get => _minNameLength;
        set => SetProperty(ref _minNameLength, value);
    }

    public byte MaxNameLength
    {
        get => _maxNameLength;
        set => SetProperty(ref _maxNameLength, value);
    }

    public byte MinPasswordLength
    {
        get => _minPasswordLength;
        set => SetProperty(ref _minPasswordLength, value);
    }

    public byte MaxPasswordLength
    {
        get => _maxPasswordLength;
        set => SetProperty(ref _maxPasswordLength, value);
    }

    public EditorDataViewModel()
    {
        _gameName = Config.GameName;
        _welcomeMessage = Config.WelcomeMessage;
        _port = Config.Port;
        _maxPlayers = Config.MaxPlayers;
        _maxCharacters = Config.MaxCharacters;
        _maxPartyMembers = Config.MaxPartyMembers;
        _maxMapItems = Config.MaxMapItems;
        _numPoints = Config.NumPoints;
        _minNameLength = Config.MinNameLength;
        _maxNameLength = Config.MaxNameLength;
        _minPasswordLength = Config.MinPasswordLength;
        _maxPasswordLength = Config.MaxPasswordLength;
    }

    public void Save()
    {
        Config.GameName = GameName;
        Config.WelcomeMessage = WelcomeMessage;
        Config.Port = Port;
        Config.MaxPlayers = MaxPlayers;
        Config.MaxCharacters = MaxCharacters;
        Config.MaxPartyMembers = MaxPartyMembers;
        Config.MaxMapItems = MaxMapItems;
        Config.NumPoints = NumPoints;
        Config.MinNameLength = MinNameLength;
        Config.MaxNameLength = MaxNameLength;
        Config.MinPasswordLength = MinPasswordLength;
        Config.MaxPasswordLength = MaxPasswordLength;
        PackageSender.WriteServerData();
    }

    public void Cancel()
    {
        PackageSender.RequestServerData();
    }
}
