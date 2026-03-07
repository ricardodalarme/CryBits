using System;
using System.IO;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using SFML.Window;

namespace CryBits.Client.Framework.Persistence.Repositories;

/// <summary>
/// Reads and writes the player's key-binding configuration to
/// <c>Data/Keybinds.json</c>.  When the file does not exist a default
/// binding file is created automatically.
/// </summary>
public static class KeyBindingsRepository
{
    /// <summary>
    /// Load key bindings from JSON into the runtime <see cref="KeyBindings"/> object.
    /// Creates a default file if none exists.
    /// </summary>
    public static void Read()
    {
        if (!Directories.Keybinds.Exists)
        {
            Write();
            return;
        }

        using var stream = Directories.Keybinds.OpenRead();
        KeyBindingsDto dto;
        try
        {
            dto = JsonSerializer.Deserialize<KeyBindingsDto>(stream, JsonConfig.Options) ?? new KeyBindingsDto();
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"[KeyBindings] Failed to parse {Directories.Keybinds.FullName}: {ex.Message}. Using defaults.");
            dto = new KeyBindingsDto();
        }

        KeyBindings.MoveUp    = ParseScancode(dto.MoveUp,    KeyBindings.MoveUp);
        KeyBindings.MoveDown  = ParseScancode(dto.MoveDown,  KeyBindings.MoveDown);
        KeyBindings.MoveLeft  = ParseScancode(dto.MoveLeft,  KeyBindings.MoveLeft);
        KeyBindings.MoveRight = ParseScancode(dto.MoveRight, KeyBindings.MoveRight);

        KeyBindings.Run    = ParseKey(dto.Run,    KeyBindings.Run);
        KeyBindings.Attack = ParseKey(dto.Attack, KeyBindings.Attack);

        KeyBindings.Chat    = ParseKey(dto.Chat,    KeyBindings.Chat);
        KeyBindings.Collect = ParseKey(dto.Collect, KeyBindings.Collect);
        KeyBindings.Hotbar1 = ParseKey(dto.Hotbar1, KeyBindings.Hotbar1);
        KeyBindings.Hotbar2 = ParseKey(dto.Hotbar2, KeyBindings.Hotbar2);
        KeyBindings.Hotbar3 = ParseKey(dto.Hotbar3, KeyBindings.Hotbar3);
        KeyBindings.Hotbar4 = ParseKey(dto.Hotbar4, KeyBindings.Hotbar4);
        KeyBindings.Hotbar5 = ParseKey(dto.Hotbar5, KeyBindings.Hotbar5);
        KeyBindings.Hotbar6 = ParseKey(dto.Hotbar6, KeyBindings.Hotbar6);
        KeyBindings.Hotbar7 = ParseKey(dto.Hotbar7, KeyBindings.Hotbar7);
        KeyBindings.Hotbar8 = ParseKey(dto.Hotbar8, KeyBindings.Hotbar8);
        KeyBindings.Hotbar9 = ParseKey(dto.Hotbar9, KeyBindings.Hotbar9);
        KeyBindings.Hotbar0 = ParseKey(dto.Hotbar0, KeyBindings.Hotbar0);
    }

    /// <summary>
    /// Persist the current runtime <see cref="KeyBindings"/> to the JSON file.
    /// </summary>
    public static void Write()
    {
        Directories.Keybinds.Directory?.Create();
        var dto = new KeyBindingsDto
        {
            MoveUp    = KeyBindings.MoveUp.ToString(),
            MoveDown  = KeyBindings.MoveDown.ToString(),
            MoveLeft  = KeyBindings.MoveLeft.ToString(),
            MoveRight = KeyBindings.MoveRight.ToString(),

            Run    = KeyBindings.Run.ToString(),
            Attack = KeyBindings.Attack.ToString(),

            Chat    = KeyBindings.Chat.ToString(),
            Collect = KeyBindings.Collect.ToString(),
            Hotbar1 = KeyBindings.Hotbar1.ToString(),
            Hotbar2 = KeyBindings.Hotbar2.ToString(),
            Hotbar3 = KeyBindings.Hotbar3.ToString(),
            Hotbar4 = KeyBindings.Hotbar4.ToString(),
            Hotbar5 = KeyBindings.Hotbar5.ToString(),
            Hotbar6 = KeyBindings.Hotbar6.ToString(),
            Hotbar7 = KeyBindings.Hotbar7.ToString(),
            Hotbar8 = KeyBindings.Hotbar8.ToString(),
            Hotbar9 = KeyBindings.Hotbar9.ToString(),
            Hotbar0 = KeyBindings.Hotbar0.ToString(),
        };
        using var stream = Directories.Keybinds.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, dto, JsonConfig.Options);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Keyboard.Scancode ParseScancode(string value, Keyboard.Scancode fallback)
    {
        if (Enum.TryParse<Keyboard.Scancode>(value, ignoreCase: true, out var result))
            return result;

        return fallback;
    }

    private static Keyboard.Key ParseKey(string value, Keyboard.Key fallback)
    {
        if (Enum.TryParse<Keyboard.Key>(value, ignoreCase: true, out var result))
            return result;

        return fallback;
    }
}
