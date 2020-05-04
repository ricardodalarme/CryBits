class Player
{
    public static Character Character(byte Index)
    {
        // Retorna com os valores do personagem atual
        return Lists.Player[Index].Character[Lists.Temp_Player[Index].Using];
    }

    public static bool IsPlaying(byte Index)
    {
        // Verifica se o jogador está dentro do jogo
        if (Socket.IsConnected(Index))
            if (Lists.Temp_Player[Index].Playing)
                return true;

        return false;
    }

    public static byte FindUser(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (IsPlaying(i))
                if (Lists.Player[i].User == Name)
                    return i;

        return 0;
    }

    public static byte Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i < Lists.Player.Length; i++)
            if (IsPlaying(i))
                if (Character(i).Name == Name)
                    return i;

        return 0;
    }

    public static byte FindCharacter(byte Index, string Name)
    {
        // Encontra o personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            if (Lists.Player[Index].Character[i].Name.Equals(Name))
                return i;

        return 0;
    }

    public static bool HasCharacter(byte Index)
    {
        // Verifica se o jogador tem algum personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            if (!string.IsNullOrEmpty(Lists.Player[Index].Character[i].Name))
                return true;

        return false;
    }

    public static bool MultipleAccounts(string User)
    {
        // Verifica se já há alguém conectado com essa conta
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Socket.IsConnected(i))
                if (Lists.Player[i].User.Equals(User))
                    return true;

        return false;
    }
}