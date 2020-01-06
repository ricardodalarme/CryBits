﻿using System.Collections.Generic;

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
        // Lista os nomes das ferramentas
        for (byte i = 0; i < List.Count; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Menu_Close()
    {
        // Fecha todos os paineis abertos
        Get("Conectar").Visible = false;
        Get("Registrar").Visible = false;
        Get("Opções").Visible = false;
        Get("SelecionarPersonagem").Visible = false;
        Get("CriarPersonagem").Visible = false;
    }
}