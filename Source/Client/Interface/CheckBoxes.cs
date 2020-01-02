using System.Drawing;

public class CheckBoxes
{
    // Armazenamento dos dados da ferramenta
    public static Structure[] List;

    // Margem da textura até o texto
    public const byte Margin = 4;

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        public string Text;
        public bool State;
    }

    public static byte GetIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name == Name)
                return i;

        return 0;
    }

    public static Structure Get(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name == Name)
                return List[i];

        return null;
    }

    public class Events
    {
        public static void MouseUp(byte Index)
        {
            // Somente se estiver disponível
            if (!List[Index].Able) return;

            // Tamanho do marcador
            Size Texture_Size = Graphics.TSize(Graphics.Tex_CheckBox);
            int Text_Width = Tools.MeasureString(List[Index].Text);
            Size Box = new Size(Texture_Size.Width / 2 + Text_Width + Margin, Texture_Size.Height);

            // Somente se estiver sobrepondo a ferramenta
            if (!Tools.IsAbove(new Rectangle(List[Index].Position, Box))) return;

            // Altera o estado do marcador
            List[Index].State = !List[Index].State;

            // Executa o evento
            Execute(List[Index].Name);
            Audio.Sound.Play(Audio.Sounds.Click);
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
}