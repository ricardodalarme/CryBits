using CryBits.Server.Entities;
using CryBits;
using CryBits.Entities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static CryBits.Server.Logic.Utils;

namespace CryBits.Server.Library
{
    static class Write
    {
        public static void Settings()
        {
            // Escreve as configurações
            using (var Data = new BinaryWriter(Directories.Settings.OpenWrite()))
            {
                Data.Write(Game_Name);
                Data.Write(Welcome_Message);
                Data.Write(Port);
                Data.Write(Max_Players);
                Data.Write(Max_Characters);
                Data.Write(Max_Party_Members);
                Data.Write(Max_Map_Items);
                Data.Write(Num_Points);
                Data.Write(Max_Name_Length);
                Data.Write(Min_Name_Length);
                Data.Write(Max_Password_Length);
                Data.Write(Min_Password_Length);
            }
        }

        public static void Account(Account Account)
        {
            FileInfo File = new FileInfo(Directories.Accounts.FullName + Account.User + "\\Data" + Directories.Format);

            // Evita erros
            if (!File.Directory.Exists) File.Directory.Create();

            // Escreve os dados da conta no arquivo
            using (var Data = new BinaryWriter(File.OpenWrite()))
            {
                Data.Write(Account.User);
                Data.Write(Account.Password);
                Data.Write((byte)Account.Acess);
            }
        }

        public static void Character(Account Account)
        {
            FileInfo File = new FileInfo(Directories.Accounts.FullName + Account.User + "\\Characters\\" + Account.Character.Name + Directories.Format);

            // Evita erros
            if (!File.Directory.Exists) File.Directory.Create();

            // Salva os dados do personagem no arquivo
            using (var Data = new BinaryWriter(File.OpenWrite()))
            {
                Data.Write(Account.Character.Name);
                Data.Write(Account.Character.Texture_Num);
                Data.Write(Account.Character.Level);
                Data.Write(Account.Character.Class.GetID());
                Data.Write(Account.Character.Genre);
                Data.Write(Account.Character.Experience);
                Data.Write(Account.Character.Points);
                Data.Write(Account.Character.Map.GetID());
                Data.Write(Account.Character.X);
                Data.Write(Account.Character.Y);
                Data.Write((byte)Account.Character.Direction);
                for (byte n = 0; n < (byte)Vitals.Count; n++) Data.Write(Account.Character.Vital[n]);
                for (byte n = 0; n < (byte)Attributes.Count; n++) Data.Write(Account.Character.Attribute[n]);
                for (byte n = 1; n <= Max_Inventory; n++)
                {
                    Data.Write(Account.Character.Inventory[n].Item.GetID());
                    Data.Write(Account.Character.Inventory[n].Amount);
                }
                for (byte n = 0; n < (byte)Equipments.Count; n++) Data.Write(Account.Character.Equipment[n].GetID());
                for (byte n = 0; n < Max_Hotbar; n++)
                {
                    Data.Write((byte)Account.Character.Hotbar[n].Type);
                    Data.Write(Account.Character.Hotbar[n].Slot);
                }
            }
        }

        public static void Character_Name(string Name)
        {
            // Salva o nome de um personagem no arquivo
            using (var Data = new StreamWriter(Directories.Characters.FullName, true))
                Data.Write(";" + Name + ":");
        }

        public static void Characters_Name(string Characters_Name)
        {
            // Salva o nome de todos os personagens no arquivo
            using (var Data = new StreamWriter(Directories.Characters.FullName))
                Data.Write(Characters_Name);
        }

        public static void Class(Class Class)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Classes.FullName + Class.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Class);
        }

        public static void NPC(NPC NPC)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.NPCs.FullName + NPC.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, NPC);
        }

        public static void Item(Item Item)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Items.FullName + Item.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Item);
        }

        public static void Map(Map Map)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Maps.FullName + Map.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Map);
        }

        public static void Shop(Shop Shop)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Shops.FullName + Shop.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Shop);
        }
    }
}