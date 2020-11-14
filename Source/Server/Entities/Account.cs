using System.Collections.Generic;
using Lidgren.Network;

namespace CryBits.Server.Entities
{
    internal class Account
    {
        // Lista de dados
        public static List<Account> List = new List<Account>();

        // Dados básicos
        public NetConnection Connection;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Accesses Access { get; set; }
        public bool InEditor;
        public Player Character;
        public List<TempCharacter> Characters = new List<TempCharacter>();
        public struct TempCharacter
        {
            public string Name;
            public short Texture_Num;
            public short Level;
        }

        // Construtor
        public Account(NetConnection connection)
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
}