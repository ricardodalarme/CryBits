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
        Lists.Options.FPS = false;
        Lists.Options.Latency = false;

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

    public static void Sprite(short Index)
    {
        // Reseta os valores
        Lists.Sprite[Index] = new Lists.Structures.Sprite();
        Lists.Sprite[Index].Frame_Width = 32;
        Lists.Sprite[Index].Frame_Height = 32;
        Lists.Sprite[Index].Movement = new Lists.Structures.Sprite_Movement[(byte)Game.Movements.Count];
        for (byte i = 0; i < (byte)Game.Movements.Count; i++)
        {
            Lists.Sprite[Index].Movement[i] = new Lists.Structures.Sprite_Movement();
            Lists.Sprite[Index].Movement[i].Direction = new Lists.Structures.Sprite_Movement_Direction[(byte)Game.Directions.Count];
            Lists.Sprite[Index].Movement[i].Color = System.Drawing.Color.FromArgb(255, 255, 255).ToArgb();
            for (byte n = 0; n < (byte)Game.Directions.Count; n++)
            {
                Lists.Sprite[Index].Movement[i].Direction[n] = new Lists.Structures.Sprite_Movement_Direction();
                Lists.Sprite[Index].Movement[i].Direction[n].Frames = 1;
            }
        }
    }
}