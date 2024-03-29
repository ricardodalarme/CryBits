﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Library;

internal static class Read
{
    public static void All()
    {
        // Carrega todos os dados
        Console.WriteLine("Loading settings.");
        Defaults();
        Console.WriteLine("Loading maps.");
        Maps();
        Console.WriteLine("Loading classes.");
        Classes();
        Console.WriteLine("Loading npcs.");
        Npcs();
        Console.WriteLine("Loading items.");
        Items();
        Console.WriteLine("Loading shops.");
        Shops();
    }

    private static void Defaults()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Defaults.Exists)
        {
            Write.Defaults();
            return;
        }

        // Carrega as configurações
        using var data = new BinaryReader(Directories.Defaults.OpenRead());
        GameName = data.ReadString();
        WelcomeMessage = data.ReadString();
        Port = data.ReadInt16();
        MaxPlayers = data.ReadByte();
        MaxCharacters = data.ReadByte();
        MaxPartyMembers = data.ReadByte();
        MaxMapItems = data.ReadByte();
        NumPoints = data.ReadByte();
        MaxNameLength = data.ReadByte();
        MinNameLength = data.ReadByte();
        MaxPasswordLength = data.ReadByte();
        MinPasswordLength = data.ReadByte();
    }

    public static void Account(Account account, string name)
    {
        var file = new FileInfo(Directories.Accounts.FullName + name + "\\Data" + Directories.Format);

        // Carrega os dados da conta
        using var data = new BinaryReader(file.OpenRead());
        account.User = data.ReadString();
        account.Password = data.ReadString();
        account.Access = (Access)data.ReadByte();
    }

    public static void Characters(Account account)
    {
        var directory = new DirectoryInfo(Directories.Accounts.FullName + account.User + "\\Characters");

        // Previne erros
        if (!directory.Exists) directory.Create();

        // Lê todos os personagens
        var file = directory.GetFiles();
        account.Characters = new List<Account.TempCharacter>();
        for (byte i = 0; i < file.Length; i++)
            // Cria um arquivo temporário
            using (var data = new BinaryReader(file[i].OpenRead()))
                // Carrega os dados e os adiciona à lista
                account.Characters.Add(new Account.TempCharacter
                {
                    Name = data.ReadString(),
                    TextureNum = data.ReadInt16()
                });
    }

    public static void Character(Account account, string name)
    {
        var file = new FileInfo(Directories.Accounts.FullName + account.User + "\\Characters\\" + name + Directories.Format);

        // Verifica se o diretório existe
        if (!file.Directory.Exists) return;

        // Cria um arquivo temporário
        using var data = new BinaryReader(file.OpenRead());
        // Carrega os dados e os adiciona ao cache
        account.Character = new Player(account);
        account.Character.Name = data.ReadString();
        account.Character.TextureNum = data.ReadInt16();
        account.Character.Level = data.ReadInt16();
        account.Character.Class = Class.List.Get(new Guid(data.ReadString()));
        account.Character.Genre = data.ReadBoolean();
        account.Character.Experience = data.ReadInt32();
        account.Character.Points = data.ReadByte();
        account.Character.Map = TempMap.List.Get(new Guid(data.ReadString()));
        account.Character.X = data.ReadByte();
        account.Character.Y = data.ReadByte();
        account.Character.Direction = (Direction)data.ReadByte();
        for (byte n = 0; n < (byte)Vital.Count; n++) account.Character.Vital[n] = data.ReadInt16();
        for (byte n = 0; n < (byte)Attribute.Count; n++) account.Character.Attribute[n] = data.ReadInt16();
        for (byte n = 0; n < MaxInventory; n++)
        {
            account.Character.Inventory[n].Item = Item.List.Get(new Guid(data.ReadString()));
            account.Character.Inventory[n].Amount = data.ReadInt16();
        }
        for (byte n = 0; n < (byte)Equipment.Count; n++) account.Character.Equipment[n] = Item.List.Get(new Guid(data.ReadString()));
        for (byte n = 0; n < MaxHotbar; n++) account.Character.Hotbar[n] = new HotbarSlot((SlotType)data.ReadByte(), data.ReadByte());
    }

    public static string CharactersName()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Characters.Exists)
        {
            Write.CharactersName(string.Empty);
            return string.Empty;
        }

        // Retorna o nome de todos os personagens registrados
        using var data = new StreamReader(Directories.Characters.FullName);
        return data.ReadToEnd();
    }

    private static void Classes()
    {
        Class.List = new Dictionary<Guid, Class>();
        var file = Directories.Classes.GetFiles();

        // Lê os dados
        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Class.List.Add(new Guid(file[i].Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(stream));
        // Cria uma classe caso não houver nenhuma
        else
        {
            var @class = new Class();
            Class.List.Add(@class.Id, @class);
            Write.Class(@class);
        }
    }

    private static void Items()
    {
        // Lê os dados
        Item.List = new Dictionary<Guid, Item>();
        var file = Directories.Items.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Item.List.Add(new Guid(file[i].Name.Remove(36)), (Item)new BinaryFormatter().Deserialize(stream));
    }

    private static void Maps()
    {
        // Lê os dados
        Map.List = new Dictionary<Guid, Map>();
        var file = Directories.Maps.GetFiles();

        // Lê os dados
        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Map.List.Add(new Guid(file[i].Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(stream));
        // Cria um mapa novo caso não houver nenhuma
        else
        {
            // Cria um mapa novo
            var map = new Map();
            Map.List.Add(map.Id, map);

            // Escreve os dados
            Write.Map(map);
        }
    }

    private static void Npcs()
    {
        // Lê os dados
        Npc.List = new Dictionary<Guid, Npc>();
        var file = Directories.Npcs.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Npc.List.Add(new Guid(file[i].Name.Remove(36)), (Npc)new BinaryFormatter().Deserialize(stream));
    }

    private static void Shops()
    {
        // Lê os dados
        Shop.List = new Dictionary<Guid, Shop>();
        var file = Directories.Shops.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Shop.List.Add(new Guid(file[i].Name.Remove(36)), (Shop)new BinaryFormatter().Deserialize(stream));
    }
}