﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Library;
using Lidgren.Network;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Network;

internal static class Receive
{
    public static void Handle(Account account, NetIncomingMessage data)
    {
        var player = account.Character;

        // Manuseia os dados recebidos 
        switch ((ClientPacket)data.ReadByte())
        {
            case ClientPacket.Connect: Connect(account, data); break;
            case ClientPacket.Latency: Latency(account); break;
            case ClientPacket.Register: Register(account, data); break;
            case ClientPacket.CreateCharacter: CreateCharacter(account, data); break;
            case ClientPacket.CharacterUse: CharacterUse(account, data); break;
            case ClientPacket.CharacterCreate: CharacterCreate(account); break;
            case ClientPacket.CharacterDelete: CharacterDelete(account, data); break;
            case ClientPacket.PlayerDirection: PlayerDirection(player, data); break;
            case ClientPacket.PlayerMove: PlayerMove(player, data); break;
            case ClientPacket.PlayerAttack: PlayerAttack(player); break;
            case ClientPacket.Message: Message(player, data); break;
            case ClientPacket.AddPoint: AddPoint(player, data); break;
            case ClientPacket.CollectItem: CollectItem(player); break;
            case ClientPacket.DropItem: DropItem(player, data); break;
            case ClientPacket.InventoryChange: InventoryChange(player, data); break;
            case ClientPacket.InventoryUse: InventoryUse(player, data); break;
            case ClientPacket.EquipmentRemove: EquipmentRemove(player, data); break;
            case ClientPacket.HotbarAdd: HotbarAdd(player, data); break;
            case ClientPacket.HotbarChange: HotbarChange(player, data); break;
            case ClientPacket.HotbarUse: HotbarUse(player, data); break;
            case ClientPacket.PartyInvite: PartyInvite(player, data); break;
            case ClientPacket.PartyAccept: PartyAccept(player); break;
            case ClientPacket.PartyDecline: PartyDecline(player); break;
            case ClientPacket.PartyLeave: PartyLeave(player); break;
            case ClientPacket.TradeInvite: TradeInvite(player, data); break;
            case ClientPacket.TradeAccept: TradeAccept(player); break;
            case ClientPacket.TradeDecline: TradeDecline(player); break;
            case ClientPacket.TradeLeave: TradeLeave(player); break;
            case ClientPacket.TradeOffer: TradeOffer(player, data); break;
            case ClientPacket.TradeOfferState: TradeOfferState(player, data); break;
            case ClientPacket.ShopBuy: ShopBuy(player, data); break;
            case ClientPacket.ShopSell: ShopSell(player, data); break;
            case ClientPacket.ShopClose: ShopClose(player); break;
            case ClientPacket.WriteSettings: WriteSettings(account, data); break;
            case ClientPacket.WriteClasses: WriteClasses(account, data); break;
            case ClientPacket.WriteMaps: WriteMaps(account, data); break;
            case ClientPacket.WriteNpcs: WriteNpcs(account, data); break;
            case ClientPacket.WriteItems: WriteItems(account, data); break;
            case ClientPacket.WriteShops: WriteShops(account, data); break;
            case ClientPacket.RequestSetting: RequestSetting(account); break;
            case ClientPacket.RequestClasses: RequestClasses(account); break;
            case ClientPacket.RequestMap: RequestMap(account, data); break;
            case ClientPacket.RequestMaps: RequestMaps(account); break;
            case ClientPacket.RequestNpcs: RequestNpcs(account); break;
            case ClientPacket.RequestItems: RequestItems(account); break;
            case ClientPacket.RequestShops: RequestShops(account); break;
        }
    }

    private static void Latency(Account account)
    {
        // Envia o pacote para a contagem da latência
        Send.Latency(account);
    }

    private static void Connect(Account account, NetBuffer data)
    {
        // Lê os dados
        var user = data.ReadString().Trim();
        var password = data.ReadString();
        var editor = data.ReadBoolean();

        // Verifica se está tudo certo
        if (!Directory.Exists(Directories.Accounts.FullName + user))
        {
            Send.Alert(account, "This username isn't registered.");
            return;
        }
        if (Account.List.Find(x => x.User.Equals(user)) != null)
        {
            Send.Alert(account, "Someone already signed in to this account.");
            return;
        }

        // Carrega os dados da conta
        Read.Account(account, user);

        // Verifica se a senha está correta
        if (!account.Password.Equals(password))
        {
            Send.Alert(account, "Password is incorrect.");
            return;
        }

        account.Access = Access.Administrator;

        if (editor)
        {
            // Verifica se o jogador tem permissão para fazer entrar no modo edição
            if (account.Access < Access.Editor)
            {
                Send.Alert(account, "You're not allowed to do this.");
                return;
            }

            // Envia todos os dados
            account.InEditor = true;
            Send.ServerData(account);
            Send.Maps(account);
            Send.Items(account);
            Send.Shops(account);
            Send.Classes(account);
            Send.Npcs(account);
            Send.Connect(account);
        }
        else
        {
            // Carrega os dados do jogador
            Read.Characters(account);

            // Envia os dados das classes e dos personagens ao jogador
            Send.Classes(account);
            Send.Characters(account);

            // Se o jogador não tiver nenhum personagem então abrir o painel de criação de personagem
            if (account.Characters.Count == 0)
            {
                Send.CreateCharacter(account);
                return;
            }

            // Abre a janela de seleção de personagens
            Send.Connect(account);
        }
    }

    private static void Register(Account account, NetBuffer data)
    {
        // Lê os dados
        var user = data.ReadString().Trim();
        var password = data.ReadString();

        // Verifica se está tudo certo
        if (user.Length < MinNameLength || user.Length > MaxNameLength)
        {
            Send.Alert(account, "The username must contain between " + MinNameLength + " and " + MaxNameLength + " characters.");
            return;
        }
        if (password.Length < MinNameLength || password.Length > MaxNameLength)
        {
            Send.Alert(account, "The password must contain between " + MinNameLength + " and " + MaxNameLength + " characters.");
            return;
        }
        if (File.Exists(Directories.Accounts.FullName + user + Directories.Format))
        {
            Send.Alert(account, "There is already someone registered with this name.");
            return;
        }

        // Cria a conta
        account.User = user;
        account.Password = password;

        // Salva a conta
        Write.Account(account);

        // Abre a janela de seleção de personagens
        Send.Classes(account);
        Send.CreateCharacter(account);
    }

    private static void CreateCharacter(Account account, NetBuffer data)
    {
        // Lê os dados
        var name = data.ReadString().Trim();

        // Verifica se está tudo certo
        if (name.Length < MinNameLength || name.Length > MaxNameLength)
        {
            Send.Alert(account, "The character name must contain between " + MinNameLength + " and " + MaxNameLength + " characters.", false);
            return;
        }
        if (name.Contains(';') || name.Contains(':'))
        {
            Send.Alert(account, "Can't contain ';' and ':' in the character name.", false);
            return;
        }
        if (Read.CharactersName().Contains(";" + name + ":"))
        {
            Send.Alert(account, "A character with this name already exists", false);
            return;
        }

        // Define os valores iniciais do personagem
        Class @class;
        account.Character = new Player(account);
        account.Character.Name = name;
        account.Character.Level = 1;
        account.Character.Class = @class = Class.List.Get(new Guid(data.ReadString()));
        account.Character.Genre = data.ReadBoolean();
        account.Character.TextureNum = account.Character.Genre ? @class.TextureMale[data.ReadByte()] : @class.TextureFemale[data.ReadByte()];
        account.Character.Attribute = @class.Attribute;
        account.Character.Map = TempMap.List.Get(@class.SpawnMap.Id);
        account.Character.Direction = (Direction)@class.SpawnDirection;
        account.Character.X = @class.SpawnX;
        account.Character.Y = @class.SpawnY;
        for (byte i = 0; i < (byte)Vital.Count; i++) account.Character.Vital[i] = account.Character.MaxVital(i);
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
            if (@class.Item[i].Item.Type == ItemType.Equipment && account.Character.Equipment[@class.Item[i].Item.EquipType] == null)
                account.Character.Equipment[@class.Item[i].Item.EquipType] = @class.Item[i].Item;
            else
                account.Character.GiveItem(@class.Item[i].Item, @class.Item[i].Amount);
        for (byte i = 0; i < MaxHotbar; i++) account.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        // Salva a conta
        Write.CharacterName(name);
        Write.Character(account);

        // Entra no jogo
        account.Character.Join();
    }

    private static void CharacterUse(Account account, NetBuffer data)
    {
        var character = data.ReadInt32();

        // Verifica se o personagem existe
        if (character < 0 || character >= account.Characters.Count) return;

        // Entra no jogo
        Read.Character(account, account.Characters[character].Name);
        account.Character.Join();
    }

    private static void CharacterCreate(Account account)
    {
        // Verifica se o jogador já criou o máximo de personagens possíveis
        if (account.Characters.Count == MaxCharacters)
        {
            Send.Alert(account, "You can only have " + MaxCharacters + " characters.", false);
            return;
        }

        // Abre a janela de seleção de personagens
        Send.Classes(account);
        Send.CreateCharacter(account);
    }

    private static void CharacterDelete(Account account, NetBuffer data)
    {
        var character = data.ReadInt32();

        // Verifica se o personagem existe
        if (character < 0 || character >= account.Characters.Count) return;

        // Deleta o personagem
        var name = account.Characters[character].Name;
        Send.Alert(account, "The character '" + name + "' has been deleted.", false);
        Write.CharactersName(Read.CharactersName().Replace(":;" + name + ":", ":"));
        account.Characters.RemoveAt(character);
        File.Delete(Directories.Accounts.FullName + account.User + "\\Characters\\" + name + Directories.Format);

        // Salva o personagem
        Send.Characters(account);
        Write.Account(account);
    }

    private static void PlayerDirection(Player player, NetBuffer data)
    {
        var direction = (Direction)data.ReadByte();

        // Previne erros
        if (direction < Direction.Up || direction > Direction.Right) return;
        if (player.GettingMap) return;

        // Defini a direção do jogador
        player.Direction = direction;
        Send.PlayerDirection(player);
    }

    private static void PlayerMove(Player player, NetBuffer data)
    {
        // Move o jogador se necessário
        if (player.X != data.ReadByte() || player.Y != data.ReadByte())
            Send.PlayerPosition(player);
        else
            player.Move(data.ReadByte());
    }

    private static void Message(Player player, NetBuffer data)
    {
        var message = data.ReadString();

        // Evita caracteres inválidos
        for (byte i = 0; i >= message.Length; i++)
            if (message[i] < 32 && message[i] > 126)
                return;

        // Envia a mensagem para os outros jogadores
        switch ((Message)data.ReadByte())
        {
            case Enums.Message.Map: Send.MessageMap(player, message); break;
            case Enums.Message.Global: Send.MessageGlobal(player, message); break;
            case Enums.Message.Private: Send.MessagePrivate(player, data.ReadString(), message); break;
        }
    }

    private static void PlayerAttack(Player player)
    {
        // Ataca
        player.Attack();
    }

    private static void AddPoint(Player player, NetBuffer data)
    {
        var attributeNum = data.ReadByte();

        // Adiciona um ponto a determinado atributo
        if (player.Points > 0)
        {
            player.Attribute[attributeNum]++;
            player.Points--;
            Send.PlayerExperience(player);
            Send.MapPlayers(player);
        }
    }

    private static void CollectItem(Player player)
    {
        var mapItem = player.Map.HasItem(player.X, player.Y);

        // Somente se necessário
        if (mapItem == null) return;

        // Dá o item ao jogador
        if (player.GiveItem(mapItem.Item, mapItem.Amount))
        {
            // Retira o item do mapa
            player.Map.Item.Remove(mapItem);
            Send.MapItems(player.Map);
        }
    }

    private static void DropItem(Player player, NetBuffer data)
    {
        player.DropItem(data.ReadInt16(), data.ReadInt16());
    }

    private static void InventoryChange(Player player, NetBuffer data)
    {
        short slotOld = data.ReadInt16(), slotNew = data.ReadInt16();

        // Somente se necessário
        if (player.Inventory[slotOld].Item == null) return;
        if (slotOld == slotNew) return;
        if (player.Trade != null) return;

        // Muda o item de slot
        (player.Inventory[slotOld], player.Inventory[slotNew]) = (player.Inventory[slotNew], player.Inventory[slotOld]);
        Send.PlayerInventory(player);

        // Altera na hotbar
        var hotbarSlot = player.FindHotbar(SlotType.Item, player.Inventory[slotOld]);
        if (hotbarSlot != null)
        {
            hotbarSlot.Slot = slotNew;
            Send.PlayerHotbar(player);
        }
    }

    private static void InventoryUse(Player player, NetBuffer data)
    {
        player.UseItem(player.Inventory[data.ReadByte()]);
    }

    private static void EquipmentRemove(Player player, NetBuffer data)
    {
        var slot = data.ReadByte();

        // Apenas se necessário
        if (player.Equipment[slot] == null) return;
        if (player.Equipment[slot].Bind == BindOn.Equip) return;

        // Adiciona o equipamento ao inventário
        if (!player.GiveItem(player.Equipment[slot], 1))
        {
            // Somente se necessário
            if (player.Map.Item.Count == MaxMapItems) return;

            // Solta o item no chão
            player.Map.Item.Add(new TempMapItems(player.Equipment[slot], 1, player.X, player.Y));

            // Envia os dados
            Send.MapItems(player.Map);
            Send.PlayerInventory(player);
        }

        // Remove o equipamento
        for (byte i = 0; i < (byte)Attribute.Count; i++) player.Attribute[i] -= player.Equipment[slot].EquipAttribute[i];
        player.Equipment[slot] = null;

        // Envia os dados
        Send.PlayerEquipments(player);
    }

    private static void HotbarAdd(Player player, NetBuffer data)
    {
        var hotbarSlot = data.ReadInt16();
        var type = (SlotType)data.ReadByte();
        var slot = data.ReadInt16();

        // Somente se necessário
        if (slot != 0 && player.FindHotbar(type, slot) != null) return;

        // Define os dados
        player.Hotbar[hotbarSlot].Slot = slot;
        player.Hotbar[hotbarSlot].Type = type;

        // Envia os dados
        Send.PlayerHotbar(player);
    }

    private static void HotbarChange(Player player, NetBuffer data)
    {
        short slotOld = data.ReadInt16(), slotNew = data.ReadInt16();

        // Somente se necessário
        if (slotOld < 0 || slotNew < 0) return;
        if (slotOld == slotNew) return;
        if (player.Hotbar[slotOld].Slot == 0) return;

        // Muda o item de slot
        (player.Hotbar[slotOld], player.Hotbar[slotNew]) = (player.Hotbar[slotNew], player.Hotbar[slotOld]);
        Send.PlayerHotbar(player);
    }

    private static void HotbarUse(Player player, NetBuffer data)
    {
        var hotbarSlot = data.ReadInt16();

        // Usa o item
        switch (player.Hotbar[hotbarSlot].Type)
        {
            case SlotType.Item: player.UseItem(player.Hotbar[hotbarSlot].Slot); break;
        }
    }

    private static void WriteSettings(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Altera os dados
        GameName = data.ReadString();
        WelcomeMessage = data.ReadString();
        Port = data.ReadInt16();
        MaxPlayers = data.ReadByte();
        MaxCharacters = data.ReadByte();
        MaxPartyMembers = data.ReadByte();
        MaxMapItems = data.ReadByte();
        NumPoints = data.ReadByte();
        MinNameLength = data.ReadByte();
        MaxNameLength = data.ReadByte();
        MinPasswordLength = data.ReadByte();
        MaxPasswordLength = data.ReadByte();

        // Salva os dados
        Write.Defaults();
    }

    private static void WriteClasses(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
        Write.Classes();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                Send.Classes(Account.List[i]);
    }

    private static void WriteMaps(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Map.List = (Dictionary<Guid, Map>)data.ReadObject();
        Write.Maps();

        // Envia os novos dados para todos jogadores 
        foreach (var tempMap in TempMap.List.Values)
        {
            // Itens do mapa
            tempMap.SpawnItems();

            // Envia o mapa para todos os jogadores que estão nele
            for (byte n = 0; n < Account.List.Count; n++)
                if (Account.List[n] != account)
                    if (Account.List[n].Character.Map == tempMap || Account.List[n].InEditor)
                        Send.Map(Account.List[n], tempMap.Data);
        }
    }

    private static void WriteNpcs(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
        Write.Npcs();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                Send.Npcs(Account.List[i]);
    }

    private static void WriteItems(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
        Write.Items();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                Send.Items(Account.List[i]);
    }

    private static void WriteShops(Account account, NetBuffer data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (account.Access < Access.Editor)
        {
            Send.Alert(account, "You aren't allowed to do this.");
            return;
        }

        // Recebe e salva os novos dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
        Write.Shops();

        // Envia os novos dados para todos jogadores conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i] != account)
                Send.Shops(Account.List[i]);
    }

    private static void RequestSetting(Account account)
    {
        Send.ServerData(account);
    }

    private static void RequestClasses(Account account)
    {
        Send.Classes(account);
    }

    private static void RequestMap(Account account, NetBuffer data)
    {
        if (account.InEditor)
            Send.Map(account, Map.List.Get(new Guid(data.ReadString())));
        else
        {
            var player = account.Character;

            // Se necessário enviar as informações do mapa ao jogador
            if (data.ReadBoolean()) Send.Map(player.Account, player.Map.Data);

            // Envia a informação aos outros jogadores
            Send.MapPlayers(player);

            // Entra no mapa
            player.GettingMap = false;
            Send.JoinMap(player);
        }
    }

    private static void RequestMaps(Account account)
    {
        Send.Maps(account);
    }

    private static void RequestNpcs(Account account)
    {
        Send.Npcs(account);
    }

    private static void RequestItems(Account account)
    {
        Send.Items(account);
    }

    private static void RequestShops(Account account)
    {
        Send.Shops(account);
    }

    private static void PartyInvite(Player player, NetBuffer data)
    {
        var name = data.ReadString();

        // Encontra o jogador
        var invited = Player.Find(name);

        // Verifica se o jogador está convectado
        if (invited == null)
        {
            Send.Message(player, "The player isn't connected.", Color.White);
            return;
        }
        // Verifica se não está tentando se convidar
        if (invited == player)
        {
            Send.Message(player, "You can't be invited.", Color.White);
            return;
        }
        // Verifica se já tem um grupo
        if (invited.Party.Count != 0)
        {
            Send.Message(player, "The player is already part of a party.", Color.White);
            return;
        }
        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(invited.PartyRequest))
        {
            Send.Message(player, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }
        // Verifica se o grupo está cheio
        if (player.Party.Count == MaxPartyMembers - 1)
        {
            Send.Message(player, "Your party is full.", Color.White);
            return;
        }

        // Convida o jogador
        invited.PartyRequest = player.Name;
        Send.PartyInvitation(invited, player.Name);
    }

    private static void PartyAccept(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);

        // Verifica se já tem um grupo
        if (player.Party.Count != 0)
        {
            Send.Message(player, "You are already part of a party.", Color.White);
            return;
        }

        // Verifica se quem chamou ainda está disponível
        if (invitation == null)
        {
            Send.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }
        // Verifica se o grupo está cheio
        if (invitation.Party.Count == MaxPartyMembers - 1)
        {
            Send.Message(player, "The party is full.", Color.White);
            return;
        }

        // Entra na festa
        for (byte i = 0; i < invitation.Party.Count; i++)
        {
            invitation.Party[i].Party.Add(player);
            player.Party.Add(invitation.Party[i]);
        }
        player.Party.Insert(0, invitation);
        invitation.Party.Add(player);
        player.PartyRequest = string.Empty;
        Send.Message(invitation, player.Name + " joined the party.", Color.White);

        // Envia os dados para o grupo
        Send.Party(player);
        for (byte i = 0; i < player.Party.Count; i++) Send.Party(player.Party[i]);
    }

    private static void PartyDecline(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);

        // Recusa o convite
        if (invitation != null) Send.Message(invitation, player.Name + " decline the party.", Color.White);
        player.PartyRequest = string.Empty;
    }

    private static void PartyLeave(Player player)
    {
        // Sai do grupo
        player.PartyLeave();
    }

    private static void TradeInvite(Player player, NetBuffer data)
    {
        var name = data.ReadString();

        // Encontra o jogador
        var invited = Player.Find(name);

        // Verifica se o jogador está convectado
        if (invited == null)
        {
            Send.Message(player, "The player isn't connected.", Color.White);
            return;
        }
        // Verifica se não está tentando se convidar
        if (invited == player)
        {
            Send.Message(player, "You can't be invited.", Color.White);
            return;
        }
        // Verifica se já tem um grupo
        if (invited.Trade != null)
        {
            Send.Message(player, "The player is already part of a trade.", Color.White);
            return;
        }
        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(invited.TradeRequest))
        {
            Send.Message(player, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }
        // Verifica se os jogadores não estão em com a loja aberta
        if (player.Shop != null)
        {
            Send.Message(player, "You can't start a trade while in the shop.", Color.White);
            return;
        }
        if (invited.Shop != null)
        {
            Send.Message(player, "The player is in the shop.", Color.White);
            return;
        }
        // Verifica se os jogadores estão pertods um do outro
        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            Send.Message(player, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        // Convida o jogador
        invited.TradeRequest = player.Name;
        Send.TradeInvitation(invited, player.Name);
    }

    private static void TradeAccept(Player player)
    {
        var invited = Player.Find(player.TradeRequest);

        // Verifica se já tem um grupo
        if (player.Trade != null)
        {
            Send.Message(player, "You are already part of a trade.", Color.White);
            return;
        }
        // Verifica se quem chamou ainda está disponível
        if (invited == null)
        {
            Send.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }
        // Verifica se os jogadores estão pertods um do outro
        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            Send.Message(player, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }
        // Verifica se  os jogadores não estão em com a loja aberta
        if (invited.Shop != null)
        {
            Send.Message(player, "Who invited you is in the shop.", Color.White);
            return;
        }

        // Entra na troca
        player.Trade = invited;
        invited.Trade = player;
        Send.Message(player, "You have accepted " + invited.Name + "'s trade request.", Color.White);
        Send.Message(invited, player.Name + " has accepted your trade request.", Color.White);

        // Limpa os dadoss
        player.TradeRequest = string.Empty;
        player.TradeOffer = new TradeSlot[MaxInventory];
        invited.TradeOffer = new TradeSlot[MaxInventory];

        // Envia os dados para o grupo
        Send.Trade(player, true);
        Send.Trade(invited, true);
    }

    private static void TradeDecline(Player player)
    {
        var invited = Player.Find(player.TradeRequest);

        // Recusa o convite
        if (invited != null) Send.Message(invited, player.Name + " decline the trade.", Color.White);
        player.TradeRequest = string.Empty;
    }

    private static void TradeLeave(Player player)
    {
        player.TradeLeave();
    }

    private static void TradeOffer(Player player, NetBuffer data)
    {
        short slot = data.ReadInt16(), inventorySlot = data.ReadInt16();
        var amount = Math.Min(data.ReadInt16(), player.Inventory[inventorySlot].Amount);

        // Adiciona o item à troca
        if (inventorySlot != 0)
        {
            // Evita itens repetidos
            for (byte i = 0; i < MaxInventory; i++)
                if (player.TradeOffer[i].SlotNum == inventorySlot)
                    return;

            player.TradeOffer[slot].SlotNum = inventorySlot;
            player.TradeOffer[slot].Amount = amount;
        }
        // Remove o item da troca
        else
            player.TradeOffer[slot] = new TradeSlot();

        // Envia os dados ao outro jogador
        Send.TradeOffer(player);
        Send.TradeOffer(player.Trade, false);
    }

    private static void TradeOfferState(Player player, NetBuffer data)
    {
        var state = (TradeStatus)data.ReadByte();
        var invited = player.Trade;

        switch (state)
        {
            case TradeStatus.Accepted:
                // Verifica se os jogadores têm espaço disponivel para trocar os itens
                if (player.TotalTradeItems > invited.TotalInventoryFree)
                {
                    Send.Message(invited, invited.Name + " don't have enough space in their inventory to do this trade.", Color.Red);
                    break;
                }
                if (invited.TotalTradeItems > player.TotalInventoryFree)
                {
                    Send.Message(invited, "You don't have enough space in your inventory to do this trade.", Color.Red);
                    break;
                }

                // Mensagem de confirmação
                Send.Message(invited, "The offer was accepted.", Color.Green);

                // Dados da oferta
                ItemSlot[] yourInventory = (ItemSlot[])player.Inventory.Clone(),
                    theirInventory = (ItemSlot[])invited.Inventory.Clone();

                // Remove os itens do inventário dos jogadores
                var to = player;
                for (byte j = 0; j < 2; j++, to = to == player ? invited : player)
                for (byte i = 0; i < MaxInventory; i++)
                    to.TakeItem(to.Inventory[to.TradeOffer[i].SlotNum], to.TradeOffer[i].Amount);

                // Dá os itens aos jogadores
                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (player.TradeOffer[i].SlotNum > 0) invited.GiveItem(yourInventory[player.TradeOffer[i].SlotNum].Item, player.TradeOffer[i].Amount);
                    if (invited.TradeOffer[i].SlotNum > 0) player.GiveItem(theirInventory[invited.TradeOffer[i].SlotNum].Item, invited.TradeOffer[i].Amount);
                }

                // Envia os dados do inventário aos jogadores
                Send.PlayerInventory(player);
                Send.PlayerInventory(invited);

                // Limpa a troca
                player.TradeOffer = new TradeSlot[MaxInventory];
                invited.TradeOffer = new TradeSlot[MaxInventory];
                Send.TradeOffer(invited);
                Send.TradeOffer(invited, false);
                break;
            case TradeStatus.Declined:
                Send.Message(invited, "The offer was declined.", Color.Red);
                break;
            case TradeStatus.Waiting:
                Send.Message(invited, player.Name + " send you a offer.", Color.White);
                break;
        }

        // Envia os dados
        Send.TradeState(invited, state);
    }

    private static void ShopBuy(Player player, NetBuffer data)
    {
        var shopSold = player.Shop.Sold[data.ReadInt16()];
        var inventorySlot = player.FindInventory(player.Shop.Currency);

        // Verifica se o jogador tem dinheiro
        if (inventorySlot == null || inventorySlot.Amount < shopSold.Price)
        {
            Send.Message(player, "You don't have enough money to buy the item.", Color.Red);
            return;
        }
        // Verifica se há espaço no inventário
        if (player.TotalInventoryFree == 0 && inventorySlot.Amount > shopSold.Price)
        {
            Send.Message(player, "You  don't have space in your bag.", Color.Red);
            return;
        }

        // Realiza a compra do item
        player.TakeItem(inventorySlot, shopSold.Price);
        player.GiveItem(shopSold.Item, shopSold.Amount);
        Send.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", Color.Green);
    }

    private static void ShopSell(Player player, NetBuffer data)
    {
        var inventorySlot = data.ReadByte();
        var amount = Math.Min(data.ReadInt16(), player.Inventory[inventorySlot].Amount);
        var buy = player.Shop.FindBought(player.Inventory[inventorySlot].Item);

        // Verifica se a loja vende o item
        if (buy == null)
        {
            Send.Message(player, "The store doesn't sell this item", Color.Red);
            return;
        }
        // Verifica se há espaço no inventário
        if (player.TotalInventoryFree == 0 && player.Inventory[inventorySlot].Amount > amount)
        {
            Send.Message(player, "You don't have space in your bag.", Color.Red);
            return;
        }

        // Realiza a venda do item
        Send.Message(player, "You sold " + player.Inventory[inventorySlot].Item.Name + "x " + amount + "for .", Color.Green);
        player.TakeItem(player.Inventory[inventorySlot], amount);
        player.GiveItem(player.Shop.Currency, (short)(buy.Price * amount));
    }

    private static void ShopClose(Player player)
    {
        player.Shop = null;
    }
}