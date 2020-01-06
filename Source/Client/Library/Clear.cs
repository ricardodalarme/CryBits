class Clear
{
    public static void Options()
    {
        // Defini os dados das opções
        Lists.Options.GameName = "CryBits";
        Lists.Options.SaveUsername = true;
        Lists.Options.Musics = true;
        Lists.Options.Sounds = true;
        Lists.Options.Username = string.Empty;

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

        // Reseta os valores
        Lists.Player[Index].Name = string.Empty;
    }
}