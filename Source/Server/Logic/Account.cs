using System.Collections.Generic;

class Account
{
    public static Structure Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].User.Equals(Name))
                    return Lists.Account[i];

        return null;
    }

    public static Player FindPlayer(string Name)
    {
        // Encontra o usuário
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Name.Equals(Name))
                    return Lists.Account[i].Character;

        return null;
    }

    public static bool MultipleAccounts(string User)
    {
        // Verifica se já há alguém conectado com essa conta
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Socket.IsConnected(Lists.Account[i]))
                if (Lists.Account[i].User.Equals(User))
                    return true;

        return false;
    }

    public class Structure
    {
        // Dados básicos
        public Lidgren.Network.NetConnection Connection;
        public string User = string.Empty;
        public string Password = string.Empty;
        public Game.Accesses Acess;
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
        public Structure(Lidgren.Network.NetConnection Connection)
        {
            this.Connection = Connection;
        }

        // Verifica se o jogador está dentro do jogo
        public bool IsPlaying => Socket.IsConnected(this) && Character != null;

        public void Leave()
        {
            // Limpa os dados do jogador
            if (Character != null) Character.Leave();
        }
    }
}