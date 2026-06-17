namespace CryBits.Client.UI.Menu;

internal static class MenuEvents
{
    public static event Action? ConnectSucceeded;
    public static event Action<string>? AlertReceived;
    public static event Action? CharacterCreateOpened;
    public static event Action? CharactersUpdated;
    public static event Action? JoinGame;
    public static event Action? ClassesUpdated;

    public static void FireConnectSucceeded() => ConnectSucceeded?.Invoke();
    public static void FireAlert(string message) => AlertReceived?.Invoke(message);
    public static void FireCharacterCreateOpened() => CharacterCreateOpened?.Invoke();
    public static void FireCharactersUpdated() => CharactersUpdated?.Invoke();
    public static void FireJoinGame() => JoinGame?.Invoke();
    public static void FireClassesUpdated() => ClassesUpdated?.Invoke();
}
