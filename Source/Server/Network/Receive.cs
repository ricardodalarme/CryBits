using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;

class Receive
{
    // Pacotes do cliente
    private enum Client_Packets
    {
        Connect,
        Latency,
        Register,
        CreateCharacter,
        Character_Use,
        Character_Create,
        Character_Delete,
        Player_Direction,
        Player_Move,
        Player_Attack,
        RequestMap,
        Message,
        AddPoint,
        CollectItem,
        DropItem,
        Inventory_Change,
        Inventory_Use,
        Equipment_Remove,
        Hotbar_Add,
        Hotbar_Change,
        Hotbar_Use,
        Party_Invite,
        Party_Accept,
        Party_Decline,
        Party_Leave,
        Trade_Invite,
        Trade_Accept,
        Trade_Decline,
        Trade_Leave,
        Trade_Offer,
        Trade_Offer_State,
        Shop_Buy,
        Shop_Sell,
        Shop_Close,
        Warp
    }

    // Pacotes do editor
    private enum Editor_Packets
    {
        Connect,
        Write_Server_Data,
        Write_Classes,
        Write_Maps,
        Write_NPCs,
        Write_Items,
        Write_Shops,
        Request_Server_Data,
        Request_Classes,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items,
        Request_Shops
    }

    public static void Handle(Objects.Account Account, NetIncomingMessage Data)
    {
        byte Packet_Num = Data.ReadByte();
        Objects.Player Player = Account.Character;

        // Pacote principal de conexão
        if (Packet_Num == 0) Connect(Account, Data);
        else if (!Account.InEditor)
            // Manuseia os dados recebidos do cliente
            switch ((Client_Packets)Packet_Num)
            {
                case Client_Packets.Latency: Latency(Account); break;
                case Client_Packets.Register: Register(Account, Data); break;
                case Client_Packets.CreateCharacter: CreateCharacter(Account, Data); break;
                case Client_Packets.Character_Use: Character_Use(Account, Data); break;
                case Client_Packets.Character_Create: Character_Create(Account); break;
                case Client_Packets.Character_Delete: Character_Delete(Account, Data); break;
                case Client_Packets.Player_Direction: Player_Direction(Player, Data); break;
                case Client_Packets.Player_Move: Player_Move(Player, Data); break;
                case Client_Packets.Player_Attack: Player_Attack(Player); break;
                case Client_Packets.RequestMap: RequestMap(Player, Data); break;
                case Client_Packets.Message: Message(Player, Data); break;
                case Client_Packets.AddPoint: AddPoint(Player, Data); break;
                case Client_Packets.CollectItem: CollectItem(Player); break;
                case Client_Packets.DropItem: DropItem(Player, Data); break;
                case Client_Packets.Inventory_Change: Inventory_Change(Player, Data); break;
                case Client_Packets.Inventory_Use: Inventory_Use(Player, Data); break;
                case Client_Packets.Equipment_Remove: Equipment_Remove(Player, Data); break;
                case Client_Packets.Hotbar_Add: Hotbar_Add(Player, Data); break;
                case Client_Packets.Hotbar_Change: Hotbar_Change(Player, Data); break;
                case Client_Packets.Hotbar_Use: Hotbar_Use(Player, Data); break;
                case Client_Packets.Party_Invite: Party_Invite(Player, Data); break;
                case Client_Packets.Party_Accept: Party_Accept(Player); break;
                case Client_Packets.Party_Decline: Party_Decline(Player); break;
                case Client_Packets.Party_Leave: Party_Leave(Player); break;
                case Client_Packets.Trade_Invite: Trade_Invite(Player, Data); break;
                case Client_Packets.Trade_Accept: Trade_Accept(Player); break;
                case Client_Packets.Trade_Decline: Trade_Decline(Player); break;
                case Client_Packets.Trade_Leave: Trade_Leave(Player); break;
                case Client_Packets.Trade_Offer: Trade_Offer(Player, Data); break;
                case Client_Packets.Trade_Offer_State: Trade_Offer_State(Player, Data); break;
                case Client_Packets.Shop_Buy: Shop_Buy(Player, Data); break;
                case Client_Packets.Shop_Sell: Shop_Sell(Player, Data); break;
                case Client_Packets.Shop_Close: Shop_Close(Player); break;
                case Client_Packets.Warp: Warp(Player, Data); break;
            }
        else
            // Manuseia os dados recebidos do editor
            switch ((Editor_Packets)Packet_Num)
            {
                case Editor_Packets.Write_Server_Data: Write_Server_Data(Account, Data); break;
                case Editor_Packets.Write_Classes: Write_Classes(Account, Data); break;
                case Editor_Packets.Write_Maps: Write_Maps(Account, Data); break;
                case Editor_Packets.Write_NPCs: Write_NPCs(Account, Data); break;
                case Editor_Packets.Write_Items: Write_Items(Account, Data); break;
                case Editor_Packets.Write_Shops: Write_Shops(Account, Data); break;
                case Editor_Packets.Request_Server_Data: Request_Server_Data(Account); break;
                case Editor_Packets.Request_Classes: Request_Classes(Account); break;
                case Editor_Packets.Request_Map: Request_Map(Account, Data); break;
                case Editor_Packets.Request_Maps: Request_Maps(Account, Data); break;
                case Editor_Packets.Request_NPCs: Request_NPCs(Account); break;
                case Editor_Packets.Request_Items: Request_Items(Account); break;
                case Editor_Packets.Request_Shops: Request_Shops(Account); break;
            }
    }

    private static void Latency(Objects.Account Account)
    {
        // Envia o pacote para a contagem da latência
        Send.Latency(Account);
    }

    private static void Connect(Objects.Account Account, NetIncomingMessage Data)
    {
        // Lê os dados
        string User = Data.ReadString().Trim();
        string Password = Data.ReadString();
        bool Editor = Data.ReadBoolean();

        // Verifica se está tudo certo
        if (!Directory.Exists(Directories.Accounts.FullName + User))
        {
            Send.Alert(Account, "This username isn't registered.");
            return;
        }
        if (Lists.Account.Find(x => x.User.Equals(User)) != null)
        {
            Send.Alert(Account, "Someone already signed in to this account.");
            return;
        }

        // Carrega os dados da conta
        Read.Account(Account, User);

        // Verifica se a senha está correta
        if (!Account.Password.Equals(Password))
        {
            Send.Alert(Account, "Password is incorrect.");
            return;
        }

        Account.Acess = Game.Accesses.Administrator;

        if (Editor)
        {
            // Verifica se o jogador tem permissão para fazer entrar no modo edição
            if (Account.Acess < Game.Accesses.Editor)
            {
                Send.Alert(Account, "You're not allowed to do this.");
                return;
            }

            // Envia todos os dados
            Account.InEditor = true;
            Send.Server_Data(Account);
            Send.Maps(Account);
            Send.Items(Account);
            Send.Shops(Account);
            Send.Classes(Account);
            Send.NPCs(Account);
            Send.Connect(Account);
        }
        else
        {
            // Carrega os dados do jogador
            Read.Characters(Account);

            // Envia os dados das classes e dos personagens ao jogador
            Send.Classes(Account);
            Send.Characters(Account);

            // Se o jogador não tiver nenhum personagem então abrir o painel de criação de personagem
            if (Account.Characters.Count == 0)
            {
                Send.CreateCharacter(Account);
                return;
            }

            // Abre a janela de seleção de personagens
            Send.Connect(Account);
        }
    }

    private static void Register(Objects.Account Account, NetIncomingMessage Data)
    {
        // Lê os dados
        string User = Data.ReadString().Trim();
        string Password = Data.ReadString();

        // Verifica se está tudo certo
        if (User.Length < Lists.Server_Data.Min_Name_Length || User.Length > Lists.Server_Data.Max_Name_Length)
        {
            Send.Alert(Account, "The username must contain between " + Lists.Server_Data.Min_Name_Length + " and " + Lists.Server_Data.Max_Name_Length + " characters.");
            return;
        }
        if (Password.Length < Lists.Server_Data.Min_Name_Length || Password.Length > Lists.Server_Data.Max_Name_Length)
        {
            Send.Alert(Account, "The password must contain between " + Lists.Server_Data.Min_Name_Length + " and " + Lists.Server_Data.Max_Name_Length + " characters.");
            return;
        }
        if (File.Exists(Directories.Accounts.FullName + User + Directories.Format))
        {
            Send.Alert(Account, "There is already someone registered with this name.");
            return;
        }

        // Cria a conta
        Account.User = User;
        Account.Password = Password;

        // Salva a conta
        Write.Account(Account);

        // Abre a janela de seleção de personagens
        Send.Classes(Account);
        Send.CreateCharacter(Account);
    }

    private static void CreateCharacter(Objects.Account Account, NetIncomingMessage Data)
    {
        // Lê os dados
        string Name = Data.ReadString().Trim();

        // Verifica se está tudo certo
        if (Name.Length < Lists.Server_Data.Min_Name_Length || Name.Length > Lists.Server_Data.Max_Name_Length)
        {
            Send.Alert(Account, "The character name must contain between " + Lists.Server_Data.Min_Name_Length + " and " + Lists.Server_Data.Max_Name_Length + " characters.", false);
            return;
        }
        if (Name.Contains(";") || Name.Contains(":"))
        {
            Send.Alert(Account, "Can't contain ';' and ':' in the character name.", false);
            return;
        }
        if (Read.Characters_Name().Contains(";" + Name + ":"))
        {
            Send.Alert(Account, "A character with this name already exists", false);
            return;
        }

        // Define os valores iniciais do personagem
        Objects.Class Class;
        Account.Character = new Objects.Player(Account);
        Account.Character.Name = Name;
        Account.Character.Level = 1;
        Account.Character.Class = Class = (Objects.Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
        Account.Character.Genre = Data.ReadBoolean();
        if (Account.Character.Genre) Account.Character.Texture_Num = Class.Tex_Male[Data.ReadByte()];
        else Account.Character.Texture_Num = Class.Tex_Female[Data.ReadByte()];
        Account.Character.Attribute = Class.Attribute;
        Account.Character.Map = (Objects.TMap)Lists.GetData(Lists.Temp_Map, Class.Spawn_Map.ID);
        Account.Character.Direction = (Game.Directions)Class.Spawn_Direction;
        Account.Character.X = Class.Spawn_X;
        Account.Character.Y = Class.Spawn_Y;
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Account.Character.Vital[i] = Account.Character.MaxVital(i);
        for (byte i = 0; i < (byte)Class.Item.Length; i++)
            if (Class.Item[i].Item1.Type == (byte)Game.Items.Equipment && Account.Character.Equipment[Class.Item[i].Item1.Equip_Type] == null)
                Account.Character.Equipment[Class.Item[i].Item1.Equip_Type] = Class.Item[i].Item1;
            else
                Account.Character.GiveItem(Class.Item[i].Item1, Class.Item[i].Item2);

        // Salva a conta
        Write.Character_Name(Name);
        Write.Character(Account);

        // Entra no jogo
        Account.Character.Join();
    }

    private static void Character_Use(Objects.Account Account, NetIncomingMessage Data)
    {
        byte Character = Data.ReadByte();

        // Verifica se o personagem existe
        if (Character < 0 || Character >= Account.Characters.Count) return;

        // Entra no jogo
        Read.Character(Account, Account.Characters[Character].Name);
        Account.Character.Join();
    }

    private static void Character_Create(Objects.Account Account)
    {
        // Verifica se o jogador já criou o máximo de personagens possíveis
        if (Account.Characters.Count == Lists.Server_Data.Max_Characters)
        {
            Send.Alert(Account, "You can only have " + Lists.Server_Data.Max_Characters + " characters.", false);
            return;
        }

        // Abre a janela de seleção de personagens
        Send.Classes(Account);
        Send.CreateCharacter(Account);
    }

    private static void Character_Delete(Objects.Account Account, NetIncomingMessage Data)
    {
        byte Character = Data.ReadByte();

        // Verifica se o personagem existe
        if (Character < 0 || Character >= Account.Characters.Count) return;

        // Deleta o personagem
        string Name = Account.Characters[Character].Name;
        Send.Alert(Account, "The character '" + Name + "' has been deleted.", false);
        Write.Characters_Name(Read.Characters_Name().Replace(":;" + Name + ":", ":"));
        Account.Characters.RemoveAt(Character);
        File.Delete(Directories.Accounts.FullName + Account.User + "\\Characters\\" + Name + Directories.Format);

        // Salva o personagem
        Send.Characters(Account);
        Write.Account(Account);
    }

    private static void Player_Direction(Objects.Player Player, NetIncomingMessage Data)
    {
        Game.Directions Direction = (Game.Directions)Data.ReadByte();

        // Previne erros
        if (Direction < Game.Directions.Up || Direction > Game.Directions.Right) return;
        if (Player.GettingMap) return;

        // Defini a direção do jogador
        Player.Direction = Direction;
        Send.Player_Direction(Player);
    }

    private static void Player_Move(Objects.Player Player, NetIncomingMessage Data)
    {
        // Move o jogador se necessário
        if (Player.X != Data.ReadByte() || Player.Y != Data.ReadByte())
            Send.Player_Position(Player);
        else
            Player.Move(Data.ReadByte());
    }

    private static void RequestMap(Objects.Player Player, NetIncomingMessage Data)
    {
        // Se necessário enviar as informações do mapa ao jogador
        if (Data.ReadBoolean()) Send.Map(Player.Account, Player.Map.Data);

        // Envia a informação aos outros jogadores
        Send.Map_Players(Player);

        // Entra no mapa
        Player.GettingMap = false;
        Send.JoinMap(Player);
    }

    private static void Message(Objects.Player Player, NetIncomingMessage Data)
    {
        string Message = Data.ReadString();

        // Evita caracteres inválidos
        for (byte i = 0; i >= Message.Length; i++)
            if (Message[i] < 32 && Message[i] > 126)
                return;

        // Envia a mensagem para os outros jogadores
        switch ((Game.Messages)Data.ReadByte())
        {
            case Game.Messages.Map: Send.Message_Map(Player, Message); break;
            case Game.Messages.Global: Send.Message_Global(Player, Message); break;
            case Game.Messages.Private: Send.Message_Private(Player, Data.ReadString(), Message); break;
        }
    }

    private static void Player_Attack(Objects.Player Player)
    {
        // Ataca
        Player.Attack();
    }

    private static void AddPoint(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Attribute_Num = Data.ReadByte();

        // Adiciona um ponto a determinado atributo
        if (Player.Points > 0)
        {
            Player.Attribute[Attribute_Num] += 1;
            Player.Points -= 1;
            Send.Player_Experience(Player);
            Send.Map_Players(Player);
        }
    }

    private static void CollectItem(Objects.Player Player)
    {
        Objects.TMap_Items Map_Item = Player.Map.HasItem(Player.X, Player.Y);

        // Somente se necessário
        if (Map_Item == null) return;

        // Dá o item ao jogador
        if (Player.GiveItem(Map_Item.Item, Map_Item.Amount))
        {
            // Retira o item do mapa
            Player.Map.Item.Remove(Map_Item);
            Send.Map_Items(Player.Map);
        }
    }

    private static void DropItem(Objects.Player Player, NetIncomingMessage Data)
    {
        Player.DropItem(Data.ReadByte(), Data.ReadInt16());
    }

    private static void Inventory_Change(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Slot_Old = Data.ReadByte(), Slot_New = Data.ReadByte();
        byte Hotbar_Slot = Player.FindHotbar((byte)Game.Hotbar.Item, Slot_Old);

        // Somente se necessário
        if (Player.Inventory[Slot_Old].Item == null) return;
        if (Slot_Old == Slot_New) return;
        if (Player.Trade != null) return;

        // Muda o item de slot
        (Player.Inventory[Slot_Old].Item, Player.Inventory[Slot_New].Item) = (Player.Inventory[Slot_New].Item, Player.Inventory[Slot_Old].Item);
        (Player.Inventory[Slot_Old].Amount, Player.Inventory[Slot_New].Amount) = (Player.Inventory[Slot_New].Amount, Player.Inventory[Slot_Old].Amount);
        Player.Hotbar[Hotbar_Slot].Slot = Slot_New;
        Send.Player_Inventory(Player);
        Send.Player_Hotbar(Player);
    }

    private static void Inventory_Use(Objects.Player Player, NetIncomingMessage Data)
    {
        Player.UseItem(Data.ReadByte());
    }

    private static void Equipment_Remove(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Slot = Data.ReadByte();
        Objects.TMap_Items Map_Item = new Objects.TMap_Items();

        // Apenas se necessário
        if (Player.Equipment[Slot] == null) return;
        if (Player.Equipment[Slot].Bind == (byte)Game.BindOn.Equip) return;

        // Adiciona o equipamento ao inventário
        if (!Player.GiveItem(Player.Equipment[Slot], 1))
        {
            // Somente se necessário
            if (Player.Map.Item.Count == Lists.Server_Data.Max_Map_Items) return;

            // Solta o item no chão
            Map_Item.Item = Player.Equipment[Slot];
            Map_Item.Amount = 1;
            Map_Item.X = Player.X;
            Map_Item.Y = Player.Y;
            Player.Map.Item.Add(Map_Item);

            // Envia os dados
            Send.Map_Items(Player.Map);
            Send.Player_Inventory(Player);
        }

        // Remove o equipamento
        for (byte i = 0; i < (byte)Game.Attributes.Count; i++) Player.Attribute[i] -= Player.Equipment[Slot].Equip_Attribute[i];
        Player.Equipment[Slot] = null;

        // Envia os dados
        Send.Player_Equipments(Player);
    }

    private static void Hotbar_Add(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Hotbar_Slot = Data.ReadByte();
        byte Type = Data.ReadByte();
        byte Slot = Data.ReadByte();

        // Somente se necessário
        if (Slot != 0 && Player.FindHotbar(Type, Slot) > 0) return;

        // Define os dados
        Player.Hotbar[Hotbar_Slot].Slot = Slot;
        Player.Hotbar[Hotbar_Slot].Type = Type;

        // Envia os dados
        Send.Player_Hotbar(Player);
    }

    private static void Hotbar_Change(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Slot_Old = Data.ReadByte(), Slot_New = Data.ReadByte();

        // Somente se necessário
        if (Player.Hotbar[Slot_Old].Slot == 0) return;
        if (Slot_Old == Slot_New) return;

        // Muda o item de slot
        (Player.Hotbar[Slot_Old].Slot, Player.Hotbar[Slot_New].Slot) = (Player.Hotbar[Slot_New].Slot, Player.Hotbar[Slot_Old].Slot);
        (Player.Hotbar[Slot_Old].Type, Player.Hotbar[Slot_New].Type) = (Player.Hotbar[Slot_New].Type, Player.Hotbar[Slot_Old].Type);
        Send.Player_Hotbar(Player);
    }

    private static void Hotbar_Use(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Hotbar_Slot = Data.ReadByte();

        // Usa o item
        if (Player.Hotbar[Hotbar_Slot].Type == (byte)Game.Hotbar.Item)
            Player.UseItem(Player.Hotbar[Hotbar_Slot].Slot);
    }

    private static void Write_Server_Data(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Altera os dados
        Lists.Server_Data.Game_Name = Data.ReadString();
        Lists.Server_Data.Welcome = Data.ReadString();
        Lists.Server_Data.Port = Data.ReadInt16();
        Lists.Server_Data.Max_Players = Data.ReadByte();
        Lists.Server_Data.Max_Characters = Data.ReadByte();
        Lists.Server_Data.Max_Party_Members = Data.ReadByte();
        Lists.Server_Data.Max_Map_Items = Data.ReadByte();
        Lists.Server_Data.Num_Points = Data.ReadByte();
        Lists.Server_Data.Min_Name_Length = Data.ReadByte();
        Lists.Server_Data.Max_Name_Length = Data.ReadByte();
        Lists.Server_Data.Min_Password_Length = Data.ReadByte();
        Lists.Server_Data.Max_Password_Length = Data.ReadByte();

        // Salva os dados
        Write.Server_Data();
    }

    private static void Write_Classes(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Classes a serem removidas
        Dictionary<Guid, Objects.Class> ToRemove = new Dictionary<Guid, Objects.Class>(Lists.Class);

        // Quantidade de classes
        short Count = Data.ReadByte();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Objects.Class Class;

            // Obtém o dado
            if (Lists.Class.ContainsKey(ID))
            {
                Class = Lists.Class[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Class = new Objects.Class(ID);
                Lists.Class.Add(Class.ID, Class);
            }

            // Redimensiona os valores necessários 
            Class.Tex_Male = new short[Data.ReadByte()];
            Class.Tex_Female = new short[Data.ReadByte()];
            Class.Item = new Tuple<Objects.Item, short>[Data.ReadByte()];

            // Lê os dados
            Class.Name = Data.ReadString();
            Class.Description = Data.ReadString();
            for (byte t = 0; t < Class.Tex_Male.Length; t++) Class.Tex_Male[t] = Data.ReadInt16();
            for (byte t = 0; t < Class.Tex_Female.Length; t++) Class.Tex_Female[t] = Data.ReadInt16();
            Class.Spawn_Map = (Objects.Map)Lists.GetData(Lists.Map, new Guid(Data.ReadString()));
            Class.Spawn_Direction = Data.ReadByte();
            Class.Spawn_X = Data.ReadByte();
            Class.Spawn_Y = Data.ReadByte();
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Class.Vital[v] = Data.ReadInt16();
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Class.Attribute[a] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Class.Item.Length; n++) Class.Item[n] = new Tuple<Objects.Item, short>((Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16());

            // Salva os dados das classes
            Write.Class(Class);
        }

        // Remove as classes que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys)
        {
            Lists.Class.Remove(Remove);
            File.Delete(Directories.Classes.FullName + Remove.ToString() + Directories.Format);
        }

        // Salva os dados e envia pra todos jogadores conectados
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Classes(Lists.Account[i]);
    }

    private static void Write_Maps(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Classes a serem removidas
        Dictionary<Guid, Objects.Map> ToRemove = new Dictionary<Guid, Objects.Map>(Lists.Map);

        // Quantidade de classes
        short Count = Data.ReadByte();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Objects.Map Map;

            // Obtém o dado
            if (Lists.Map.ContainsKey(ID))
            {
                Map = Lists.Map[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Map = new Objects.Map(ID);
                Lists.Map.Add(Map.ID, Map);
                Map.Create_Temporary();
            }

            // Mapa temporário
            Objects.TMap Temp_Map = Lists.Temp_Map[ID];

            // Dados gerais
            Map.Revision = Data.ReadInt16();
            Map.Name = Data.ReadString();
            Map.Moral = Data.ReadByte();
            Map.Panorama = Data.ReadByte();
            Map.Music = Data.ReadByte();
            Map.Color = Data.ReadInt32();
            Map.Weather.Type = Data.ReadByte();
            Map.Weather.Intensity = Data.ReadByte();
            Map.Fog.Texture = Data.ReadByte();
            Map.Fog.Speed_X = Data.ReadSByte();
            Map.Fog.Speed_Y = Data.ReadSByte();
            Map.Fog.Alpha = Data.ReadByte();
            Map.Lighting = Data.ReadByte();

            // Ligações
            for (short n = 0; n < (short)Game.Directions.Count; n++)
                Map.Link[n] = (Objects.Map)Lists.GetData(Lists.Map, new Guid(Data.ReadString()));

            // Camadas
            Map.Layer = new Objects.Map_Layer[Data.ReadByte()];
            for (byte n = 0; n < Map.Layer.Length; n++)
            {
                // Dados básicos
                Map.Layer[n] = new Objects.Map_Layer();
                Map.Layer[n].Name = Data.ReadString();
                Map.Layer[n].Type = Data.ReadByte();

                // Azulejos
                for (byte x = 0; x < Objects.Map.Width; x++)
                    for (byte y = 0; y < Objects.Map.Height; y++)
                    {
                        Map.Layer[n].Tile[x, y].X = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].Y = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].Tile = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].Auto = Data.ReadBoolean();
                    }
            }

            // Dados específicos dos azulejos
            for (byte x = 0; x < Objects.Map.Width; x++)
                for (byte y = 0; y < Objects.Map.Height; y++)
                {
                    Map.Attribute[x, y] = new Objects.Map_Attribute();
                    Map.Attribute[x, y].Type = Data.ReadByte();
                    Map.Attribute[x, y].Data_1 = Data.ReadString();
                    Map.Attribute[x, y].Data_2 = Data.ReadInt16();
                    Map.Attribute[x, y].Data_3 = Data.ReadInt16();
                    Map.Attribute[x, y].Data_4 = Data.ReadInt16();
                    Map.Attribute[x, y].Zone = Data.ReadByte();

                    for (byte n = 0; n < (byte)Game.Directions.Count; n++)
                        Map.Attribute[x, y].Block[n] = Data.ReadBoolean();
                }

            // Luzes
            Map.Light = new Objects.Map_Light[Data.ReadByte()];
            for (byte n = 0; n < Map.Light.Length; n++)
            {
                Map.Light[n].X = Data.ReadByte();
                Map.Light[n].Y = Data.ReadByte();
                Map.Light[n].Width = Data.ReadByte();
                Map.Light[n].Height = Data.ReadByte();
            }

            // NPCs
            Map.NPC = new Objects.Map_NPC[Data.ReadByte()];
            Temp_Map.NPC = new Objects.TNPC[Map.NPC.Length];
            for (byte n = 0; n < Map.NPC.Length; n++)
            {
                Map.NPC[n].NPC = (Objects.NPC)Lists.GetData(Lists.NPC, new Guid(Data.ReadString()));
                Map.NPC[n].Zone = Data.ReadByte();
                Map.NPC[n].Spawn = Data.ReadBoolean();
                Map.NPC[n].X = Data.ReadByte();
                Map.NPC[n].Y = Data.ReadByte();
                Temp_Map.NPC[n].Spawn();
            }

            // Itens do mapa
            Temp_Map.Spawn_Items();

            // Envia o mapa para todos os jogadores que estão nele
            for (byte n = 0; n < Lists.Account.Count; n++)
                if (Lists.Account[n] != Account)
                    if (Lists.Account[n].Character.Map == Temp_Map || Lists.Account[n].InEditor)
                        Send.Map(Lists.Account[n], Temp_Map.Data);

            // Salva os dados das classes
            Write.Map(Map);
        }

        // Remove os mapas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys)
        {
            Lists.Map.Remove(Remove);
            File.Delete(Directories.Maps.FullName + Remove.ToString() + Directories.Format);
        }
    }

    private static void Write_NPCs(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Lojas a serem removidas
        Dictionary<Guid, Objects.NPC> ToRemove = new Dictionary<Guid, Objects.NPC>(Lists.NPC);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Objects.NPC NPC;

            // Obtém o dado
            if (Lists.NPC.ContainsKey(ID))
            {
                NPC = Lists.NPC[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                NPC = new Objects.NPC(ID);
                Lists.NPC.Add(NPC.ID, NPC);
            }

            // Lê os dados
            NPC.Name = Data.ReadString();
            NPC.SayMsg = Data.ReadString();
            NPC.Texture = Data.ReadInt16();
            NPC.Behaviour = Data.ReadByte();
            NPC.SpawnTime = Data.ReadByte();
            NPC.Sight = Data.ReadByte();
            NPC.Experience = Data.ReadInt32();
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++) NPC.Vital[n] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Game.Attributes.Count; n++) NPC.Attribute[n] = Data.ReadInt16();
            NPC.Drop = new Objects.NPC_Drop[Data.ReadByte()];
            for (byte n = 0; n < NPC.Drop.Length; n++) NPC.Drop[n] = new Objects.NPC_Drop((Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadByte());
            NPC.AttackNPC = Data.ReadBoolean();
            NPC.Allie = new Objects.NPC[Data.ReadByte()];
            for (byte n = 0; n < NPC.Allie.Length; n++) NPC.Allie[n] = (Objects.NPC)Lists.GetData(Lists.NPC, new Guid(Data.ReadString()));
            NPC.Movement = (Game.NPC_Movements)Data.ReadByte();
            NPC.Flee_Helth = Data.ReadByte();
            NPC.Shop = (Objects.Shop)Lists.GetData(Lists.Shop, new Guid(Data.ReadString()));

            // Salva os dados do item
            Write.NPC(NPC);
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys)
        {
            Lists.NPC.Remove(Remove);
            File.Delete(Directories.NPCs.FullName + Remove.ToString() + Directories.Format);
        }

        // Salva os dados e envia pra todos jogadores conectados
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.NPCs(Lists.Account[i]);
    }

    private static void Write_Items(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Lojas a serem removidas
        Dictionary<Guid, Objects.Item> ToRemove = new Dictionary<Guid, Objects.Item>(Lists.Item);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Objects.Item Item;

            // Obtém o dado
            if (Lists.Item.ContainsKey(ID))
            {
                Item = Lists.Item[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Item = new Objects.Item(ID);
                Lists.Item.Add(Item.ID, Item);
            }

            // Lê os dados
            Item.Name = Data.ReadString();
            Item.Description = Data.ReadString();
            Item.Texture = Data.ReadInt16();
            Item.Type = Data.ReadByte();
            Item.Stackable = Data.ReadBoolean();
            Item.Bind = Data.ReadByte();
            Item.Rarity = Data.ReadByte();
            Item.Req_Level = Data.ReadInt16();
            Item.Req_Class = (Objects.Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
            Item.Potion_Experience = Data.ReadInt32();
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Item.Potion_Vital[v] = Data.ReadInt16();
            Item.Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Item.Equip_Attribute[a] = Data.ReadInt16();
            Item.Weapon_Damage = Data.ReadInt16();

            // Salva os dados do item
            Write.Item(Item);
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys)
        {
            Lists.Item.Remove(Remove);
            File.Delete(Directories.Items.FullName + Remove.ToString() + Directories.Format);
        }

        // Salva os dados e envia pra todos jogadores conectados
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Items(Lists.Account[i]);
    }

    private static void Write_Shops(Objects.Account Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Lojas a serem removidas
        Dictionary<Guid, Objects.Shop> ToRemove = new Dictionary<Guid, Objects.Shop>(Lists.Shop);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Objects.Shop Shop;

            // Obtém o dado
            if (Lists.Shop.ContainsKey(ID))
            {
                Shop = Lists.Shop[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Shop = new Objects.Shop(ID);
                Lists.Shop.Add(Shop.ID, Shop);
            }

            // Redimensiona os valores necessários 
            Shop.Sold = new Objects.Shop_Item[Data.ReadByte()];
            Shop.Bought = new Objects.Shop_Item[Data.ReadByte()];

            // Lê os dados
            Shop.Name = Data.ReadString();
            Shop.Currency = (Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            for (byte j = 0; j < Shop.Sold.Length; j++)
                Shop.Sold[j] = new Objects.Shop_Item
                {
                    Item = (Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())),
                    Amount = Data.ReadInt16(),
                    Price = Data.ReadInt16()
                };
            for (byte j = 0; j < Shop.Bought.Length; j++)
                Shop.Bought[j] = new Objects.Shop_Item
                {
                    Item = (Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())),
                    Amount = Data.ReadInt16(),
                    Price = Data.ReadInt16()
                };

            // Salva os dados da loja
            Write.Shop(Shop);
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys)
        {
            Lists.Shop.Remove(Remove);
            File.Delete(Directories.Shops.FullName + Remove.ToString() + Directories.Format);
        }

        // Salva os dados e envia pra todos jogadores conectados
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Shops(Lists.Account[i]);
    }

    private static void Request_Server_Data(Objects.Account Account)
    {
        Send.Server_Data(Account);
    }

    private static void Request_Classes(Objects.Account Account)
    {
        Send.Classes(Account);
    }

    private static void Request_Map(Objects.Account Account, NetIncomingMessage Data)
    {
        Send.Map(Account, (Objects.Map)Lists.GetData(Lists.Map, new Guid(Data.ReadString())));
    }

    private static void Request_Maps(Objects.Account Account, NetIncomingMessage Data)
    {
        Send.Maps(Account);
    }

    private static void Request_NPCs(Objects.Account Account)
    {
        Send.NPCs(Account);
    }

    private static void Request_Items(Objects.Account Account)
    {
        Send.Items(Account);
    }

    private static void Request_Shops(Objects.Account Account)
    {
        Send.Shops(Account);
    }

    private static void Party_Invite(Objects.Player Player, NetIncomingMessage Data)
    {
        string Name = Data.ReadString();

        // Encontra o jogador
        Objects.Player Invited = Objects.Player.Find(Name);

        // Verifica se o jogador está convectado
        if (Invited == null)
        {
            Send.Message(Player, "The player ins't connected.", System.Drawing.Color.White);
            return;
        }
        // Verifica se não está tentando se convidar
        if (Invited == Player)
        {
            Send.Message(Player, "You can't be invited.", System.Drawing.Color.White);
            return;
        }
        // Verifica se já tem um grupo
        if (Invited.Party.Count != 0)
        {
            Send.Message(Player, "The player is already part of a party.", System.Drawing.Color.White);
            return;
        }
        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(Invited.Party_Request))
        {
            Send.Message(Player, "The player is analyzing an invitation to another party.", System.Drawing.Color.White);
            return;
        }
        // Verifica se o grupo está cheio
        if (Player.Party.Count == Lists.Server_Data.Max_Party_Members - 1)
        {
            Send.Message(Player, "Your party is full.", System.Drawing.Color.White);
            return;
        }

        // Convida o jogador
        Invited.Party_Request = Player.Name;
        Send.Party_Invitation(Invited, Player.Name);
    }

    private static void Party_Accept(Objects.Player Player)
    {
        Objects.Player Invitation = Objects.Player.Find(Player.Party_Request);

        // Verifica se já tem um grupo
        if (Player.Party.Count != 0)
        {
            Send.Message(Player, "You are already part of a party.", System.Drawing.Color.White);
            return;
        }

        // Verifica se quem chamou ainda está disponível
        if (Invitation == null)
        {
            Send.Message(Player, "Who invited you is no longer avaliable.", System.Drawing.Color.White);
            return;
        }
        // Verifica se o grupo está cheio
        if (Invitation.Party.Count == Lists.Server_Data.Max_Party_Members - 1)
        {
            Send.Message(Player, "The party is full.", System.Drawing.Color.White);
            return;
        }

        // Entra na festa
        for (byte i = 0; i < Invitation.Party.Count; i++)
        {
            Invitation.Party[i].Party.Add(Player);
            Player.Party.Add(Invitation.Party[i]);
        }
        Player.Party.Insert(0, Invitation);
        Invitation.Party.Add(Player);
        Player.Party_Request = string.Empty;
        Send.Message(Invitation, Player.Name + " joined the party.", System.Drawing.Color.White);

        // Envia os dados para o grupo
        Send.Party(Player);
        for (byte i = 0; i < Player.Party.Count; i++) Send.Party(Player.Party[i]);
    }

    private static void Party_Decline(Objects.Player Player)
    {
        Objects.Player Invitation = Objects.Player.Find(Player.Party_Request);

        // Recusa o convite
        if (Invitation != null) Send.Message(Invitation, Player.Name + " decline the party.", System.Drawing.Color.White);
        Player.Party_Request = string.Empty;
    }

    private static void Party_Leave(Objects.Player Player)
    {
        // Sai do grupo
        Player.Party_Leave();
    }

    private static void Trade_Invite(Objects.Player Player, NetIncomingMessage Data)
    {
        string Name = Data.ReadString();

        // Encontra o jogador
        Objects.Player Invited = Objects.Player.Find(Name);

        // Verifica se o jogador está convectado
        if (Invited == null)
        {
            Send.Message(Player, "The player ins't connected.", System.Drawing.Color.White);
            return;
        }
        // Verifica se não está tentando se convidar
        if (Invited == Player)
        {
            Send.Message(Player, "You can't be invited.", System.Drawing.Color.White);
            return;
        }
        // Verifica se já tem um grupo
        if (Invited.Trade != null)
        {
            Send.Message(Player, "The player is already part of a trade.", System.Drawing.Color.White);
            return;
        }
        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(Invited.Trade_Request))
        {
            Send.Message(Player, "The player is analyzing an invitation of another trade.", System.Drawing.Color.White);
            return;
        }
        // Verifica se os jogadores não estão em com a loja aberta
        if (Player.Shop != null)
        {
            Send.Message(Player, "You can't start a trade while in the shop.", System.Drawing.Color.White);
            return;
        }
        if (Invited.Shop != null)
        {
            Send.Message(Player, "The player is in the shop.", System.Drawing.Color.White);
            return;
        }
        // Verifica se os jogadores estão pertods um do outro
        if (System.Math.Abs(Player.X - Invited.X) + System.Math.Abs(Player.Y - Invited.Y) != 1)
        {
            Send.Message(Player, "You need to be close to the player to start trade.", System.Drawing.Color.White);
            return;
        }

        // Convida o jogador
        Invited.Trade_Request = Player.Name;
        Send.Trade_Invitation(Invited, Player.Name);
    }

    private static void Trade_Accept(Objects.Player Player)
    {
        Objects.Player Invited = Objects.Player.Find(Player.Trade_Request);

        // Verifica se já tem um grupo
        if (Player.Trade != null)
        {
            Send.Message(Player, "You are already part of a trade.", System.Drawing.Color.White);
            return;
        }
        // Verifica se quem chamou ainda está disponível
        if (Invited == null)
        {
            Send.Message(Player, "Who invited you is no longer avaliable.", System.Drawing.Color.White);
            return;
        }
        // Verifica se os jogadores estão pertods um do outro
        if (System.Math.Abs(Player.X - Invited.X) + System.Math.Abs(Player.Y - Invited.Y) != 1)
        {
            Send.Message(Player, "You need to be close to the player to accept the trade.", System.Drawing.Color.White);
            return;
        }
        // Verifica se  os jogadores não estão em com a loja aberta
        if (Invited.Shop != null)
        {
            Send.Message(Player, "Who invited you is in the shop.", System.Drawing.Color.White);
            return;
        }

        // Entra na troca
        Player.Trade = Invited;
        Invited.Trade = Player;
        Send.Message(Player, "You have accepted " + Invited.Name + "'s trade request.", System.Drawing.Color.White);
        Send.Message(Invited, Player.Name + " has accepted your trade request.", System.Drawing.Color.White);

        // Limpa os dadoss
        Player.Trade_Request = string.Empty;
        Player.Trade_Offer = new Lists.Structures.Trade_Slot[Game.Max_Inventory + 1];
        Invited.Trade_Offer = new Lists.Structures.Trade_Slot[Game.Max_Inventory + 1];

        // Envia os dados para o grupo
        Send.Trade(Player, true);
        Send.Trade(Invited, true);
    }

    private static void Trade_Decline(Objects.Player Player)
    {
        Objects.Player Invited = Objects.Player.Find(Player.Trade_Request);

        // Recusa o convite
        if (Invited != null) Send.Message(Invited, Player.Name + " decline the trade.", System.Drawing.Color.White);
        Player.Trade_Request = string.Empty;
    }

    private static void Trade_Leave(Objects.Player Player)
    {
        Player.Trade_Leave();
    }

    private static void Trade_Offer(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Slot = Data.ReadByte(), Inventory_Slot = Data.ReadByte();
        short Amount = Math.Min(Data.ReadInt16(), Player.Inventory[Inventory_Slot].Amount);

        // Adiciona o item à troca
        if (Inventory_Slot != 0)
        {
            // Evita itens repetidos
            for (byte i = 1; i <= Game.Max_Inventory; i++)
                if (Player.Trade_Offer[i].Slot_Num == Inventory_Slot)
                    return;

            Player.Trade_Offer[Slot].Slot_Num = Inventory_Slot;
            Player.Trade_Offer[Slot].Amount = Amount;
        }
        // Remove o item da troca
        else
            Player.Trade_Offer[Slot] = new Lists.Structures.Trade_Slot();

        // Envia os dados ao outro jogador
        Send.Trade_Offer(Player);
        Send.Trade_Offer(Player.Trade, false);
    }

    private static void Trade_Offer_State(Objects.Player Player, NetIncomingMessage Data)
    {
        Game.Trade_Status State = (Game.Trade_Status)Data.ReadByte();
        Objects.Player Invited = Player.Trade;

        switch (State)
        {
            case Game.Trade_Status.Accepted:
                // Verifica se os jogadores têm espaço disponivel para trocar os itens
                if (Player.Total_Trade_Items() > Invited.Total_Inventory_Free())
                {
                    Send.Message(Invited, Invited.Name + " don't have enought space in their inventory to do this trade.", System.Drawing.Color.Red);
                    break;
                }
                if (Invited.Total_Trade_Items() > Player.Total_Inventory_Free())
                {
                    Send.Message(Invited, "You don't have enought space in your inventory to do this trade.", System.Drawing.Color.Red);
                    break;
                }

                // Mensagem de confirmação
                Send.Message(Invited, "The offer was accepted.", System.Drawing.Color.Green);

                // Dados da oferta
                Lists.Structures.Inventories[] Your_Inventory = (Lists.Structures.Inventories[])Player.Inventory.Clone(),
                    Their_Inventory = (Lists.Structures.Inventories[])Invited.Inventory.Clone();

                // Remove os itens do inventário dos jogadores
                Objects.Player To = Player;
                for (byte j = 0; j < 2; j++, To = To == Player ? Invited : Player)
                    for (byte i = 1; i <= Game.Max_Inventory; i++)
                        To.TakeItem((byte)To.Trade_Offer[i].Slot_Num, To.Trade_Offer[i].Amount);

                // Dá os itens aos jogadores
                for (byte i = 1; i <= Game.Max_Inventory; i++)
                {
                    if (Player.Trade_Offer[i].Slot_Num > 0) Invited.GiveItem(Your_Inventory[Player.Trade_Offer[i].Slot_Num].Item, Player.Trade_Offer[i].Amount);
                    if (Invited.Trade_Offer[i].Slot_Num > 0) Player.GiveItem(Their_Inventory[Invited.Trade_Offer[i].Slot_Num].Item, Invited.Trade_Offer[i].Amount);
                }

                // Envia os dados do inventário aos jogadores
                Send.Player_Inventory(Player);
                Send.Player_Inventory(Invited);

                // Limpa a troca
                Player.Trade_Offer = new Lists.Structures.Trade_Slot[Game.Max_Inventory + 1];
                Invited.Trade_Offer = new Lists.Structures.Trade_Slot[Game.Max_Inventory + 1];
                Send.Trade_Offer(Invited);
                Send.Trade_Offer(Invited, false);
                break;
            case Game.Trade_Status.Declined:
                Send.Message(Invited, "The offer was declined.", System.Drawing.Color.Red);
                break;
            case Game.Trade_Status.Waiting:
                Send.Message(Invited, Player.Name + " send you a offer.", System.Drawing.Color.White);
                break;
        }

        // Envia os dados
        Send.Trade_State(Invited, State);
    }

    private static void Shop_Buy(Objects.Player Player, NetIncomingMessage Data)
    {
        Objects.Shop_Item Shop_Sold = Player.Shop.Sold[Data.ReadByte()];
        byte Inventory_Slot = Player.FindInventory(Player.Shop.Currency);

        // Verifica se o jogador tem dinheiro
        if (Inventory_Slot == 0 || Player.Inventory[Inventory_Slot].Amount < Shop_Sold.Price)
        {
            Send.Message(Player, "You don't have enough money to buy the item.", System.Drawing.Color.Red);
            return;
        }
        // Verifica se há espaço no inventário
        if (Player.Total_Inventory_Free() == 0 && Player.Inventory[Inventory_Slot].Amount > Shop_Sold.Price)
        {
            Send.Message(Player, "You  don't have space in your bag.", System.Drawing.Color.Red);
            return;
        }

        // Realiza a compra do item
        Player.TakeItem(Inventory_Slot, Shop_Sold.Price);
        Player.GiveItem(Shop_Sold.Item, Shop_Sold.Amount);
        Send.Message(Player, "You bought " + Shop_Sold.Price + "x " + Shop_Sold.Item.Name + ".", System.Drawing.Color.Green);
    }

    private static void Shop_Sell(Objects.Player Player, NetIncomingMessage Data)
    {
        byte Inventory_Slot = Data.ReadByte();
        short Amount = Math.Min(Data.ReadInt16(), Player.Inventory[Inventory_Slot].Amount);
        Objects.Shop_Item Buy = Player.Shop.BoughtItem(Player.Inventory[Inventory_Slot].Item);

        // Verifica se a loja vende o item
        if (Buy == null)
        {
            Send.Message(Player, "The store doesn't sell this item", System.Drawing.Color.Red);
            return;
        }
        // Verifica se há espaço no inventário
        if (Player.Total_Inventory_Free() == 0 && Player.Inventory[Inventory_Slot].Amount > Amount)
        {
            Send.Message(Player, "You don't have space in your bag.", System.Drawing.Color.Red);
            return;
        }

        // Realiza a venda do item
        Send.Message(Player, "You sold " + Player.Inventory[Inventory_Slot].Item.Name + "x " + Amount + "for .", System.Drawing.Color.Green);
        Player.TakeItem(Inventory_Slot, Amount);
        Player.GiveItem(Player.Shop.Currency, (short)(Buy.Price * Amount));
    }

    private static void Shop_Close(Objects.Player Player)
    {
        Player.Shop = null;
    }

    private static void Warp(Objects.Player Player, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Player.Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Player.Account, "You aren't allowed to do this.");
            return;
        }

        // Teletransporta o jogador para o mapa TODO
        //   Player.Warp(Data.ReadInt16(), Data.ReadByte(), Data.ReadByte());
    }
}