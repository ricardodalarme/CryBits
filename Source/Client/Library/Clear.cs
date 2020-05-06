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
}