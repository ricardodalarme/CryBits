using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Server.Entities;
using static CryBits.Defaults;
using Item = CryBits.Entities.Item;

namespace CryBits.Server.Library
{
    internal static class Write
    {
        public static void Defaults()
        {
            // Escreve as configurações
            using (var data = new BinaryWriter(Directories.Defaults.OpenWrite()))
            {
                data.Write(GameName);
                data.Write(WelcomeMessage);
                data.Write(Port);
                data.Write(MaxPlayers);
                data.Write(MaxCharacters);
                data.Write(MaxPartyMembers);
                data.Write(MaxMapItems);
                data.Write(NumPoints);
                data.Write(MaxNameLength);
                data.Write(MinNameLength);
                data.Write(MaxPasswordLength);
                data.Write(MinPasswordLength);
            }
        }

        public static void Account(Account account)
        {
            FileInfo file = new FileInfo(Directories.Accounts.FullName + account.User + "\\Data" + Directories.Format);

            // Evita erros
            if (!file.Directory.Exists) file.Directory.Create();

            // Escreve os dados da conta no arquivo
            using (var data = new BinaryWriter(file.OpenWrite()))
            {
                data.Write(account.User);
                data.Write(account.Password);
                data.Write((byte)account.Access);
            }
        }

        public static void Character(Account account)
        {
            FileInfo file = new FileInfo(Directories.Accounts.FullName + account.User + "\\Characters\\" + account.Character.Name + Directories.Format);

            // Evita erros
            if (!file.Directory.Exists) file.Directory.Create();

            // Salva os dados do personagem no arquivo
            using (var data = new BinaryWriter(file.OpenWrite()))
            {
                data.Write(account.Character.Name);
                data.Write(account.Character.TextureNum);
                data.Write(account.Character.Level);
                data.Write(account.Character.Class.GetID());
                data.Write(account.Character.Genre);
                data.Write(account.Character.Experience);
                data.Write(account.Character.Points);
                data.Write(account.Character.Map.GetID());
                data.Write(account.Character.X);
                data.Write(account.Character.Y);
                data.Write((byte)account.Character.Direction);
                for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(account.Character.Vital[n]);
                for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(account.Character.Attribute[n]);
                for (byte n = 0; n < MaxInventory; n++)
                {
                    data.Write(account.Character.Inventory[n].Item.GetID());
                    data.Write(account.Character.Inventory[n].Amount);
                }
                for (byte n = 0; n < (byte)Equipment.Count; n++) data.Write(account.Character.Equipment[n].GetID());
                for (byte n = 0; n < MaxHotbar; n++)
                {
                    data.Write((byte)account.Character.Hotbar[n].Type);
                    data.Write(account.Character.Hotbar[n].Slot);
                }
            }
        }

        public static void CharacterName(string name)
        {
            // Salva o nome de um personagem no arquivo
            using (var data = new StreamWriter(Directories.Characters.FullName, true))
                data.Write(";" + name + ":");
        }

        public static void CharactersName(string charactersName)
        {
            // Salva o nome de todos os personagens no arquivo
            using (var data = new StreamWriter(Directories.Characters.FullName))
                data.Write(charactersName);
        }

        public static void Classes()
        {
            // Escreve os dados
            foreach (var @class in CryBits.Entities.Class.List.Values)
                Class(@class);
        }

        public static void Class(Class @class)
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.Classes.FullName + @class.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(stream, @class);
        }

        public static void Npcs()
        {
            // Escreve os dados
            foreach (var Npc in Npc.List.Values)
                using (var stream = new FileInfo(Directories.Npcs.FullName + Npc.ID + Directories.Format).OpenWrite())
                    new BinaryFormatter().Serialize(stream, Npc);
        }

        public static void Items()
        {
            // Escreve os dados
            foreach (var item in Item.List.Values)
                using (var stream = new FileInfo(Directories.Items.FullName + item.ID + Directories.Format).OpenWrite())
                    new BinaryFormatter().Serialize(stream, item);
        }
        public static void Maps()
        {
            // Escreve os dados
            foreach (var map in CryBits.Entities.Map.List.Values)
                Map(map);
        }

        public static void Map(Map map)
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.Maps.FullName + map.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(stream, map);
        }

        public static void Shops()
        {
            // Escreve os dados
            foreach (var shop in Shop.List.Values)
                using (var stream = new FileInfo(Directories.Shops.FullName + shop.ID + Directories.Format).OpenWrite())
                    new BinaryFormatter().Serialize(stream, shop);
        }
    }
}