using System.Collections.Generic;

class Account
{
    public static Player Character(byte Index)
    {
        // Retorna com os valores do personagem atual
        return Lists.Account[Index].Character;
    }

    public static bool IsPlaying(byte Index)
    {
        // Verifica se o jogador está dentro do jogo
        if (Socket.IsConnected(Index))
            if (Lists.Account[Index].Playing)
                return true;

        return false;
    }

    public static byte FindUser(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (IsPlaying(i))
                if (Lists.Account[i].User.Equals(Name))
                    return i;

        return 0;
    }

    public static Player Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (IsPlaying(i))
                if (Character(i).Name.Equals(Name))
                    return Character(i);

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
        public byte Using;
        public bool InEditor;
        public bool Playing;
        public Player Character;
        public List<TempCharacter> Characters = new List<TempCharacter>();

        // Construtor
        public Structure(byte Index)
        {
            // Inicializa os personagens
            this.Index = Index;
        }

        // Encontra o personagem
      //  public byte FindCharacter(string Name) => Characters.Find(x => x.Name.Equals(Name));

        public struct TempCharacter
        {
            public string Name;
            public short Texture_Num;
            public short Level;
        }
    }
}