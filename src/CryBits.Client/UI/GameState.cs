namespace CryBits.Client.UI;

public enum ScreenType { Menu, Game }

public static class GameState
{
    public static ScreenType CurrentScreen { get; set; } = ScreenType.Menu;

    public static event Action<SFML.Window.KeyEventArgs>? GameKeyReleased;

    public static void FireGameKeyReleased(SFML.Window.KeyEventArgs e) => GameKeyReleased?.Invoke(e);
}
