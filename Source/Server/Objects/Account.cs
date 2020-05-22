using Lidgren.Network;
using System.Collections.Generic;

namespace Objects
{
    class Account
    {
        // Dados básicos
        public NetConnection Connection;
        public string User = string.Empty;
        public string Password = string.Empty;
        public Game.Accesses Acess;
        public bool InEditor;
        public Objects.Player Character;
        public List<TempCharacter> Characters = new List<TempCharacter>();
        public struct TempCharacter
        {
            public string Name;
            public short Texture_Num;
            public short Level;
        }

        // Construtor
        public Account(NetConnection Connection)
        {
            this.Connection = Connection;
        }

        // Verifica se o jogador está dentro do jogo
        public bool IsPlaying => Character != null;

        public void Leave()
        {
            // Limpa os dados do jogador
            if (Character != null) Character.Leave();
            Lists.Account.Remove(this);
        }
    }
}