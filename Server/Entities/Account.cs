using System.Collections.Generic;
using CryBits.Enums;
using CryBits.Server.Systems;
using LiteNetLib;

namespace CryBits.Server.Entities;

internal class Account(NetPeer connection)
{
    // Active accounts list.
    public static readonly List<Account> List = [];

    // Account fields.
    public NetPeer Connection { get; } = connection;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Access Access { get; set; }
    public bool InEditor { get; set; }
    public Player Character { get; set; }
    public List<TempCharacter> Characters { get; set; } = [];

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    // Indicates whether a character is currently playing.
    public bool IsPlaying => Character != null;

    public void Leave()
    {
        if (Character != null) CharacterSystem.Leave(Character);
        List.Remove(this);
    }
}