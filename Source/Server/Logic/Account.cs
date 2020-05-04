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

    public static byte Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i < Lists.Account.Length; i++)
            if (IsPlaying(i))
                if (Character(i).Name == Name)
                    return i;

        return 0;
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
        public string User;
        public string Password;
        public Game.Accesses Acess;
        public byte Using;
        public bool InEditor;
        public bool Playing;
        public Player[] Character;

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