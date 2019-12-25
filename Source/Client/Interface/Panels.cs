public class Panels
{
    // Armazenamento dos dados da ferramenta
    public static Estrutura[] List = new Estrutura[1];

    // Estrutura da ferramenta
    public class Estrutura
    {
        public byte Texture;
        public Tools.Structure General;
    }

    public static byte FindIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].General.Name == Name)
                return i;

        return 0;
    }

    public static Estrutura Find(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].General.Name == Name)
                return List[i];

        return null;
    }

    public static void Menu_Close()
    {
        // Fecha todos os paineis abertos
        Find("Conectar").General.Visible = false;
        Find("Registrar").General.Visible = false;
        Find("Opções").General.Visible = false;
        Find("SelecionarPersonagem").General.Visible = false;
        Find("CriarPersonagem").General.Visible = false;
    }
}