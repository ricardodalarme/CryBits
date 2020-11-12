using CryBits.Entities;
using CryBits.Server.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using static CryBits.Server.Logic.Utils;

namespace CryBits.Server.Library
{
    static class Read
    {
        public static void All()
        {
            // Carrega todos os dados
            Console.WriteLine("Loading settings.");
            Settings();
            Console.WriteLine("Loading maps.");
            Maps();
            Console.WriteLine("Loading classes.");
            Classes();
            Console.WriteLine("Loading NPCs.");
            NPCs();
            Console.WriteLine("Loading items.");
            Items();
            Console.WriteLine("Loading shops.");
            Shops();
        }

        private static void Settings()
        {
            // Cria o arquivo caso ele não existir
            if (!Directories.Settings.Exists)
            {
                Write.Settings();
                return;
            }

            // Carrega as configurações
            using (var data = new BinaryReader(Directories.Settings.OpenRead()))
            {
                Game_Name = data.ReadString();
                Welcome_Message = data.ReadString();
                Port = data.ReadInt16();
                Max_Players = data.ReadByte();
                Max_Characters = data.ReadByte();
                Max_Party_Members = data.ReadByte();
                Max_Map_Items = data.ReadByte();
                Num_Points = data.ReadByte();
                Max_Name_Length = data.ReadByte();
                Min_Name_Length = data.ReadByte();
                Max_Password_Length = data.ReadByte();
                Min_Password_Length = data.ReadByte();
            }
        }

        public static void Account(Account account, string name)
        {
            var file = new FileInfo(Directories.Accounts.FullName + name + "\\Data" + Directories.Format);

            // Carrega os dados da conta
            using (var data = new BinaryReader(file.OpenRead()))
            {
                account.User = data.ReadString();
                account.Password = data.ReadString();
                account.Access = (Accesses)data.ReadByte();
            }
        }

        public static void Characters(Account account)
        {
            DirectoryInfo directory = new DirectoryInfo(Directories.Accounts.FullName + account.User + "\\Characters");

            // Previne erros
            if (!directory.Exists) directory.Create();

            // Lê todos os personagens
            FileInfo[] file = directory.GetFiles();
            account.Characters = new List<Account.TempCharacter>();
            for (byte i = 0; i < file.Length; i++)
                // Cria um arquivo temporário
                using (var data = new BinaryReader(file[i].OpenRead()))
                    // Carrega os dados e os adiciona à lista
                    account.Characters.Add(new Account.TempCharacter
                    {
                        Name = data.ReadString(),
                        Texture_Num = data.ReadInt16(),
                        Level = data.ReadInt16()
                    });
        }

        public static void Character(Account account, string name)
        {
            FileInfo file = new FileInfo(Directories.Accounts.FullName + account.User + "\\Characters\\" + name + Directories.Format);

            // Verifica se o diretório existe
            if (!file.Directory.Exists) return;

            // Cria um arquivo temporário
            using (var data = new BinaryReader(file.OpenRead()))
            {
                // Carrega os dados e os adiciona ao cache
                account.Character = new Player(account);
                account.Character.Name = data.ReadString();
                account.Character.Texture_Num = data.ReadInt16();
                account.Character.Level = data.ReadInt16();
                account.Character.Class = Class.Get(new Guid(data.ReadString()));
                account.Character.Genre = data.ReadBoolean();
                account.Character.Experience = data.ReadInt32();
                account.Character.Points = data.ReadByte();
                account.Character.Map = TempMap.Get(new Guid(data.ReadString()));
                account.Character.X = data.ReadByte();
                account.Character.Y = data.ReadByte();
                account.Character.Direction = (Directions)data.ReadByte();
                for (byte n = 0; n < (byte)Vitals.Count; n++) account.Character.Vital[n] = data.ReadInt16();
                for (byte n = 0; n < (byte)Attributes.Count; n++) account.Character.Attribute[n] = data.ReadInt16();
                for (byte n = 1; n <= MaxInventory; n++)
                {
                    account.Character.Inventory[n].Item = Item.Get(new Guid(data.ReadString()));
                    account.Character.Inventory[n].Amount = data.ReadInt16();
                }
                for (byte n = 0; n < (byte)Equipments.Count; n++) account.Character.Equipment[n] = Item.Get(new Guid(data.ReadString()));
                for (byte n = 0; n < MaxHotbar; n++) account.Character.Hotbar[n] = new Hotbar((Hotbars)data.ReadByte(), data.ReadByte());
            }
        }

        public static string Characters_Name()
        {
            // Cria o arquivo caso ele não existir
            if (!Directories.Characters.Exists)
            {
                Write.Characters_Name(string.Empty);
                return string.Empty;
            }

            // Retorna o nome de todos os personagens registrados
            using (var data = new StreamReader(Directories.Characters.FullName))
                return data.ReadToEnd();
        }

        private static void Classes()
        {
            Class.List = new Dictionary<Guid, Class>();
            FileInfo[] file = Directories.Classes.GetFiles();

            // Lê os dados
            if (file.Length > 0)
                for (byte i = 0; i < file.Length; i++)
                    using (var stream = file[i].OpenRead())
                        Class.List.Add(new Guid(file[i].Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(stream));
            // Cria uma classe caso não houver nenhuma
            else
            {
                Class @class = new Class(Guid.NewGuid());
                @class.Name = "New class";
                @class.Spawn_Map = Map.List.ElementAt(0).Value;
                Class.List.Add(@class.ID, @class);
                Write.Class(@class);
            }
        }

        private static void Items()
        {
            // Lê os dados
            Item.List = new Dictionary<Guid, Item>();
            FileInfo[] file = Directories.Items.GetFiles();
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Item.List.Add(new Guid(file[i].Name.Remove(36)), (Item)new BinaryFormatter().Deserialize(stream));
        }

        private static void Maps()
        {
            // Lê os dados
            Map.List = new Dictionary<Guid, Map>();
            FileInfo[] file = Directories.Maps.GetFiles();

            // Lê os dados
            if (file.Length > 0)
                for (byte i = 0; i < file.Length; i++)
                    using (var stream = file[i].OpenRead())
                        Map.List.Add(new Guid(file[i].Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(stream));
            // Cria um mapa novo caso não houver nenhuma
            else
            {
                // Cria um mapa novo
                Map map = new Map(Guid.NewGuid());
                Map.List.Add(map.ID, map);

                // Dados do mapa
                map.Name = "New map";
                map.Layer.Add(new MapLayer
                {
                    Name = "Ground"
                });

                // Escreve os dados
                Write.Map(map);
            }
        }

        private static void NPCs()
        {
            // Lê os dados
            NPC.List = new Dictionary<Guid, NPC>();
            FileInfo[] file = Directories.NPCs.GetFiles();
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    NPC.List.Add(new Guid(file[i].Name.Remove(36)), (NPC)new BinaryFormatter().Deserialize(stream));
        }

        private static void Shops()
        {
            // Lê os dados
            Shop.List = new Dictionary<Guid, Shop>();
            FileInfo[] file = Directories.Shops.GetFiles();
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Shop.List.Add(new Guid(file[i].Name.Remove(36)), (Shop)new BinaryFormatter().Deserialize(stream));
        }
    }
}