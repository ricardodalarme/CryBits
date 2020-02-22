using System.Collections.Generic;

class Panels
{
    // Armazenamento dos dados da ferramenta
    public static List<Structure> List = new List<Structure>();

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        public byte Texture_Num;
    }

    public static Structure Get(string Name)
    {
        // Retorna o painel procurado
        return List.Find(x => x.Name.Equals(Name));
    }

    public static void Menu_Close()
    {
        // Fecha todos os paineis abertos
        Get("Connect").Visible = false;
        Get("Register").Visible = false;
        Get("Options").Visible = false;
        Get("SelectCharacter").Visible = false;
        Get("CreateCharacter").Visible = false;
    }
}