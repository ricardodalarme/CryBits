namespace CryBits.Editors.Entities;

/// <summary>
/// Cross-platform replacement for the static EditorInterface.Tree field
/// that used to live in the WinForms EditorInterface form.
/// </summary>
internal class InterfaceData
{
    public static InterfaceData Instance { get; } = new();

    /// <summary>Root node whose children are the top-level Screen windows.</summary>
    public readonly InterfaceNode Tree = new();
}
