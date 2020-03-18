class Clear
{
    public static void Options()
    {
        // Defini os dados das opções
        Lists.Options.Game_Name = "CryBits";
        Lists.Options.SaveUsername = true;
        Lists.Options.Musics = true;
        Lists.Options.Sounds = true;
        Lists.Options.Username = string.Empty;
        Lists.Options.Chat = true;

        // Salva o que foi modificado
        Write.Options();
    }

    public static void Player(byte Index)
    {
        // Limpa a estrutura
        Lists.Player[Index] = new Lists.Structures.Player();
        Lists.Player[Index].Vital = new short[(byte)Game.Vitals.Count];
        Lists.Player[Index].Max_Vital = new short[(byte)Game.Vitals.Count];
        Lists.Player[Index].Attribute = new short[(byte)Game.Attributes.Count];
        Lists.Player[Index].Equipment = new short[(byte)Game.Equipments.Count];
        Lists.Player[Index].Party = System.Array.Empty<byte>();

        // Reseta os valores
        Lists.Player[Index].Name = string.Empty;
    }
}