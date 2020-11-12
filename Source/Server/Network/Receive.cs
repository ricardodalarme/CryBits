using System;
using System.Collections.Generic;
using System.IO;
using CryBits.Entities;
using CryBits.Packets;
using CryBits.Server.Entities;
using CryBits.Server.Library;
using Lidgren.Network;
using static CryBits.Server.Logic.Utils;
using static CryBits.Utils;

namespace CryBits.Server.Network
{
    internal static class Receive
    {
        public static void Handle(Account account, NetIncomingMessage data)
        {
            byte packetNum = data.ReadByte();
            Player player = account.Character;

            // Pacote principal de conexão
            if (packetNum == 0) Connect(account, data);
            else if (!account.InEditor)
                // Manuseia os dados recebidos do cliente
                switch ((ClientServer)packetNum)
                {
                    case ClientServer.Latency: Latency(account); break;
                    case ClientServer.Register: Register(account, data); break;
                    case ClientServer.CreateCharacter: CreateCharacter(account, data); break;
                    case ClientServer.Character_Use: Character_Use(account, data); break;
                    case ClientServer.Character_Create: Character_Create(account); break;
                    case ClientServer.Character_Delete: Character_Delete(account, data); break;
                    case ClientServer.Player_Direction: Player_Direction(player, data); break;
                    case ClientServer.Player_Move: Player_Move(player, data); break;
                    case ClientServer.Player_Attack: Player_Attack(player); break;
                    case ClientServer.RequestMap: RequestMap(player, data); break;
                    case ClientServer.Message: Message(player, data); break;
                    case ClientServer.AddPoint: AddPoint(player, data); break;
                    case ClientServer.CollectItem: CollectItem(player); break;
                    case ClientServer.DropItem: DropItem(player, data); break;
                    case ClientServer.Inventory_Change: Inventory_Change(player, data); break;
                    case ClientServer.Inventory_Use: Inventory_Use(player, data); break;
                    case ClientServer.Equipment_Remove: Equipment_Remove(player, data); break;
                    case ClientServer.Hotbar_Add: Hotbar_Add(player, data); break;
                    case ClientServer.Hotbar_Change: Hotbar_Change(player, data); break;
                    case ClientServer.Hotbar_Use: Hotbar_Use(player, data); break;
                    case ClientServer.Party_Invite: Party_Invite(player, data); break;
                    case ClientServer.Party_Accept: Party_Accept(player); break;
                    case ClientServer.Party_Decline: Party_Decline(player); break;
                    case ClientServer.Party_Leave: Party_Leave(player); break;
                    case ClientServer.Trade_Invite: Trade_Invite(player, data); break;
                    case ClientServer.Trade_Accept: Trade_Accept(player); break;
                    case ClientServer.Trade_Decline: Trade_Decline(player); break;
                    case ClientServer.Trade_Leave: Trade_Leave(player); break;
                    case ClientServer.Trade_Offer: Trade_Offer(player, data); break;
                    case ClientServer.Trade_Offer_State: Trade_Offer_State(player, data); break;
                    case ClientServer.Shop_Buy: Shop_Buy(player, data); break;
                    case ClientServer.Shop_Sell: Shop_Sell(player, data); break;
                    case ClientServer.Shop_Close: Shop_Close(player); break;
                }
            else
                // Manuseia os dados recebidos do editor
                switch ((EditorServer)packetNum)
                {
                    case EditorServer.Write_Settings: Write_Settings(account, data); break;
                    case EditorServer.Write_Classes: Write_Classes(account, data); break;
                    case EditorServer.Write_Maps: Write_Maps(account, data); break;
                    case EditorServer.Write_NPCs: Write_NPCs(account, data); break;
                    case EditorServer.Write_Items: Write_Items(account, data); break;
                    case EditorServer.Write_Shops: Write_Shops(account, data); break;
                    case EditorServer.Request_Setting: Request_Setting(account); break;
                    case EditorServer.Request_Classes: Request_Classes(account); break;
                    case EditorServer.Request_Map: Request_Map(account, data); break;
                    case EditorServer.Request_Maps: Request_Maps(account); break;
                    case EditorServer.Request_NPCs: Request_NPCs(account); break;
                    case EditorServer.Request_Items: Request_Items(account); break;
                    case EditorServer.Request_Shops: Request_Shops(account); break;
                }
        }

        private static void Latency(Account account)
        {
            // Envia o pacote para a contagem da latência
            Send.Latency(account);
        }

        private static void Connect(Account account, NetIncomingMessage data)
        {
            // Lê os dados
            string user = data.ReadString().Trim();
            string password = data.ReadString();
            bool editor = data.ReadBoolean();

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

            account.Access = Accesses.Administrator;

            if (editor)
            {
                // Verifica se o jogador tem permissão para fazer entrar no modo edição
                if (account.Access < Accesses.Editor)
                {
                    Send.Alert(account, "You're not allowed to do this.");
                    return;
                }

                // Envia todos os dados
                account.InEditor = true;
                Send.Server_Data(account);
                Send.Maps(account);
                Send.Items(account);
                Send.Shops(account);
                Send.Classes(account);
                Send.NPCs(account);
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

        private static void Register(Account account, NetIncomingMessage data)
        {
            // Lê os dados
            string user = data.ReadString().Trim();
            string password = data.ReadString();

            // Verifica se está tudo certo
            if (user.Length < Min_Name_Length || user.Length > Max_Name_Length)
            {
                Send.Alert(account, "The username must contain between " + Min_Name_Length + " and " + Max_Name_Length + " characters.");
                return;
            }
            if (password.Length < Min_Name_Length || password.Length > Max_Name_Length)
            {
                Send.Alert(account, "The password must contain between " + Min_Name_Length + " and " + Max_Name_Length + " characters.");
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

        private static void CreateCharacter(Account account, NetIncomingMessage data)
        {
            // Lê os dados
            string name = data.ReadString().Trim();

            // Verifica se está tudo certo
            if (name.Length < Min_Name_Length || name.Length > Max_Name_Length)
            {
                Send.Alert(account, "The character name must contain between " + Min_Name_Length + " and " + Max_Name_Length + " characters.", false);
                return;
            }
            if (name.Contains(";") || name.Contains(":"))
            {
                Send.Alert(account, "Can't contain ';' and ':' in the character name.", false);
                return;
            }
            if (Read.Characters_Name().Contains(";" + name + ":"))
            {
                Send.Alert(account, "A character with this name already exists", false);
                return;
            }

            // Define os valores iniciais do personagem
            Class @class;
            account.Character = new Player(account);
            account.Character.Name = name;
            account.Character.Level = 1;
            account.Character.Class = @class = Class.Get(new Guid(data.ReadString()));
            account.Character.Genre = data.ReadBoolean();
            if (account.Character.Genre) account.Character.Texture_Num = @class.Tex_Male[data.ReadByte()];
            else account.Character.Texture_Num = @class.Tex_Female[data.ReadByte()];
            account.Character.Attribute = @class.Attribute;
            account.Character.Map = TempMap.Get(@class.Spawn_Map.ID);
            account.Character.Direction = (Directions)@class.Spawn_Direction;
            account.Character.X = @class.Spawn_X;
            account.Character.Y = @class.Spawn_Y;
            for (byte i = 0; i < (byte)Vitals.Count; i++) account.Character.Vital[i] = account.Character.MaxVital(i);
            for (byte i = 0; i < (byte)@class.Item.Count; i++)
                if (@class.Item[i].Item.Type == Items.Equipment && account.Character.Equipment[@class.Item[i].Item.Equip_Type] == null)
                    account.Character.Equipment[@class.Item[i].Item.Equip_Type] = @class.Item[i].Item;
                else
                    account.Character.GiveItem(@class.Item[i].Item, @class.Item[i].Amount);
            for (byte i = 0; i < MaxHotbar; i++) account.Character.Hotbar[i] = new Hotbar(Hotbars.None, 0);

            // Salva a conta
            Write.Character_Name(name);
            Write.Character(account);

            // Entra no jogo
            account.Character.Join();
        }

        private static void Character_Use(Account account, NetIncomingMessage data)
        {
            int character = data.ReadInt32();

            // Verifica se o personagem existe
            if (character < 0 || character >= account.Characters.Count) return;

            // Entra no jogo
            Read.Character(account, account.Characters[character].Name);
            account.Character.Join();
        }

        private static void Character_Create(Account account)
        {
            // Verifica se o jogador já criou o máximo de personagens possíveis
            if (account.Characters.Count == Max_Characters)
            {
                Send.Alert(account, "You can only have " + Max_Characters + " characters.", false);
                return;
            }

            // Abre a janela de seleção de personagens
            Send.Classes(account);
            Send.CreateCharacter(account);
        }

        private static void Character_Delete(Account account, NetIncomingMessage data)
        {
            int character = data.ReadInt32();

            // Verifica se o personagem existe
            if (character < 0 || character >= account.Characters.Count) return;

            // Deleta o personagem
            string name = account.Characters[character].Name;
            Send.Alert(account, "The character '" + name + "' has been deleted.", false);
            Write.Characters_Name(Read.Characters_Name().Replace(":;" + name + ":", ":"));
            account.Characters.RemoveAt(character);
            File.Delete(Directories.Accounts.FullName + account.User + "\\Characters\\" + name + Directories.Format);

            // Salva o personagem
            Send.Characters(account);
            Write.Account(account);
        }

        private static void Player_Direction(Player player, NetIncomingMessage data)
        {
            Directions direction = (Directions)data.ReadByte();

            // Previne erros
            if (direction < Directions.Up || direction > Directions.Right) return;
            if (player.GettingMap) return;

            // Defini a direção do jogador
            player.Direction = direction;
            Send.Player_Direction(player);
        }

        private static void Player_Move(Player player, NetIncomingMessage data)
        {
            // Move o jogador se necessário
            if (player.X != data.ReadByte() || player.Y != data.ReadByte())
                Send.Player_Position(player);
            else
                player.Move(data.ReadByte());
        }

        private static void RequestMap(Player player, NetIncomingMessage data)
        {
            // Se necessário enviar as informações do mapa ao jogador
            if (data.ReadBoolean()) Send.Map(player.Account, player.Map.Data);

            // Envia a informação aos outros jogadores
            Send.Map_Players(player);

            // Entra no mapa
            player.GettingMap = false;
            Send.JoinMap(player);
        }

        private static void Message(Player player, NetIncomingMessage data)
        {
            string message = data.ReadString();

            // Evita caracteres inválidos
            for (byte i = 0; i >= message.Length; i++)
                if (message[i] < 32 && message[i] > 126)
                    return;

            // Envia a mensagem para os outros jogadores
            switch ((Messages)data.ReadByte())
            {
                case Messages.Map: Send.Message_Map(player, message); break;
                case Messages.Global: Send.Message_Global(player, message); break;
                case Messages.Private: Send.Message_Private(player, data.ReadString(), message); break;
            }
        }

        private static void Player_Attack(Player player)
        {
            // Ataca
            player.Attack();
        }

        private static void AddPoint(Player player, NetIncomingMessage data)
        {
            byte attributeNum = data.ReadByte();

            // Adiciona um ponto a determinado atributo
            if (player.Points > 0)
            {
                player.Attribute[attributeNum] += 1;
                player.Points -= 1;
                Send.Player_Experience(player);
                Send.Map_Players(player);
            }
        }

        private static void CollectItem(Player player)
        {
            MapItems mapItem = player.Map.HasItem(player.X, player.Y);

            // Somente se necessário
            if (mapItem == null) return;

            // Dá o item ao jogador
            if (player.GiveItem(mapItem.Item, mapItem.Amount))
            {
                // Retira o item do mapa
                player.Map.Item.Remove(mapItem);
                Send.Map_Items(player.Map);
            }
        }

        private static void DropItem(Player player, NetIncomingMessage data)
        {
            player.DropItem(data.ReadByte(), data.ReadInt16());
        }

        private static void Inventory_Change(Player player, NetIncomingMessage data)
        {
            byte slotOld = data.ReadByte(), slotNew = data.ReadByte();

            // Somente se necessário
            if (player.Inventory[slotOld].Item == null) return;
            if (slotOld == slotNew) return;
            if (player.Trade != null) return;

            // Muda o item de slot
            Swap(ref player.Inventory[slotOld], ref player.Inventory[slotNew]);
            Send.Player_Inventory(player);

            // Altera na hotbar
            Hotbar hotbarSlot = player.FindHotbar(Hotbars.Item, slotOld);
            if (hotbarSlot != null)
            {
                hotbarSlot.Slot = slotNew;
                Send.Player_Hotbar(player);
            }
        }

        private static void Inventory_Use(Player player, NetIncomingMessage data)
        {
            player.UseItem(data.ReadByte());
        }

        private static void Equipment_Remove(Player player, NetIncomingMessage data)
        {
            byte slot = data.ReadByte();
            MapItems mapItem = new MapItems();

            // Apenas se necessário
            if (player.Equipment[slot] == null) return;
            if (player.Equipment[slot].Bind == BindOn.Equip) return;

            // Adiciona o equipamento ao inventário
            if (!player.GiveItem(player.Equipment[slot], 1))
            {
                // Somente se necessário
                if (player.Map.Item.Count == Max_Map_Items) return;

                // Solta o item no chão
                mapItem.Item = player.Equipment[slot];
                mapItem.Amount = 1;
                mapItem.X = player.X;
                mapItem.Y = player.Y;
                player.Map.Item.Add(mapItem);

                // Envia os dados
                Send.Map_Items(player.Map);
                Send.Player_Inventory(player);
            }

            // Remove o equipamento
            for (byte i = 0; i < (byte)Attributes.Count; i++) player.Attribute[i] -= player.Equipment[slot].Equip_Attribute[i];
            player.Equipment[slot] = null;

            // Envia os dados
            Send.Player_Equipments(player);
        }

        private static void Hotbar_Add(Player player, NetIncomingMessage data)
        {
            short hotbarSlot = data.ReadInt16();
            Hotbars type = (Hotbars)data.ReadByte();
            byte slot = data.ReadByte();

            // Somente se necessário
            if (slot != 0 && player.FindHotbar(type, slot) != null) return;

            // Define os dados
            player.Hotbar[hotbarSlot].Slot = slot;
            player.Hotbar[hotbarSlot].Type = type;

            // Envia os dados
            Send.Player_Hotbar(player);
        }

        private static void Hotbar_Change(Player player, NetIncomingMessage data)
        {
            short slotOld = data.ReadInt16(), slotNew = data.ReadInt16();

            // Somente se necessário
            if (slotOld < 0 || slotNew < 0) return;
            if (slotOld == slotNew) return;
            if (player.Hotbar[slotOld].Slot == 0) return;

            // Muda o item de slot
            Swap(ref player.Hotbar[slotOld], ref player.Hotbar[slotNew]);
            Send.Player_Hotbar(player);
        }

        private static void Hotbar_Use(Player player, NetIncomingMessage data)
        {
            byte hotbarSlot = data.ReadByte();

            // Usa o item
            switch (player.Hotbar[hotbarSlot].Type)
            {
                case Hotbars.Item: player.UseItem(player.Hotbar[hotbarSlot].Slot); break;
            }
        }

        private static void Write_Settings(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Altera os dados
            Game_Name = data.ReadString();
            Welcome_Message = data.ReadString();
            Port = data.ReadInt16();
            Max_Players = data.ReadByte();
            Max_Characters = data.ReadByte();
            Max_Party_Members = data.ReadByte();
            Max_Map_Items = data.ReadByte();
            Num_Points = data.ReadByte();
            Min_Name_Length = data.ReadByte();
            Max_Name_Length = data.ReadByte();
            Min_Password_Length = data.ReadByte();
            Max_Password_Length = data.ReadByte();

            // Salva os dados
            Write.Settings();
        }

        private static void Write_Classes(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Recebe e salva os novos dados
            Class.List = (Dictionary<Guid, Class>)ByteArrayToObject(data);
            Write.Classes();

            // Envia os novos dados para todos jogadores conectados
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i] != account)
                    Send.Classes(Account.List[i]);
        }

        private static void Write_Maps(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Recebe e salva os novos dados
            Map.List = (Dictionary<Guid, Map>)ByteArrayToObject(data);
            Write.Maps();

            // Envia os novos dados para todos jogadores 
            foreach (var tempMap in TempMap.List.Values)
            {
                // Itens do mapa
                tempMap.Spawn_Items();

                // Envia o mapa para todos os jogadores que estão nele
                for (byte n = 0; n < Account.List.Count; n++)
                    if (Account.List[n] != account)
                        if (Account.List[n].Character.Map == tempMap || Account.List[n].InEditor)
                            Send.Map(Account.List[n], tempMap.Data);
            }
        }

        private static void Write_NPCs(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Recebe e salva os novos dados
            NPC.List = (Dictionary<Guid, NPC>)ByteArrayToObject(data);
            Write.NPCs();

            // Envia os novos dados para todos jogadores conectados
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i] != account)
                    Send.NPCs(Account.List[i]);
        }

        private static void Write_Items(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Recebe e salva os novos dados
            Item.List = (Dictionary<Guid, Item>)ByteArrayToObject(data);
            Write.Items();

            // Envia os novos dados para todos jogadores conectados
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i] != account)
                    Send.Items(Account.List[i]);
        }

        private static void Write_Shops(Account account, NetIncomingMessage data)
        {
            // Verifica se o jogador realmente tem permissão 
            if (account.Access < Accesses.Editor)
            {
                Send.Alert(account, "You aren't allowed to do this.");
                return;
            }

            // Recebe e salva os novos dados
            Shop.List = (Dictionary<Guid, Shop>)ByteArrayToObject(data);
            Write.Shops();

            // Envia os novos dados para todos jogadores conectados
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i] != account)
                    Send.Shops(Account.List[i]);
        }

        private static void Request_Setting(Account account)
        {
            Send.Server_Data(account);
        }

        private static void Request_Classes(Account account)
        {
            Send.Classes(account);
        }

        private static void Request_Map(Account account, NetIncomingMessage data)
        {
            Send.Map(account, Map.Get(new Guid(data.ReadString())));
        }

        private static void Request_Maps(Account account)
        {
            Send.Maps(account);
        }

        private static void Request_NPCs(Account account)
        {
            Send.NPCs(account);
        }

        private static void Request_Items(Account account)
        {
            Send.Items(account);
        }

        private static void Request_Shops(Account account)
        {
            Send.Shops(account);
        }

        private static void Party_Invite(Player player, NetIncomingMessage data)
        {
            string name = data.ReadString();

            // Encontra o jogador
            Player invited = Player.Find(name);

            // Verifica se o jogador está convectado
            if (invited == null)
            {
                Send.Message(player, "The player ins't connected.", System.Drawing.Color.White);
                return;
            }
            // Verifica se não está tentando se convidar
            if (invited == player)
            {
                Send.Message(player, "You can't be invited.", System.Drawing.Color.White);
                return;
            }
            // Verifica se já tem um grupo
            if (invited.Party.Count != 0)
            {
                Send.Message(player, "The player is already part of a party.", System.Drawing.Color.White);
                return;
            }
            // Verifica se o jogador já está analisando um convite para algum grupo
            if (!string.IsNullOrEmpty(invited.Party_Request))
            {
                Send.Message(player, "The player is analyzing an invitation to another party.", System.Drawing.Color.White);
                return;
            }
            // Verifica se o grupo está cheio
            if (player.Party.Count == Max_Party_Members - 1)
            {
                Send.Message(player, "Your party is full.", System.Drawing.Color.White);
                return;
            }

            // Convida o jogador
            invited.Party_Request = player.Name;
            Send.Party_Invitation(invited, player.Name);
        }

        private static void Party_Accept(Player player)
        {
            Player invitation = Player.Find(player.Party_Request);

            // Verifica se já tem um grupo
            if (player.Party.Count != 0)
            {
                Send.Message(player, "You are already part of a party.", System.Drawing.Color.White);
                return;
            }

            // Verifica se quem chamou ainda está disponível
            if (invitation == null)
            {
                Send.Message(player, "Who invited you is no longer avaliable.", System.Drawing.Color.White);
                return;
            }
            // Verifica se o grupo está cheio
            if (invitation.Party.Count == Max_Party_Members - 1)
            {
                Send.Message(player, "The party is full.", System.Drawing.Color.White);
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
            player.Party_Request = string.Empty;
            Send.Message(invitation, player.Name + " joined the party.", System.Drawing.Color.White);

            // Envia os dados para o grupo
            Send.Party(player);
            for (byte i = 0; i < player.Party.Count; i++) Send.Party(player.Party[i]);
        }

        private static void Party_Decline(Player player)
        {
            Player invitation = Player.Find(player.Party_Request);

            // Recusa o convite
            if (invitation != null) Send.Message(invitation, player.Name + " decline the party.", System.Drawing.Color.White);
            player.Party_Request = string.Empty;
        }

        private static void Party_Leave(Player player)
        {
            // Sai do grupo
            player.Party_Leave();
        }

        private static void Trade_Invite(Player player, NetIncomingMessage data)
        {
            string name = data.ReadString();

            // Encontra o jogador
            Player invited = Player.Find(name);

            // Verifica se o jogador está convectado
            if (invited == null)
            {
                Send.Message(player, "The player ins't connected.", System.Drawing.Color.White);
                return;
            }
            // Verifica se não está tentando se convidar
            if (invited == player)
            {
                Send.Message(player, "You can't be invited.", System.Drawing.Color.White);
                return;
            }
            // Verifica se já tem um grupo
            if (invited.Trade != null)
            {
                Send.Message(player, "The player is already part of a trade.", System.Drawing.Color.White);
                return;
            }
            // Verifica se o jogador já está analisando um convite para algum grupo
            if (!string.IsNullOrEmpty(invited.Trade_Request))
            {
                Send.Message(player, "The player is analyzing an invitation of another trade.", System.Drawing.Color.White);
                return;
            }
            // Verifica se os jogadores não estão em com a loja aberta
            if (player.Shop != null)
            {
                Send.Message(player, "You can't start a trade while in the shop.", System.Drawing.Color.White);
                return;
            }
            if (invited.Shop != null)
            {
                Send.Message(player, "The player is in the shop.", System.Drawing.Color.White);
                return;
            }
            // Verifica se os jogadores estão pertods um do outro
            if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
            {
                Send.Message(player, "You need to be close to the player to start trade.", System.Drawing.Color.White);
                return;
            }

            // Convida o jogador
            invited.Trade_Request = player.Name;
            Send.Trade_Invitation(invited, player.Name);
        }

        private static void Trade_Accept(Player player)
        {
            Player invited = Player.Find(player.Trade_Request);

            // Verifica se já tem um grupo
            if (player.Trade != null)
            {
                Send.Message(player, "You are already part of a trade.", System.Drawing.Color.White);
                return;
            }
            // Verifica se quem chamou ainda está disponível
            if (invited == null)
            {
                Send.Message(player, "Who invited you is no longer avaliable.", System.Drawing.Color.White);
                return;
            }
            // Verifica se os jogadores estão pertods um do outro
            if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
            {
                Send.Message(player, "You need to be close to the player to accept the trade.", System.Drawing.Color.White);
                return;
            }
            // Verifica se  os jogadores não estão em com a loja aberta
            if (invited.Shop != null)
            {
                Send.Message(player, "Who invited you is in the shop.", System.Drawing.Color.White);
                return;
            }

            // Entra na troca
            player.Trade = invited;
            invited.Trade = player;
            Send.Message(player, "You have accepted " + invited.Name + "'s trade request.", System.Drawing.Color.White);
            Send.Message(invited, player.Name + " has accepted your trade request.", System.Drawing.Color.White);

            // Limpa os dadoss
            player.Trade_Request = string.Empty;
            player.Trade_Offer = new TradeSlot[MaxInventory + 1];
            invited.Trade_Offer = new TradeSlot[MaxInventory + 1];

            // Envia os dados para o grupo
            Send.Trade(player, true);
            Send.Trade(invited, true);
        }

        private static void Trade_Decline(Player player)
        {
            Player invited = Player.Find(player.Trade_Request);

            // Recusa o convite
            if (invited != null) Send.Message(invited, player.Name + " decline the trade.", System.Drawing.Color.White);
            player.Trade_Request = string.Empty;
        }

        private static void Trade_Leave(Player player)
        {
            player.Trade_Leave();
        }

        private static void Trade_Offer(Player player, NetIncomingMessage data)
        {
            byte slot = data.ReadByte(), inventorySlot = data.ReadByte();
            short amount = Math.Min(data.ReadInt16(), player.Inventory[inventorySlot].Amount);

            // Adiciona o item à troca
            if (inventorySlot != 0)
            {
                // Evita itens repetidos
                for (byte i = 1; i <= MaxInventory; i++)
                    if (player.Trade_Offer[i].Slot_Num == inventorySlot)
                        return;

                player.Trade_Offer[slot].Slot_Num = inventorySlot;
                player.Trade_Offer[slot].Amount = amount;
            }
            // Remove o item da troca
            else
                player.Trade_Offer[slot] = new TradeSlot();

            // Envia os dados ao outro jogador
            Send.Trade_Offer(player);
            Send.Trade_Offer(player.Trade, false);
        }

        private static void Trade_Offer_State(Player player, NetIncomingMessage data)
        {
            TradeStatus state = (TradeStatus)data.ReadByte();
            Player invited = player.Trade;

            switch (state)
            {
                case TradeStatus.Accepted:
                    // Verifica se os jogadores têm espaço disponivel para trocar os itens
                    if (player.Total_Trade_Items() > invited.Total_Inventory_Free())
                    {
                        Send.Message(invited, invited.Name + " don't have enought space in their inventory to do this trade.", System.Drawing.Color.Red);
                        break;
                    }
                    if (invited.Total_Trade_Items() > player.Total_Inventory_Free())
                    {
                        Send.Message(invited, "You don't have enought space in your inventory to do this trade.", System.Drawing.Color.Red);
                        break;
                    }

                    // Mensagem de confirmação
                    Send.Message(invited, "The offer was accepted.", System.Drawing.Color.Green);

                    // Dados da oferta
                    Inventory[] yourInventory = (Inventory[])player.Inventory.Clone(),
                       theirInventory = (Inventory[])invited.Inventory.Clone();

                    // Remove os itens do inventário dos jogadores
                    Player to = player;
                    for (byte j = 0; j < 2; j++, to = to == player ? invited : player)
                        for (byte i = 1; i <= MaxInventory; i++)
                            to.TakeItem((byte)to.Trade_Offer[i].Slot_Num, to.Trade_Offer[i].Amount);

                    // Dá os itens aos jogadores
                    for (byte i = 1; i <= MaxInventory; i++)
                    {
                        if (player.Trade_Offer[i].Slot_Num > 0) invited.GiveItem(yourInventory[player.Trade_Offer[i].Slot_Num].Item, player.Trade_Offer[i].Amount);
                        if (invited.Trade_Offer[i].Slot_Num > 0) player.GiveItem(theirInventory[invited.Trade_Offer[i].Slot_Num].Item, invited.Trade_Offer[i].Amount);
                    }

                    // Envia os dados do inventário aos jogadores
                    Send.Player_Inventory(player);
                    Send.Player_Inventory(invited);

                    // Limpa a troca
                    player.Trade_Offer = new TradeSlot[MaxInventory + 1];
                    invited.Trade_Offer = new TradeSlot[MaxInventory + 1];
                    Send.Trade_Offer(invited);
                    Send.Trade_Offer(invited, false);
                    break;
                case TradeStatus.Declined:
                    Send.Message(invited, "The offer was declined.", System.Drawing.Color.Red);
                    break;
                case TradeStatus.Waiting:
                    Send.Message(invited, player.Name + " send you a offer.", System.Drawing.Color.White);
                    break;
            }

            // Envia os dados
            Send.Trade_State(invited, state);
        }

        private static void Shop_Buy(Player player, NetIncomingMessage data)
        {
            ShopItem shopSold = player.Shop.Sold[data.ReadByte()];
            byte inventorySlot = player.FindInventory(player.Shop.Currency);

            // Verifica se o jogador tem dinheiro
            if (inventorySlot == 0 || player.Inventory[inventorySlot].Amount < shopSold.Price)
            {
                Send.Message(player, "You don't have enough money to buy the item.", System.Drawing.Color.Red);
                return;
            }
            // Verifica se há espaço no inventário
            if (player.Total_Inventory_Free() == 0 && player.Inventory[inventorySlot].Amount > shopSold.Price)
            {
                Send.Message(player, "You  don't have space in your bag.", System.Drawing.Color.Red);
                return;
            }

            // Realiza a compra do item
            player.TakeItem(inventorySlot, shopSold.Price);
            player.GiveItem(shopSold.Item, shopSold.Amount);
            Send.Message(player, "You bought " + shopSold.Price + "x " + shopSold.Item.Name + ".", System.Drawing.Color.Green);
        }

        private static void Shop_Sell(Player player, NetIncomingMessage data)
        {
            byte inventorySlot = data.ReadByte();
            short amount = Math.Min(data.ReadInt16(), player.Inventory[inventorySlot].Amount);
            ShopItem buy = player.Shop.FindBought(player.Inventory[inventorySlot].Item);

            // Verifica se a loja vende o item
            if (buy == null)
            {
                Send.Message(player, "The store doesn't sell this item", System.Drawing.Color.Red);
                return;
            }
            // Verifica se há espaço no inventário
            if (player.Total_Inventory_Free() == 0 && player.Inventory[inventorySlot].Amount > amount)
            {
                Send.Message(player, "You don't have space in your bag.", System.Drawing.Color.Red);
                return;
            }

            // Realiza a venda do item
            Send.Message(player, "You sold " + player.Inventory[inventorySlot].Item.Name + "x " + amount + "for .", System.Drawing.Color.Green);
            player.TakeItem(inventorySlot, amount);
            player.GiveItem(player.Shop.Currency, (short)(buy.Price * amount));
        }

        private static void Shop_Close(Player player)
        {
            player.Shop = null;
        }
    }
}