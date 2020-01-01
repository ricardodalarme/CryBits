public class Panels
{
    // Armazenamento dos dados da ferramenta
    public static Structure[] List = new Structure[1];

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        public byte Texture;
    }

    public static byte FindIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name == Name)
                return i;

        return 0;
    }

    public static Structure Find(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name == Name)
                return List[i];

        return null;
    }

    public static void Menu_Close()
    {
        // Fecha todos os paineis abertos
        Find("Conectar").Visible = false;
        Find("Registrar").Visible = false;
        Find("Opções").Visible = false;
        Find("SelecionarPersonagem").Visible = false;
        Find("CriarPersonagem").Visible = false;
    }
}