using CryBits.Definitions.Common;

namespace CryBits.Host.Core;

internal sealed class Account
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Access AccessLevel { get; set; }
    public List<CharacterSlot> Characters { get; set; } = [];

    public struct CharacterSlot
    {
        public string Name;
        public short TextureNum;
    }
}
