using Lidgren.Network;
using System.Collections.Generic;

namespace CryBits.Server.Entities
{
    class Account
    {
        // Lista de dados
        public static List<Account> List = new List<Account>();

        // Dados básicos
        public NetConnection Connection;
        public string User = string.Empty;
        public string Password = string.Empty;
        public Accesses Acess;
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
            this.Connection = connection;
        }

        // Verifica se o jogador está dentro do jogo
        public bool IsPlaying => Character != null;

        public void Leave()
        {
            // Limpa os dados do jogador
            if (Character != null) Character.Leave();
            List.Remove(this);
        }
    }
}