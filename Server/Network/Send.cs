using System.Drawing;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Server.Entities;
using Lidgren.Network;
using static CryBits.Defaults;
using static CryBits.Utils;

namespace CryBits.Server.Network
{
    internal static class Send
    {
        private static void ToPlayer(Account account, NetOutgoingMessage data)
        {
            // Recria o pacote e o envia
            NetOutgoingMessage dataSend = Socket.Device.CreateMessage(data.LengthBytes);
            dataSend.Write(data);
            Socket.Device.SendMessage(dataSend, account.Connection, NetDeliveryMethod.ReliableOrdered);
        }

        private static void ToPlayer(Player player, NetOutgoingMessage data)
        {
            ToPlayer(player.Account, data);
        }

        private static void ToAll(NetOutgoingMessage data)
        {
            // Envia os dados para todos conectados
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    ToPlayer(Account.List[i].Character, data);
        }

        private static void ToAllBut(Player player, NetOutgoingMessage data)
        {
            // Envia os dados para todos conectados, com excessão do índice
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    if (player != Account.List[i].Character)
                        ToPlayer(Account.List[i].Character, data);
        }

        private static void ToMap(TempMap map, NetOutgoingMessage data)
        {
            // Envia os dados para todos conectados, com excessão do índice
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    if (Account.List[i].Character.Map == map)
                        ToPlayer(Account.List[i].Character, data);
        }

        private static void ToMapBut(TempMap map, Player player, NetOutgoingMessage data)
        {
            // Envia os dados para todos conectados, com excessão do índice
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    if (Account.List[i].Character.Map == map)
                        if (player != Account.List[i].Character)
                            ToPlayer(Account.List[i].Character, data);
        }

        public static void Alert(Account account, string message, bool disconnect = true)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Alert);
            data.Write(message);
            ToPlayer(account, data);

            // Desconecta o jogador
            if (disconnect) account.Connection.Disconnect(string.Empty);
        }

        public static void Connect(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Connect);
            ToPlayer(account, data);
        }

        public static void CreateCharacter(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.CreateCharacter);
            ToPlayer(account, data);
        }

        public static void Join(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Join);
            data.Write(player.Name);
            ToPlayer(player, data);
        }

        public static void Characters(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Characters);
            data.Write((byte)account.Characters.Count);

            for (byte i = 0; i < account.Characters.Count; i++)
            {
                data.Write(account.Characters[i].Name);
                data.Write(account.Characters[i].TextureNum);
            }

            ToPlayer(account, data);
        }

        public static void Classes(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
             data.Write((byte)ServerPackets.Classes);
            ObjectToByteArray(data, Class.List);
            ToPlayer(account, data);
        }

        public static void JoinGame(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.JoinGame);
            ToPlayer(player, data);
        }

        private static NetOutgoingMessage PlayerDataCache(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Escreve os dados
            data.Write((byte)ServerPackets.PlayerData);
            data.Write(player.Name);
            data.Write(player.TextureNum);
            data.Write(player.Level);
            data.Write(player.Map.GetID());
            data.Write(player.X);
            data.Write(player.Y);
            data.Write((byte)player.Direction);
            for (byte n = 0; n < (byte)Vitals.Count; n++)
            {
                data.Write(player.Vital[n]);
                data.Write(player.MaxVital(n));
            }
            for (byte n = 0; n < (byte)Attributes.Count; n++) data.Write(player.Attribute[n]);
            for (byte n = 0; n < (byte)Equipments.Count; n++) data.Write(player.Equipment[n].GetID());

            return data;
        }

        public static void PlayerPosition(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerPosition);
            data.Write(player.Name);
            data.Write(player.X);
            data.Write(player.Y);
            data.Write((byte)player.Direction);
            ToMap(player.Map, data);
        }

        public static void PlayerVitals(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerVitals);
            data.Write(player.Name);
            for (byte i = 0; i < (byte)Vitals.Count; i++)
            {
                data.Write(player.Vital[i]);
                data.Write(player.MaxVital(i));
            }

            ToMap(player.Map, data);
        }

        public static void PlayerLeave(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerLeave);
            data.Write(player.Name);
            ToAllBut(player, data);
        }

        public static void PlayerMove(Player player, byte movement)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerMove);
            data.Write(player.Name);
            data.Write(player.X);
            data.Write(player.Y);
            data.Write((byte)player.Direction);
            data.Write(movement);
            ToMapBut(player.Map, player, data);
        }

        public static void PlayerDirection(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerDirection);
            data.Write(player.Name);
            data.Write((byte)player.Direction);
            ToMapBut(player.Map, player, data);
        }

        public static void PlayerExperience(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerExperience);
            data.Write(player.Experience);
            data.Write(player.ExpNeeded);
            data.Write(player.Points);
            ToPlayer(player, data);
        }

        public static void PlayerEquipments(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerEquipments);
            data.Write(player.Name);
            for (byte i = 0; i < (byte)Equipments.Count; i++) data.Write(player.Equipment[i].GetID());
            ToMap(player.Map, data);
        }

        public static void MapPlayers(Player player)
        {
            // Envia os dados dos outros jogadores 
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    if (player != Account.List[i].Character)
                        if (Account.List[i].Character.Map == player.Map)
                            ToPlayer(player, PlayerDataCache(Account.List[i].Character));

            // Envia os dados do jogador
            ToMap(player.Map, PlayerDataCache(player));
        }

        public static void JoinMap(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.JoinMap);
            ToPlayer(player, data);
        }

        public static void PlayerLeaveMap(Player player, TempMap map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerLeave);
            data.Write(player.Name);
            ToMapBut(map, player, data);
        }

        public static void MapRevision(Player player, Map map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapRevision);
            data.Write(map.GetID());
            data.Write(map.Revision);
            ToPlayer(player, data);
        }

        public static void Maps(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Maps);
            data.Write((short)CryBits.Entities.Map.List.Count);
            ToPlayer(account, data);
            foreach (Map map in CryBits.Entities.Map.List.Values) Map(account, map);
        }

        public static void Map(Account account, Map map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Map);
            ObjectToByteArray(data, map);
            ToPlayer(account, data);
        }

        public static void Latency(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Latency);
            ToPlayer(account, data);
        }

        public static void Message(Player player, string text, Color color)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Message);
            data.Write(text);
            data.Write(color.ToArgb());
            ToPlayer(player, data);
        }

        public static void MessageMap(Player player, string text)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();
            string message = "[Map] " + player.Name + ": " + text;

            // Envia os dados
            data.Write((byte)ServerPackets.Message);
            data.Write(message);
            data.Write(Color.White.ToArgb());
            ToMap(player.Map, data);
        }

        public static void MessageGlobal(Player player, string text)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();
            string message = "[Global] " + player.Name + ": " + text;

            // Envia os dados
            data.Write((byte)ServerPackets.Message);
            data.Write(message);
            data.Write(Color.Yellow.ToArgb());
            ToAll(data);
        }

        public static void MessagePrivate(Player player, string addresseeName, string text)
        {
            Player addressee = Player.Find(addresseeName);

            // Verifica se o jogador está conectado
            if (addressee == null)
            {
                Message(player, addresseeName + " is currently offline.", Color.Blue);
                return;
            }

            // Envia as mensagens
            Message(player, "[To] " + addresseeName + ": " + text, Color.Pink);
            Message(addressee, "[From] " + player.Name + ": " + text, Color.Pink);
        }

        public static void PlayerAttack(Player player, string victim = "", Targets victimType = 0)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerAttack);
            data.Write(player.Name);
            data.Write(victim);
            data.Write((byte)victimType);
            ToMap(player.Map, data);
        }

        public static void Items(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Items);
            ObjectToByteArray(data, Item.List);
            ToPlayer(account, data);
        }

        public static void MapItems(Player player, TempMap map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapItems);
            data.Write((byte)map.Item.Count);

            for (byte i = 0; i < map.Item.Count; i++)
            {
                // Geral
                data.Write(map.Item[i].Item.GetID());
                data.Write(map.Item[i].X);
                data.Write(map.Item[i].Y);
            }

            // Envia os dados
            ToPlayer(player, data);
        }

        public static void MapItems(TempMap map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapItems);
            data.Write((byte)map.Item.Count);
            for (byte i = 0; i < map.Item.Count; i++)
            {
                data.Write(map.Item[i].Item.GetID());
                data.Write(map.Item[i].X);
                data.Write(map.Item[i].Y);
            }
            ToMap(map, data);
        }

        public static void PlayerInventory(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerInventory);
            for (byte i = 0; i < MaxInventory; i++)
            {
                data.Write(player.Inventory[i].Item.GetID());
                data.Write(player.Inventory[i].Amount);
            }
            ToPlayer(player, data);
        }

        public static void PlayerHotbar(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PlayerHotbar);
            for (byte i = 0; i < MaxHotbar; i++)
            {
                data.Write((byte)player.Hotbar[i].Type);
                data.Write(player.Hotbar[i].Slot);
            }
            ToPlayer(player, data);
        }

        public static void Npcs(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Npcs);
            ObjectToByteArray(data, Npc.List);
            ToPlayer(account, data);
        }

        public static void MapNpcs(Player player, TempMap map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcs);
            data.Write((short)map.Npc.Length);
            for (byte i = 0; i < map.Npc.Length; i++)
            {
                data.Write(map.Npc[i].Data.GetID());
                data.Write(map.Npc[i].X);
                data.Write(map.Npc[i].Y);
                data.Write((byte)map.Npc[i].Direction);
                for (byte n = 0; n < (byte)Vitals.Count; n++) data.Write(map.Npc[i].Vital[n]);
            }
            ToPlayer(player, data);
        }

        public static void MapNpc(TempNpc Npc)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpc);
            data.Write(Npc.Index);
            data.Write(Npc.Data.GetID());
            data.Write(Npc.X);
            data.Write(Npc.Y);
            data.Write((byte)Npc.Direction);
            for (byte n = 0; n < (byte)Vitals.Count; n++) data.Write(Npc.Vital[n]);
            ToMap(Npc.Map, data);
        }

        public static void MapNpcMovement(TempNpc Npc, byte movement)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcMovement);
            data.Write(Npc.Index);
            data.Write(Npc.X);
            data.Write(Npc.Y);
            data.Write((byte)Npc.Direction);
            data.Write(movement);
            ToMap(Npc.Map, data);
        }

        public static void MapNpcDirection(TempNpc Npc)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcDirection);
            data.Write(Npc.Index);
            data.Write((byte)Npc.Direction);
            ToMap(Npc.Map, data);
        }

        public static void MapNpcVitals(TempNpc Npc)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcVitals);
            data.Write(Npc.Index);
            for (byte n = 0; n < (byte)Vitals.Count; n++) data.Write(Npc.Vital[n]);
            ToMap(Npc.Map, data);
        }

        public static void MapNpcAttack(TempNpc Npc, string victim = "", Targets victimType = 0)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcAttack);
            data.Write(Npc.Index);
            data.Write(victim);
            data.Write((byte)victimType);
            ToMap(Npc.Map, data);
        }

        public static void MapNpcDied(TempNpc Npc)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.MapNpcDied);
            data.Write(Npc.Index);
            ToMap(Npc.Map, data);
        }

        public static void ServerData(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.ServerData);
            data.Write(GameName);
            data.Write(WelcomeMessage);
            data.Write(Port);
            data.Write(MaxPlayers);
            data.Write(MaxCharacters);
            data.Write(MaxPartyMembers);
            data.Write(MaxMapItems);
            data.Write(NumPoints);
            data.Write(MinNameLength);
            data.Write(MaxNameLength);
            data.Write(MinPasswordLength);
            data.Write(MaxPasswordLength);
            ToPlayer(account, data);
        }

        public static void Party(Player player)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Party);
            data.Write((byte)player.Party.Count);
            for (byte i = 0; i < player.Party.Count; i++) data.Write(player.Party[i].Name);
            ToPlayer(player, data);
        }

        public static void PartyInvitation(Player player, string playerInvitation)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.PartyInvitation);
            data.Write(playerInvitation);
            ToPlayer(player, data);
        }

        public static void Trade(Player player, bool state)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Trade);
            data.Write(state);
            ToPlayer(player, data);
        }

        public static void TradeInvitation(Player player, string playerInvitation)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.TradeInvitation);
            data.Write(playerInvitation);
            ToPlayer(player, data);
        }

        public static void TradeState(Player player, TradeStatus state)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.TradeState);
            data.Write((byte)state);
            ToPlayer(player, data);
        }

        public static void TradeOffer(Player player, bool own = true)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();
            Player to = own ? player : player.Trade;

            // Envia os dados
            data.Write((byte)ServerPackets.TradeOffer);
            data.Write(own);
            for (byte i = 0; i < MaxInventory; i++)
            {
                data.Write(to.Inventory[to.TradeOffer[i].SlotNum].Item.GetID());
                data.Write(to.TradeOffer[i].Amount);
            }
            ToPlayer(player, data);
        }

        public static void Shops(Account account)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.Shops);
            ObjectToByteArray(data, Shop.List);
            ToPlayer(account, data);
        }

        public static void ShopOpen(Player player, Shop shop)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ServerPackets.ShopOpen);
            data.Write(shop.GetID());
            ToPlayer(player, data);
        }
    }
}