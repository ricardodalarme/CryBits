using Lidgren.Network;
using System.Collections.Generic;

class Account
{
    public static Structure Find(string Name) => Lists.Account.Find(x => x.User.Equals(Name));

    public static Objects.Player FindPlayer(string Name)
    {
        // Encontra o usuário
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Name.Equals(Name))
                    return Lists.Account[i].Character;

        return null;
    }

    public class Structure
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
        public Structure(NetConnection Connection)
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