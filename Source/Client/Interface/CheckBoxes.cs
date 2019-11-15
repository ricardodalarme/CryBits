using System.Drawing;
using System.Windows.Forms;

public class CheckBoxes
{
    // Armazenamento dos dados da ferramenta
    public static Structure[] List = new Structure[1];

    // Margem da textura até o texto
    public const byte Margin = 4;

    // Estrutura da ferramenta
    public class Structure
    {
        public string Text;
        public bool State;
        public Tools.Structure General;
    }

    public static byte FindIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i <= List.GetUpperBound(0); i++)
            if (List[i].General.Name == Name)
                return i;

        return 0;
    }

    public static Structure Find(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i <= List.GetUpperBound(0); i++)
            if (List[i].General.Name == Name)
                return List[i];

        return null;
    }

    public class Events
    {
        public static void MouseUp(MouseEventArgs e, byte Index)
        {
            int Text_Width; Size Texture_Size; Size Box;

            // Somente se necessário
            if (!List[Index].General.Able) return;

            // Tamanho do marcador
            Texture_Size = Graphics.TSize(Graphics.Tex_CheckBox);
            Text_Width = Tools.MeasureString(List[Index].Text);
            Box = new Size(Texture_Size.Width / 2 + Text_Width + Margin, Texture_Size.Height);

            // Somente se estiver sobrepondo a ferramenta
            if (!Tools.IsAbove(new Rectangle(List[Index].General.Position, Box))) return;

            // Altera o estado do marcador
            List[Index].State = !List[Index].State;

            // Executa o evento
            Execute(List[Index].General.Name);
            Audio.Sound.Play(Audio.Sounds.Click);
        }

        public static void Execute(string name)
        {
            // Executa o evento do marcador
            switch (name)
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
            Lists.Options.Sounds = Find("Sons").State;
            Write.Options();
        }

        public static void Musics()
        {
            // Salva os dados
            Lists.Options.Musics = Find("Músicas").State;
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
            Lists.Options.SaveUsername = Find("SalvarUsuário").State;
            Write.Options();
        }

        public static void GenreName()
        {
            // Altera o estado do marcador de outro gênero
            Find("GêneroFeminino").State = !Find("GêneroMasculino").State;
        }

        public static void GenreFemale()
        {
            // Altera o estado do marcador de outro gênero
            Find("GêneroMasculino").State = !Find("GêneroFeminino").State;
        }
    }
}