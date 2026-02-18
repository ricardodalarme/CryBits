using System.Collections.Generic;
using CryBits.Enums;
using LiteNetLib;

namespace CryBits.Server.Entities;

internal class Account
{
    // Lista de dados
    public static readonly List<Account> List = new();

    // Dados básicos
    public NetPeer Connection { get; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Access Access { get; set; }
    public bool InEditor { get; set; }
    public Player Character { get; set; }
    public List<TempCharacter> Characters { get; set; } = new();
    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    // Construtor
    public Account(NetPeer connection)
    {
        Connection = connection;
    }

    // Verifica se o jogador está dentro do jogo
    public bool IsPlaying => Character != null;

    public void Leave()
    {
        // Limpa os dados do jogador
        Character?.Leave();
        List.Remove(this);
    }
}