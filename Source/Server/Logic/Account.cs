class Account
{
    public static Player Character(byte Index)
    {
        // Retorna com os valores do personagem atual
        return Lists.Account[Index].Character[Lists.Account[Index].Using];
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
                if (Lists.Account[i].User == Name)
                    return i;

        return 0;
    }

    public static Player Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i < Lists.Account.Length; i++)
            if (IsPlaying(i))
                if (Character(i).Name == Name)
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
        public Player[] Character = new Player[Lists.Server_Data.Max_Characters + 1];

        // Construtor
        public Structure(byte Index)
        {
            // Inicializa os personagens
            for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++) Character[i] = new Player(Index);
            this.Index = Index;
        }

        public bool HasCharacter()
        {
            // Verifica se o jogador tem algum personagem
            for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
                if (!string.IsNullOrEmpty(Character[i].Name))
                    return true;

            return false;
        }

        public byte FindCharacter(string Name)
        {
            // Encontra o personagem
            for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
                if (Character[i].Name.Equals(Name))
                    return i;

            return 0;
        }
    }
}