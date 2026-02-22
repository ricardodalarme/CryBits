using System.Collections.Generic;
using CryBits.Enums;
using CryBits.Server.Systems;
using CryBits.Server.World;
using LiteNetLib;

namespace CryBits.Server.Entities;

internal class Account(NetPeer connection)
{
    public NetPeer Connection { get; } = connection;
    public string User { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
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
        GameWorld.Current.Accounts.Remove(this);
    }
}
