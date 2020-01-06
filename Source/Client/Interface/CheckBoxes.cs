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
        public bool State;

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
            State = !State;

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
            case "Sons": Sounds(); break;
            case "Músicas": Musics(); break;
            case "SalvarUsuário": SaveUsername(); break;
            case "GêneroMasculino": GenreName(); break;
            case "GêneroFeminino": GenreFemale(); break;
        }
    }

    public static void Sounds()
    {
        // Salva os dados
        Lists.Options.Sounds = Get("Sons").State;
        Write.Options();
    }

    public static void Musics()
    {
        // Salva os dados
        Lists.Options.Musics = Get("Músicas").State;
        Write.Options();

        // Para ou reproduz a música dependendo do estado do marcador
        if (!Lists.Options.Musics)
            Audio.Music.Stop();
        else
            Audio.Music.Play(Audio.Musics.Menu);
    }

    public static void SaveUsername()
    {
        // Salva os dados
        Lists.Options.SaveUsername = Get("SalvarUsuário").State;
        Write.Options();
    }

    public static void GenreName()
    {
        // Altera o estado do marcador de outro gênero
        Get("GêneroFeminino").State = !Get("GêneroMasculino").State;
    }

    public static void GenreFemale()
    {
        // Altera o estado do marcador de outro gênero
        Get("GêneroMasculino").State = !Get("GêneroFeminino").State;
    }
}