class Clear
{
    public static void All()
    {
        // Limpa todos os dados necessários
        Players();
    }

    public static void Players()
    {
        // Redimensiona a lista
        Lists.Player = new Lists.Structures.Player[Lists.Server_Data.Max_Players + 1];
        Lists.TempPlayer = new Lists.Structures.TempPlayer[Lists.Server_Data.Max_Players + 1];

        // Limpa os dados de todos jogadores
        for (byte i = 1; i <= Lists.Server_Data.Max_Players; i++)
            Player(i);
    }

    public static void Player(byte Index)
    {
        // Limpa os dados do jogador
        Lists.Player[Index] = new Lists.Structures.Player();
        Lists.TempPlayer[Index] = new Lists.Structures.TempPlayer();
        Lists.Player[Index].User = string.Empty;
        Lists.Player[Index].Password = string.Empty;
        Lists.Player[Index].Character = new Player.Character_Structure[Lists.Server_Data.Max_Characters + 1];

        // Limpa os dados do personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            Player_Character(Index, i);
    }

    public static void Player_Character(byte Index, byte Char_Num)
    {
        // Limpa os dados
        Lists.Player[Index].Character[Char_Num] = new Player.Character_Structure
        {
            Index = Index,
            Inventory = new Lists.Structures.Inventories[Game.Max_Inventory + 1],
            Equipment = new short[(byte)Game.Equipments.Amount],
            Hotbar = new Lists.Structures.Hotbar[Game.Max_Hotbar + 1]
        };
    }
}