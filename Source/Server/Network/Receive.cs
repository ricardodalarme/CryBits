using Lidgren.Network;
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
        Write_Tiles,
        Write_Maps,
        Write_NPCs,
        Write_Items,
        Write_Shops,
        Request_Server_Data,
        Request_Classes,
        Request_Tiles,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items,
        Request_Shops
    }

    public static void Handle(Account.Structure Account, NetIncomingMessage Data)
    {
        byte Packet_Num = Data.ReadByte();
        Player Player = Account.Character;

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
                case Editor_Packets.Write_Tiles: Write_Tiles(Account, Data); break;
                case Editor_Packets.Write_Maps: Write_Maps(Account, Data); break;
                case Editor_Packets.Write_NPCs: Write_NPCs(Account, Data); break;
                case Editor_Packets.Write_Items: Write_Items(Account, Data); break;
                case Editor_Packets.Write_Shops: Write_Shops(Account, Data); break;
                case Editor_Packets.Request_Server_Data: Request_Server_Data(Account); break;
                case Editor_Packets.Request_Classes: Request_Classes(Account); break;
                case Editor_Packets.Request_Tiles: Request_Tiles(Account); break;
                case Editor_Packets.Request_Map: Request_Map(Account, Data); break;
                case Editor_Packets.Request_Maps: Request_Maps(Account, Data); break;
                case Editor_Packets.Request_NPCs: Request_NPCs(Account); break;
                case Editor_Packets.Request_Items: Request_Items(Account); break;
                case Editor_Packets.Request_Shops: Request_Shops(Account); break;
            }
    }

    private static void Latency(Account.Structure Account)
    {
        // Envia o pacote para a contagem da latência
        Send.Latency(Account);
    }

    private static void Connect(Account.Structure Account, NetIncomingMessage Data)
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
        if (global::Account.MultipleAccounts(User))
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

        if (Editor)
        {
            // Verifica se o jogador tem permissão para fazer entrar no modo edição
            if (Account.Acess < Game.Accesses.Editor)
            {
                Send.Alert(Account, "You're not allowed to do this.");
                return;
            }

            // Abre a janela de edição
            Account.InEditor = true;
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

    private static void Register(Account.Structure Account, NetIncomingMessage Data)
    {
        // Lê os dados
        string User = Data.ReadString().Trim();
        string Password = Data.ReadString();

        // Verifica se está tudo certo
        if (User.Length < Game.Min_Name_Length || User.Length > Game.Max_Name_Length || Password.Length < Game.Min_Name_Length || Password.Length > Game.Max_Name_Length)
        {
            Send.Alert(Account, "The username and password must contain between " + Game.Min_Name_Length + " and " + Game.Max_Name_Length + " characters.");
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

    private static void CreateCharacter(Account.Structure Account, NetIncomingMessage Data)
    {
        // Lê os dados
        string Name = Data.ReadString().Trim();

        // Verifica se está tudo certo
        if (Name.Length < Game.Min_Name_Length || Name.Length > Game.Max_Name_Length)
        {
            Send.Alert(Account, "The character name must contain between " + Game.Min_Name_Length + " and " + Game.Max_Name_Length + " characters.", false);
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
        Account.Character = new Player(Account);
        Account.Character.Name = Name;
        Account.Character.Level = 1;
        Account.Character.Class_Num = Data.ReadByte();
        Lists.Structures.Class Class = Lists.Class[Account.Character.Class_Num];
        Account.Character.Genre = Data.ReadBoolean();
        if (Account.Character.Genre) Account.Character.Texture_Num = Class.Tex_Male[Data.ReadByte()];
        else Account.Character.Texture_Num = Class.Tex_Female[Data.ReadByte()];
        Account.Character.Attribute = Class.Attribute;
        Account.Character.Map_Num = Class.Spawn_Map;
        Account.Character.Direction = (Game.Directions)Class.Spawn_Direction;
        Account.Character.X = Class.Spawn_X;
        Account.Character.Y = Class.Spawn_Y;
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Account.Character.Vital[i] = Account.Character.MaxVital(i);
        for (byte i = 0; i < (byte)Class.Item.Length; i++)
            if (Lists.Item[Class.Item[i].Item1].Type == (byte)Game.Items.Equipment && Account.Character.Equipment[Lists.Item[Class.Item[i].Item1].Equip_Type] == 0)
                Account.Character.Equipment[Lists.Item[Class.Item[i].Item1].Equip_Type] = Class.Item[i].Item1;
            else
                Account.Character.GiveItem(Class.Item[i].Item1, Class.Item[i].Item2);

        // Salva a conta
        Write.Character_Name(Name);
        Write.Character(Account);

        // Entra no jogo
        Account.Character.Join();
    }

    private static void Character_Use(Account.Structure Account, NetIncomingMessage Data)
    {
        byte Character = Data.ReadByte();

        // Verifica se o personagem existe
        if (Character < 0 || Character >= Account.Characters.Count) return;

        // Entra no jogo
        Read.Character(Account, Account.Characters[Character].Name);
        Account.Character.Join();
    }

    private static void Character_Create(Account.Structure Account)
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

    private static void Character_Delete(Account.Structure Account, NetIncomingMessage Data)
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

    private static void Player_Direction(Player Player, NetIncomingMessage Data)
    {
        Game.Directions Direction = (Game.Directions)Data.ReadByte();

        // Previne erros
        if (Direction < Game.Directions.Up || Direction > Game.Directions.Right) return;
        if (Player.GettingMap) return;

        // Defini a direção do jogador
        Player.Direction = Direction;
        Send.Player_Direction(Player);
    }

    private static void Player_Move(Player Player, NetIncomingMessage Data)
    {
        // Move o jogador se necessário
        if (Player.X != Data.ReadByte() || Player.Y != Data.ReadByte())
            Send.Player_Position(Player);
        else
            Player.Move(Data.ReadByte());
    }

    private static void RequestMap(Player Player, NetIncomingMessage Data)
    {
        // Se necessário enviar as informações do mapa ao jogador
        if (Data.ReadBoolean()) Send.Map(Player.Account, Player.Map_Num);

        // Envia a informação aos outros jogadores
        Send.Map_Players(Player);

        // Entra no mapa
        Player.GettingMap = false;
        Send.JoinMap(Player);
    }

    private static void Message(Player Player, NetIncomingMessage Data)
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

    private static void Player_Attack(Player Player)
    {
        // Ataca
        Player.Attack();
    }

    private static void AddPoint(Player Player, NetIncomingMessage Data)
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

    private static void CollectItem(Player Player)
    {
        short Map_Num = Player.Map_Num;
        byte Map_Item = Map.HasItem(Map_Num, Player.X, Player.Y);
        short Map_Item_Num = Lists.Temp_Map[Map_Num].Item[Map_Item].Item_Num;

        // Somente se necessário
        if (Map_Item == 0) return;

        // Dá o item ao jogador
        if (Player.GiveItem(Map_Item_Num, Lists.Temp_Map[Map_Num].Item[Map_Item].Amount))
        {
            // Retira o item do mapa
            Lists.Temp_Map[Map_Num].Item.RemoveAt(Map_Item);
            Send.Map_Items(Map_Num);
        }
    }

    private static void DropItem(Player Player, NetIncomingMessage Data)
    {
        Player.DropItem(Data.ReadByte(), Data.ReadInt16());
    }

    private static void Inventory_Change(Player Player, NetIncomingMessage Data)
    {
        byte Slot_Old = Data.ReadByte(), Slot_New = Data.ReadByte();
        byte Hotbar_Slot = Player.FindHotbar((byte)Game.Hotbar.Item, Slot_Old);

        // Somente se necessário
        if (Player.Inventory[Slot_Old].Item_Num == 0) return;
        if (Slot_Old == Slot_New) return;
        if (Player.Trade != null) return;

        // Muda o item de slot
        (Player.Inventory[Slot_Old].Item_Num, Player.Inventory[Slot_New].Item_Num) = (Player.Inventory[Slot_New].Item_Num, Player.Inventory[Slot_Old].Item_Num);
        (Player.Inventory[Slot_Old].Amount, Player.Inventory[Slot_New].Amount) = (Player.Inventory[Slot_New].Amount, Player.Inventory[Slot_Old].Amount);
        Player.Hotbar[Hotbar_Slot].Slot = Slot_New;
        Send.Player_Inventory(Player);
        Send.Player_Hotbar(Player);
    }

    private static void Inventory_Use(Player Player, NetIncomingMessage Data)
    {
        Player.UseItem(Data.ReadByte());
    }

    private static void Equipment_Remove(Player Player, NetIncomingMessage Data)
    {
        byte Slot = Data.ReadByte();
        short Map_Num = Player.Map_Num;
        Lists.Structures.Map_Items Map_Item = new Lists.Structures.Map_Items();

        // Apenas se necessário
        if (Player.Equipment[Slot] == 0) return;
        if (Lists.Item[Player.Equipment[Slot]].Bind == (byte)Game.BindOn.Equip) return;

        // Adiciona o equipamento ao inventário
        if (!Player.GiveItem(Player.Equipment[Slot], 1))
        {
            // Somente se necessário
            if (Lists.Temp_Map[Map_Num].Item.Count == Lists.Server_Data.Max_Map_Items) return;

            // Solta o item no chão
            Map_Item.Item_Num = Player.Equipment[Slot];
            Map_Item.Amount = 1;
            Map_Item.X = Player.X;
            Map_Item.Y = Player.Y;
            Lists.Temp_Map[Map_Num].Item.Add(Map_Item);

            // Envia os dados
            Send.Map_Items(Map_Num);
            Send.Player_Inventory(Player);
        }

        // Remove o equipamento
        for (byte i = 0; i < (byte)Game.Attributes.Count; i++) Player.Attribute[i] -= Lists.Item[Player.Equipment[Slot]].Equip_Attribute[i];
        Player.Equipment[Slot] = 0;

        // Envia os dados
        Send.Player_Equipments(Player);
    }

    private static void Hotbar_Add(Player Player, NetIncomingMessage Data)
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

    private static void Hotbar_Change(Player Player, NetIncomingMessage Data)
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

    private static void Hotbar_Use(Player Player, NetIncomingMessage Data)
    {
        byte Hotbar_Slot = Data.ReadByte();

        // Usa o item
        if (Player.Hotbar[Hotbar_Slot].Type == (byte)Game.Hotbar.Item)
            Player.UseItem(Player.Hotbar[Hotbar_Slot].Slot);
    }

    private static void Write_Server_Data(Account.Structure Account, NetIncomingMessage Data)
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

        // Salva os dados
        Write.Server_Data();
    }

    private static void Write_Classes(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de classes
        Lists.Class = new Lists.Structures.Class[Data.ReadByte()];
        Lists.Server_Data.Num_Classes = (byte)Lists.Class.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Class.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.Class[i] = new Lists.Structures.Class();
            Lists.Class[i].Vital = new short[(byte)Game.Vitals.Count];
            Lists.Class[i].Attribute = new short[(byte)Game.Attributes.Count];
            Lists.Class[i].Tex_Male = new short[Data.ReadByte()];
            Lists.Class[i].Tex_Female = new short[Data.ReadByte()];
            Lists.Class[i].Item = new System.Tuple<short, short>[Data.ReadByte()];

            // Lê os dados
            Lists.Class[i].Name = Data.ReadString();
            Lists.Class[i].Description = Data.ReadString();
            for (byte t = 0; t < Lists.Class[i].Tex_Male.Length; t++) Lists.Class[i].Tex_Male[t] = Data.ReadInt16();
            for (byte t = 0; t < Lists.Class[i].Tex_Female.Length; t++) Lists.Class[i].Tex_Female[t] = Data.ReadInt16();
            Lists.Class[i].Spawn_Map = Data.ReadInt16();
            Lists.Class[i].Spawn_Direction = Data.ReadByte();
            Lists.Class[i].Spawn_X = Data.ReadByte();
            Lists.Class[i].Spawn_Y = Data.ReadByte();
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Lists.Class[i].Vital[v] = Data.ReadInt16();
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Lists.Class[i].Attribute[a] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Lists.Class[i].Item.Length; n++) Lists.Class[i].Item[n] = new System.Tuple<short, short>(Data.ReadInt16(), Data.ReadInt16());
        }

        // Salva os dados e envia pra todos jogadores conectados
        Write.Classes();
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Classes(Lists.Account[i]);
    }

    private static void Write_Tiles(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de azulejos 
        Lists.Tile = new Lists.Structures.Tile[Data.ReadByte()];
        Lists.Server_Data.Num_Tiles = (byte)Lists.Tile.GetUpperBound(0);
        Write.Server_Data();

        for (byte i = 1; i < Lists.Tile.Length; i++)
        {
            // Dados básicos
            Lists.Tile[i] = new Lists.Structures.Tile();
            Lists.Tile[i].Width = Data.ReadByte();
            Lists.Tile[i].Height = Data.ReadByte();
            Lists.Tile[i].Data = new Lists.Structures.Tile_Data[Lists.Tile[i].Width + 1, Lists.Tile[i].Height + 1];

            for (byte x = 0; x <= Lists.Tile[i].Width; x++)
                for (byte y = 0; y <= Lists.Tile[i].Height; y++)
                {
                    // Atributos
                    Lists.Tile[i].Data[x, y] = new Lists.Structures.Tile_Data();
                    Lists.Tile[i].Data[x, y].Attribute = Data.ReadByte();
                    Lists.Tile[i].Data[x, y].Block = new bool[(byte)Game.Directions.Count];

                    // Bloqueio direcional
                    for (byte d = 0; d < (byte)Game.Directions.Count; d++) Lists.Tile[i].Data[x, y].Block[d] = Data.ReadBoolean();
                }
        }

        // Salva os dados e envia pra todos jogadores conectados
        Write.Tiles();
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Tiles(Lists.Account[i]);
    }

    private static void Write_Maps(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de mapas 
        Lists.Map = new Lists.Structures.Map[Data.ReadInt16()];
        Lists.Temp_Map = new Lists.Structures.Temp_Map[Lists.Map.Length];
        Lists.Server_Data.Num_Maps = (short)Lists.Map.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Map.Length; i++)
        {
            Lists.Map[i].Revision = Data.ReadInt16();
            Lists.Map[i].Name = Data.ReadString();
            Lists.Map[i].Width = Data.ReadByte();
            Lists.Map[i].Height = Data.ReadByte();
            Lists.Map[i].Moral = Data.ReadByte();
            Lists.Map[i].Panorama = Data.ReadByte();
            Lists.Map[i].Music = Data.ReadByte();
            Lists.Map[i].Color = Data.ReadInt32();
            Lists.Map[i].Weather.Type = Data.ReadByte();
            Lists.Map[i].Weather.Intensity = Data.ReadByte();
            Lists.Map[i].Fog.Texture = Data.ReadByte();
            Lists.Map[i].Fog.Speed_X = Data.ReadSByte();
            Lists.Map[i].Fog.Speed_Y = Data.ReadSByte();
            Lists.Map[i].Fog.Alpha = Data.ReadByte();
            Lists.Map[i].Light_Global = Data.ReadByte();
            Lists.Map[i].Lighting = Data.ReadByte();

            // Ligações
            Lists.Map[i].Link = new short[(short)Game.Directions.Count];
            for (short n = 0; n < (short)Game.Directions.Count; n++)
                Lists.Map[i].Link[n] = Data.ReadInt16();

            // Quantidade de camadas
            byte Num_Layers = Data.ReadByte();
            Lists.Map[i].Layer = new System.Collections.Generic.List<Lists.Structures.Map_Layer>();

            // Camadas
            for (byte n = 0; n <= Num_Layers; n++)
            {
                // Dados básicos
                Lists.Map[i].Layer.Add(new Lists.Structures.Map_Layer());
                Lists.Map[i].Layer[n].Name = Data.ReadString();
                Lists.Map[i].Layer[n].Type = Data.ReadByte();

                // Redimensiona os azulejos
                Lists.Map[i].Layer[n].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[i].Width + 1, Lists.Map[i].Height + 1];

                // Azulejos
                for (byte x = 0; x <= Lists.Map[i].Width; x++)
                    for (byte y = 0; y <= Lists.Map[i].Height; y++)
                    {
                        Lists.Map[i].Layer[n].Tile[x, y].X = Data.ReadByte();
                        Lists.Map[i].Layer[n].Tile[x, y].Y = Data.ReadByte();
                        Lists.Map[i].Layer[n].Tile[x, y].Tile = Data.ReadByte();
                        Lists.Map[i].Layer[n].Tile[x, y].Auto = Data.ReadBoolean();
                    }
            }

            // Dados específicos dos azulejos
            Lists.Map[i].Tile = new Lists.Structures.Map_Tile[Lists.Map[i].Width + 1, Lists.Map[i].Height + 1];
            for (byte x = 0; x <= Lists.Map[i].Width; x++)
                for (byte y = 0; y <= Lists.Map[i].Height; y++)
                {
                    Lists.Map[i].Tile[x, y].Attribute = Data.ReadByte();
                    Lists.Map[i].Tile[x, y].Data_1 = Data.ReadInt16();
                    Lists.Map[i].Tile[x, y].Data_2 = Data.ReadInt16();
                    Lists.Map[i].Tile[x, y].Data_3 = Data.ReadInt16();
                    Lists.Map[i].Tile[x, y].Data_4 = Data.ReadInt16();
                    Lists.Map[i].Tile[x, y].Zone = Data.ReadByte();
                    Lists.Map[i].Tile[x, y].Block = new bool[(byte)Game.Directions.Count];

                    for (byte n = 0; n < (byte)Game.Directions.Count; n++)
                        Lists.Map[i].Tile[x, y].Block[n] = Data.ReadBoolean();
                }

            // Luzes
            Lists.Map[i].Light = new Lists.Structures.Map_Light[Data.ReadByte()];
            for (byte n = 0; n < Lists.Map[i].Light.Length; n++)
            {
                Lists.Map[i].Light[n].X = Data.ReadByte();
                Lists.Map[i].Light[n].Y = Data.ReadByte();
                Lists.Map[i].Light[n].Width = Data.ReadByte();
                Lists.Map[i].Light[n].Height = Data.ReadByte();
            }

            // NPCs
            Lists.Map[i].NPC = new Lists.Structures.Map_NPC[Data.ReadByte()];
            Lists.Temp_Map[i].NPC = new NPC.Structure[Lists.Map[i].NPC.Length];
            for (byte n = 0; n < Lists.Map[i].NPC.Length; n++)
            {
                Lists.Map[i].NPC[n].Index = Data.ReadInt16();
                Lists.Map[i].NPC[n].Zone = Data.ReadByte();
                Lists.Map[i].NPC[n].Spawn = Data.ReadBoolean();
                Lists.Map[i].NPC[n].X = Data.ReadByte();
                Lists.Map[i].NPC[n].Y = Data.ReadByte();
                Lists.Temp_Map[i].NPC[n].Spawn();
            }

            // Itens
            Lists.Temp_Map[i].Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
            Lists.Temp_Map[i].Item.Add(new Lists.Structures.Map_Items());
            Map.Spawn_Items(i);

            // Envia o mapa para todos os jogadores que estão nele
            for (byte n = 0; n < Lists.Account.Count; n++)
                if (Lists.Account[n] != Account)
                    if (Lists.Account[n].Character.Map_Num == i || Lists.Account[n].InEditor)
                        Send.Map(Lists.Account[n], i);
        }

        // Salva os dados
        Write.Maps();
    }

    private static void Write_NPCs(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de npcs
        Lists.NPC = new Lists.Structures.NPC[Data.ReadInt16()];
        Lists.Server_Data.Num_NPCs = (byte)Lists.NPC.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.NPC.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.NPC[i] = new Lists.Structures.NPC();
            Lists.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
            Lists.NPC[i].Attribute = new short[(byte)Game.Attributes.Count];

            // Lê os dados
            Lists.NPC[i].Name = Data.ReadString();
            Lists.NPC[i].SayMsg = Data.ReadString();
            Lists.NPC[i].Texture = Data.ReadInt16();
            Lists.NPC[i].Behaviour = Data.ReadByte();
            Lists.NPC[i].SpawnTime = Data.ReadByte();
            Lists.NPC[i].Sight = Data.ReadByte();
            Lists.NPC[i].Experience = Data.ReadInt32();
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Lists.NPC[i].Vital[n] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Lists.NPC[i].Attribute[n] = Data.ReadInt16();
            Lists.NPC[i].Drop = new Lists.Structures.NPC_Drop[Data.ReadByte()];
            for (byte n = 0; n < Lists.NPC[i].Drop.Length; n++) Lists.NPC[i].Drop[n] = new Lists.Structures.NPC_Drop(Data.ReadInt16(), Data.ReadInt16(), Data.ReadByte());
            Lists.NPC[i].AttackNPC = Data.ReadBoolean();
            Lists.NPC[i].Allie = new short[Data.ReadByte()];
            for (byte n = 0; n < Lists.NPC[i].Allie.Length; n++) Lists.NPC[i].Allie[n] = Data.ReadInt16();
            Lists.NPC[i].Movement = (NPC.Movements)Data.ReadByte();
            Lists.NPC[i].Flee_Helth = Data.ReadByte();
            Lists.NPC[i].Shop = Data.ReadInt16();
        }

        // Salva os dados e envia pra todos jogadores conectados
        Write.NPCs();
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.NPCs(Lists.Account[i]);
    }

    private static void Write_Items(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de itens
        Lists.Item = new Lists.Structures.Item[Data.ReadInt16()];
        Lists.Server_Data.Num_Items = (byte)Lists.Item.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Item.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.Item[i] = new Lists.Structures.Item();
            Lists.Item[i].Potion_Vital = new short[(byte)Game.Vitals.Count];
            Lists.Item[i].Equip_Attribute = new short[(byte)Game.Attributes.Count];

            // Lê os dados
            Lists.Item[i].Name = Data.ReadString();
            Lists.Item[i].Description = Data.ReadString();
            Lists.Item[i].Texture = Data.ReadInt16();
            Lists.Item[i].Type = Data.ReadByte();
            Lists.Item[i].Stackable = Data.ReadBoolean();
            Lists.Item[i].Bind = Data.ReadByte();
            Lists.Item[i].Rarity = Data.ReadByte();
            Lists.Item[i].Req_Level = Data.ReadInt16();
            Lists.Item[i].Req_Class = Data.ReadByte();
            Lists.Item[i].Potion_Experience = Data.ReadInt32();
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Lists.Item[i].Potion_Vital[v] = Data.ReadInt16();
            Lists.Item[i].Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Lists.Item[i].Equip_Attribute[a] = Data.ReadInt16();
            Lists.Item[i].Weapon_Damage = Data.ReadInt16();
        }

        // Salva os dados e envia pra todos jogadores conectados
        Write.Items();
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Items(Lists.Account[i]);
    }

    private static void Write_Shops(Account.Structure Account, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Account, "You aren't allowed to do this.");
            return;
        }

        // Quantidade de lojas
        Lists.Shop = new Lists.Structures.Shop[Data.ReadInt16()];
        Lists.Server_Data.Num_Shops = (byte)Lists.Shop.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Shop.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.Shop[i] = new Lists.Structures.Shop();
            Lists.Shop[i].Sold = new Lists.Structures.Shop_Item[Data.ReadByte()];
            Lists.Shop[i].Bought = new Lists.Structures.Shop_Item[Data.ReadByte()];

            // Lê os dados
            Lists.Shop[i].Name = Data.ReadString();
            Lists.Shop[i].Currency = Data.ReadInt16();
            for (byte j = 0; j < Lists.Shop[i].Sold.Length; j++)
            {
                Lists.Shop[i].Sold[j] = new Lists.Structures.Shop_Item();
                Lists.Shop[i].Sold[j].Item_Num = Data.ReadInt16();
                Lists.Shop[i].Sold[j].Amount = Data.ReadInt16();
                Lists.Shop[i].Sold[j].Price = Data.ReadInt16();
            }
            for (byte j = 0; j < Lists.Shop[i].Bought.Length; j++)
            {
                Lists.Shop[i].Bought[j] = new Lists.Structures.Shop_Item();
                Lists.Shop[i].Bought[j].Item_Num = Data.ReadInt16();
                Lists.Shop[i].Bought[j].Amount = Data.ReadInt16();
                Lists.Shop[i].Bought[j].Price = Data.ReadInt16();
            }
        }

        // Salva os dados e envia pra todos jogadores conectados
        Write.Shops();
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i] != Account)
                Send.Shops(Lists.Account[i]);
    }

    private static void Request_Server_Data(Account.Structure Account)
    {
        Send.Server_Data(Account);
    }

    private static void Request_Classes(Account.Structure Account)
    {
        Send.Classes(Account);
    }

    private static void Request_Tiles(Account.Structure Account)
    {
        Send.Tiles(Account);
    }

    private static void Request_Map(Account.Structure Account, NetIncomingMessage Data)
    {
        Send.Map(Account, Data.ReadInt16());
    }

    private static void Request_Maps(Account.Structure Account, NetIncomingMessage Data)
    {
        Send.Maps(Account, Data.ReadBoolean());
    }

    private static void Request_NPCs(Account.Structure Account)
    {
        Send.NPCs(Account);
    }

    private static void Request_Items(Account.Structure Account)
    {
        Send.Items(Account);
    }

    private static void Request_Shops(Account.Structure Account)
    {
        Send.Shops(Account);
    }

    private static void Party_Invite(Player Player, NetIncomingMessage Data)
    {
        string Name = Data.ReadString();

        // Encontra o jogador
        Player Invited = Account.FindPlayer(Name);

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

    private static void Party_Accept(Player Player)
    {
        Player Invitation = Account.FindPlayer(Player.Party_Request);

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

    private static void Party_Decline(Player Player)
    {
        Player Invitation = Account.FindPlayer(Player.Party_Request);

        // Recusa o convite
        if (Invitation != null) Send.Message(Invitation, Player.Name + " decline the party.", System.Drawing.Color.White);
        Player.Party_Request = string.Empty;
    }

    private static void Party_Leave(Player Player)
    {
        // Sai do grupo
        Player.Party_Leave();
    }

    private static void Trade_Invite(Player Player, NetIncomingMessage Data)
    {
        string Name = Data.ReadString();

        // Encontra o jogador
        Player Invited = Account.FindPlayer(Name);

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
        if (Player.Shop > 0)
        {
            Send.Message(Player, "You can't start a trade while in the shop.", System.Drawing.Color.White);
            return;
        }
        if (Invited.Shop > 0)
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

    private static void Trade_Accept(Player Player)
    {
        Player Invited = Account.FindPlayer(Player.Trade_Request);

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
        if (Invited.Shop > 0)
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
        Player.Trade_Offer = new Lists.Structures.Inventories[Game.Max_Inventory + 1];
        Invited.Trade_Offer = new Lists.Structures.Inventories[Game.Max_Inventory + 1];

        // Envia os dados para o grupo
        Send.Trade(Player, true);
        Send.Trade(Invited, true);
    }

    private static void Trade_Decline(Player Player)
    {
        Player Invited = Account.FindPlayer(Player.Trade_Request);

        // Recusa o convite
        if (Invited != null) Send.Message(Invited, Player.Name + " decline the trade.", System.Drawing.Color.White);
        Player.Trade_Request = string.Empty;
    }

    private static void Trade_Leave(Player Player)
    {
        Player.Trade_Leave();
    }

    private static void Trade_Offer(Player Player, NetIncomingMessage Data)
    {
        byte Slot = Data.ReadByte(), Inventory_Slot = Data.ReadByte();
        short Amount = System.Math.Min(Data.ReadInt16(), Player.Inventory[Inventory_Slot].Amount);

        // Adiciona o item à troca
        if (Inventory_Slot != 0)
        {
            // Evita itens repetidos
            for (byte i = 1; i <= Game.Max_Inventory; i++)
                if (Player.Trade_Offer[i].Item_Num == Inventory_Slot)
                    return;

            Player.Trade_Offer[Slot].Item_Num = Inventory_Slot;
            Player.Trade_Offer[Slot].Amount = Amount;
        }
        // Remove o item da troca
        else
            Player.Trade_Offer[Slot] = new Lists.Structures.Inventories();

        // Envia os dados ao outro jogador
        Send.Trade_Offer(Player);
        Send.Trade_Offer(Player.Trade, false);
    }

    private static void Trade_Offer_State(Player Player, NetIncomingMessage Data)
    {
        Game.Trade_Status State = (Game.Trade_Status)Data.ReadByte();
        Player Invited = Player.Trade;

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
                Player To = Player;
                for (byte j = 0; j < 2; j++, To = To == Player ? Invited : Player)
                    for (byte i = 1; i <= Game.Max_Inventory; i++)
                        To.TakeItem((byte)To.Trade_Offer[i].Item_Num, To.Trade_Offer[i].Amount);

                // Dá os itens aos jogadores
                for (byte i = 1; i <= Game.Max_Inventory; i++)
                {
                    if (Player.Trade_Offer[i].Item_Num > 0) Invited.GiveItem(Your_Inventory[Player.Trade_Offer[i].Item_Num].Item_Num, Player.Trade_Offer[i].Amount);
                    if (Invited.Trade_Offer[i].Item_Num > 0) Player.GiveItem(Their_Inventory[Invited.Trade_Offer[i].Item_Num].Item_Num, Invited.Trade_Offer[i].Amount);
                }

                // Envia os dados do inventário aos jogadores
                Send.Player_Inventory(Player);
                Send.Player_Inventory(Invited);

                // Limpa a troca
                Player.Trade_Offer = new Lists.Structures.Inventories[Game.Max_Inventory + 1];
                Invited.Trade_Offer = new Lists.Structures.Inventories[Game.Max_Inventory + 1];
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

    private static void Shop_Buy(Player Player, NetIncomingMessage Data)
    {
        Lists.Structures.Shop_Item Shop_Sold = Lists.Shop[Player.Shop].Sold[Data.ReadByte()];
        byte Inventory_Slot = Player.FindInventory(Lists.Shop[Player.Shop].Currency);

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
        Player.GiveItem(Shop_Sold.Item_Num, Shop_Sold.Amount);
        Send.Message(Player, "You bought " + Shop_Sold.Price + "x " + Lists.Item[Shop_Sold.Item_Num].Name + ".", System.Drawing.Color.Green);
    }

    private static void Shop_Sell(Player Player, NetIncomingMessage Data)
    {
        byte Inventory_Slot = Data.ReadByte();
        short Amount = System.Math.Min(Data.ReadInt16(), Player.Inventory[Inventory_Slot].Amount);
        Lists.Structures.Shop_Item Buy = Lists.Shop[Player.Shop].BoughtItem(Player.Inventory[Inventory_Slot].Item_Num);

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
        Player.TakeItem(Inventory_Slot, Amount);
        Player.GiveItem(Lists.Shop[Player.Shop].Currency, (short)(Buy.Price * Amount));
        Send.Message(Player, "You sold " + Lists.Item[Inventory_Slot].Name + "x " + Amount + "for .", System.Drawing.Color.Green);
    }

    private static void Shop_Close(Player Player)
    {
        Player.Shop = 0;
    }

    private static void Warp(Player Player, NetIncomingMessage Data)
    {
        // Verifica se o jogador realmente tem permissão 
        if (Player.Account.Acess < Game.Accesses.Editor)
        {
            Send.Alert(Player.Account, "You aren't allowed to do this.");
            return;
        }

        // Teletransporta o jogador para o mapa
        Player.Warp(Data.ReadInt16(), Data.ReadByte(), Data.ReadByte());
    }
}