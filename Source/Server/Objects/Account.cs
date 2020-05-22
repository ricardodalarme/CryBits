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
        public Player Character;
        public List<Lists.Structures.TempCharacter> Characters = new List<Lists.Structures.TempCharacter>();

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