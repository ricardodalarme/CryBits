using System.Collections.Generic;
using System.Drawing;

class CheckBoxes
{
    // Armazenamento dos dados da ferramenta
    public static List<Structure> List = new List<Structure>();

    // Margem da textura até o texto
    public const byte Margin = 4;

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        // Dados
        public string Text;
        public bool Checked;

        // Eventos
        public void MouseUp()
        {
            // Tamanho do marcador
            Size Texture_Size = Graphics.TSize(Graphics.Tex_CheckBox);
            int Text_Width = Tools.MeasureString(Text);
            Size Box = new Size(Texture_Size.Width / 2 + Text_Width + Margin, Texture_Size.Height);

            // Somente se estiver sobrepondo a ferramenta
            if (!Tools.IsAbove(new Rectangle(Position, Box))) return;

            // Altera o estado do marcador
            Checked = !Checked;

            // Executa o evento
            Execute(Name);
            Audio.Sound.Play(Audio.Sounds.Click);
        }
    }

    public static Structure Get(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 0; i < List.Count; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Execute(string Name)
    {
        // Executa o evento do marcador
        switch (Name)
        {
            case "Sounds": Sounds(); break;
            case "Musics": Musics(); break;
            case "Connect_Save_Username": SaveUsername(); break;
            case "GenderMale": GenreName(); break;
            case "GenderFemale": GenreFemale(); break;
            case "Options_Sounds": Sounds(); break;
            case "Options_Musics": Musics(); break;
            case "Options_Chat": Chat(); break;
        }
    }
    
    public static void Sounds()
    {
        // Salva os dados
        Lists.Options.Sounds = !Lists.Options.Sounds;
        if (!Lists.Options.Sounds) Audio.Sound.Stop_All();
        Write.Options();
    }

    public static void Musics()
    {
        // Salva os dados
        Lists.Options.Musics = !Lists.Options.Musics;
        Write.Options();

        // Para ou reproduz a música dependendo do estado do marcador
        if (!Lists.Options.Musics)
            Audio.Music.Stop();
        else if (Tools.CurrentWindow == Tools.Windows.Menu)
            Audio.Music.Play(Audio.Musics.Menu);
        else if (Tools.CurrentWindow == Tools.Windows.Game)
            Audio.Music.Play((Audio.Musics)Lists.Map.Music);
    }

    public static void SaveUsername()
    {
        // Salva os dados
        Lists.Options.SaveUsername = Get("Connect_Save_Username").Checked;
        Write.Options();
    }

    public static void GenreName()
    {
        // Altera o estado do marcador de outro gênero
        Get("GenderFemale").Checked = !Get("GenderMale").Checked;
        Game.CreateCharacter_Tex = 0;
    }

    public static void GenreFemale()
    {
        // Altera o estado do marcador de outro gênero
        Get("GenderMale").Checked = !Get("GenderFemale").Checked;
        Game.CreateCharacter_Tex = 0;
    }

    public static void Chat()
    {
        // Desabilita a prévia do chat
        Tools.Chat_Text_Visible = Get("Options_Chat").Checked;
    }
}