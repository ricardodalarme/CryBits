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

    public static void Button(int Index)
    {
        // Limpa a estrutura
        Buttons.List[Index] = new Buttons.Structure();
        Buttons.List[Index].General = new Tools.Structure();
    }

    public static void TextBox(int Index)
    {
        // Limpa a estrutura
        TextBoxes.List[Index] = new TextBoxes.Structure();
        TextBoxes.List[Index].General = new Tools.Structure();
    }

    public static void Panel(int Index)
    {
        // Limpa a estrutura
        Panels.List[Index] = new Panels.Estrutura();
        Panels.List[Index].General = new Tools.Structure();
    }

    public static void CheckBox(int Index)
    {
        // Limpa a estrutura
        CheckBoxes.List[Index] = new CheckBoxes.Structure();
        CheckBoxes.List[Index].General = new Tools.Structure();
    }

    public static void Player(byte Index)
    {
        // Limpa a estrutura
        Lists.Player[Index] = new Lists.Structures.Player();
        Lists.Player[Index].Vital = new short[(byte)Game.Vitals.Amount];
        Lists.Player[Index].Max_Vital = new short[(byte)Game.Vitals.Amount];
        Lists.Player[Index].Attribute = new short[(byte)Game.Attributes.Amount];
        Lists.Player[Index].Equipment = new short[(byte)Game.Equipments.Amount];

        // Reseta os valores
        Lists.Player[Index].Name = string.Empty;
    }
}