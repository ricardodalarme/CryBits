namespace CryBits.Client.UI.Game;

internal static class GameEvents
{
    public static event Action? BarsUpdated;
    public static event Action? CharacterUpdated;
    public static event Action? ChatMessageAdded;
    public static event Action<string, int>? InventoryChanged;
    public static event Action? ChatToggle;

    public static void FireBarsUpdated() => BarsUpdated?.Invoke();
    public static void FireCharacterUpdated() => CharacterUpdated?.Invoke();
    public static void FireChatMessageAdded() => ChatMessageAdded?.Invoke();
    public static void FireInventoryChanged(string panelName, int slot) => InventoryChanged?.Invoke(panelName, slot);
    public static void FireChatToggle() => ChatToggle?.Invoke();
}
