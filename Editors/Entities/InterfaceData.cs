namespace CryBits.Editors.Entities;

/// <summary>
/// Cross-platform replacement for the static EditorInterface.Tree field
/// that used to live in the WinForms EditorInterface form.
/// </summary>
internal static class InterfaceData
{
    /// <summary>Root node whose children are the top-level Screen windows.</summary>
    public static readonly InterfaceNode Tree = new();
}
