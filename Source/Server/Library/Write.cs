using System.IO;

class Write
{
    public static void Player(byte Index)
    {
        string Directory = Directories.Accounts.FullName + Lists.Player[Index].User + Directories.Format;

        // Evita erros
        if (Lists.Player[Index].User == string.Empty) return;

        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite(Directory));

        // Salva os dados no arquivo
        Data.Write(Lists.Player[Index].User);
        Data.Write(Lists.Player[Index].Password);
        Data.Write((byte)Lists.Player[Index].Acess);

        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
        {
            Data.Write(Lists.Player[Index].Character[i].Name);
            Data.Write(Lists.Player[Index].Character[i].Class);
            Data.Write(Lists.Player[Index].Character[i].Genre);
            Data.Write(Lists.Player[Index].Character[i].Level);
            Data.Write(Lists.Player[Index].Character[i].Experience);
            Data.Write(Lists.Player[Index].Character[i].Points);
            Data.Write(Lists.Player[Index].Character[i].Map);
            Data.Write(Lists.Player[Index].Character[i].X);
            Data.Write(Lists.Player[Index].Character[i].Y);
            Data.Write((byte)Lists.Player[Index].Character[i].Direction);
            for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++) Data.Write(Lists.Player[Index].Character[i].Vital[n]);
            for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) Data.Write(Lists.Player[Index].Character[i].Attribute[n]);
            for (byte n = 1; n <= Game.Max_Inventory; n++)
            {
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Item_Num);
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Amount);
            }
            for (byte n = 0; n <= (byte)Game.Equipments.Amount - 1; n++) Data.Write(Lists.Player[Index].Character[i].Equipment[n]);
            for (byte n = 1; n <= Game.Max_Hotbar; n++)
            {
                Data.Write(Lists.Player[Index].Character[i].Hotbar[n].Type);
                Data.Write(Lists.Player[Index].Character[i].Hotbar[n].Slot);
            }
        }

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Character(string Name)
    {
        // Cria um arquivo temporário
        StreamWriter Data = new StreamWriter(Directories.Characters.FullName, true);

        // Salva o nome do personagem no arquivo
        Data.Write(";" + Name + ":");

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Characters(string Characters_Name)
    {
        // Cria um arquivo temporário
        StreamWriter Data = new StreamWriter(Directories.Characters.FullName);

        // Salva o nome do personagem no arquivo
        Data.Write(Characters_Name);

        // Descarrega o arquivo
        Data.Dispose();
    }
}