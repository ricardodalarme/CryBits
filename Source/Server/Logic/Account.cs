using System.Collections.Generic;

class Account
{
    public static byte FindUser(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].User.Equals(Name))
                    return i;

        return 0;
    }

    public static Player Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Name.Equals(Name))
                    return Lists.Account[i].Character;

        return null;
    }

    public static bool MultipleAccounts(string User)
    {
        // Verifica se já há alguém conectado com essa conta
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Socket.IsConnected(i))
                if (Lists.Account[i].User.Equals(User))
                    return true;

        return false;
    }

    public class Structure
    {
        // Dados básicos
        public byte Index;
        public string User = string.Empty;
        public string Password = string.Empty;
        public Game.Accesses Acess;
        public bool InEditor;
        public bool Playing;
        public Player Character;
        public List<TempCharacter> Characters = new List<TempCharacter>();
        public struct TempCharacter
        {
            public string Name;
            public short Texture_Num;
            public short Level;
        }

        // Construtor
        public Structure(byte Index)
        {
            // Inicializa os personagens
            this.Index = Index;
        }

        // Verifica se o jogador está dentro do jogo
        public bool IsPlaying => Socket.IsConnected(Index) && Playing;
    }
}