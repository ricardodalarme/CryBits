using Lidgren.Network;
using System.IO;

class Receive
{
    // Pacotes do cliente
    public enum Client_Packets
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
        Hotbar_Use
    }

    // Pacotes do editor
    public enum Editor_Packets
    {
        Connect,
        Write_Server_Data,
        Write_Classes,
        Write_Tiles,
        Write_Map,
        Write_NPCs,
        Write_Items,
        Request_Server_Data,
        Request_Classes,
        Request_Tiles,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items
    }

    public static void Handle(byte Index, NetIncomingMessage Data)
    {
        byte Packet_Num = Data.ReadByte();

        // Pacote principal de conexão
        if (Packet_Num == 0) Connect(Index, Data);
        else if (!Lists.TempPlayer[Index].InEditor)
            // Manuseia os dados recebidos do cliente
            switch ((Client_Packets)Packet_Num)
            {
                case Client_Packets.Latency: Latency(Index); break;
                case Client_Packets.Register: Register(Index, Data); break;
                case Client_Packets.CreateCharacter: CreateCharacter(Index, Data); break;
                case Client_Packets.Character_Use: Character_Use(Index, Data); break;
                case Client_Packets.Character_Create: Character_Create(Index); break;
                case Client_Packets.Character_Delete: Character_Delete(Index, Data); break;
                case Client_Packets.Player_Direction: Player_Direction(Index, Data); break;
                case Client_Packets.Player_Move: Player_Move(Index, Data); break;
                case Client_Packets.Player_Attack: Player_Attack(Index); break;
                case Client_Packets.RequestMap: RequestMap(Index, Data); break;
                case Client_Packets.Message: Message(Index, Data); break;
                case Client_Packets.AddPoint: AddPoint(Index, Data); break;
                case Client_Packets.CollectItem: CollectItem(Index); break;
                case Client_Packets.DropItem: DropItem(Index, Data); break;
                case Client_Packets.Inventory_Change: Inventory_Change(Index, Data); break;
                case Client_Packets.Inventory_Use: Inventory_Use(Index, Data); break;
                case Client_Packets.Equipment_Remove: Equipment_Remove(Index, Data); break;
                case Client_Packets.Hotbar_Add: Hotbar_Add(Index, Data); break;
                case Client_Packets.Hotbar_Change: Hotbar_Change(Index, Data); break;
                case Client_Packets.Hotbar_Use: Hotbar_Use(Index, Data); break;
            }
        else
            // Manuseia os dados recebidos do editor
            switch ((Editor_Packets)Packet_Num)
            {
                case Editor_Packets.Write_Server_Data: Write_Server_Data(Index, Data); break;
                case Editor_Packets.Write_Classes: Write_Classes(Index, Data); break;
                case Editor_Packets.Write_Tiles: Write_Tiles(Index, Data); break;
                case Editor_Packets.Write_Map: Write_Maps(Index, Data); break;
                case Editor_Packets.Write_NPCs: Write_NPCs(Index, Data); break;
                case Editor_Packets.Write_Items: Write_Items(Index, Data); break;
                case Editor_Packets.Request_Server_Data: Request_Server_Data(Index, Data); break;
                case Editor_Packets.Request_Classes: Request_Classes(Index, Data); break;
                case Editor_Packets.Request_Tiles: Request_Tiles(Index, Data); break;
                case Editor_Packets.Request_Map: Request_Map(Index, Data); break;
                case Editor_Packets.Request_Maps: Request_Maps(Index, Data); break;
                case Editor_Packets.Request_NPCs: Request_NPCs(Index, Data); break;
                case Editor_Packets.Request_Items: Request_Items(Index, Data); break;
            }
    }

    private static void Latency(byte Index)
    {
        // Envia o pacote para a contagem da latência
        Send.Latency(Index);
    }

    private static void Connect(byte Index, NetIncomingMessage Data)
    {
        // Lê os dados
        string User = Data.ReadString().Trim();
        string Password = Data.ReadString();
        bool Editor = Data.ReadBoolean();

        // Verifica se está tudo certo
        if (!File.Exists(Directories.Accounts.FullName + User + Directories.Format))
        {
            Send.Alert(Index, "This username isn't registered.");
            return;
        }
        if (Player.MultipleAccounts(User))
        {
            Send.Alert(Index, "Someone already signed in to this account.");
            return;
        }
        if (Password != Read.Player_Password(User))
        {
            Send.Alert(Index, "Password is incorrect.");
            return;
        }

        if (Editor)
        {
            // Carrega somente os dados importantes do jogador
            Read.Player(Index, User, false);

            // Verifica se o jogador tem permissão para fazer entrar no modo edição
            if (Lists.Player[Index].Acess < Game.Accesses.Editor)
            {
                Clear.Player(Index);
                Send.Alert(Index, "You're not allowed to do this.");
                return;
            }

            // Abre a janela de edição
            Lists.TempPlayer[Index].InEditor = true;
            Send.Connect(Index);
        }
        else
        {
            // Carrega os dados do jogador
            Read.Player(Index, User);

            // Envia os dados das classes
            Send.Classes(Index);

            // Se o jogador não tiver nenhum personagem então abrir o painel de criação de personagem
            if (!Player.HasCharacter(Index))
            {
                Send.CreateCharacter(Index);
                return;
            }

            // Abre a janela de seleção de personagens
            Send.Characters(Index);
            Send.Connect(Index);
        }
    }

    private static void Register(byte Index, NetIncomingMessage Data)
    {
        // Lê os dados
        string User = Data.ReadString().Trim();
        string Password = Data.ReadString();

        // Verifica se está tudo certo
        if (User.Length < Game.Min_Name_Length || User.Length > Game.Max_Name_Length || Password.Length < Game.Min_Name_Length || Password.Length > Game.Max_Name_Length)
        {
            Send.Alert(Index, "The username and password must contain between " + Game.Min_Name_Length + " and " + Game.Max_Name_Length + " characters.");
            return;
        }
        if (File.Exists(Directories.Accounts.FullName + User + Directories.Format))
        {
            Send.Alert(Index, "There is already someone registered with this name.");
            return;
        }

        // Cria a conta
        Lists.Player[Index].User = User;
        Lists.Player[Index].Password = Password;

        // Salva a conta
        Write.Player(Index);

        // Abre a janela de seleção de personagens
        Send.Classes(Index);
        Send.CreateCharacter(Index);
    }

    private static void CreateCharacter(byte Index, NetIncomingMessage Data)
    {
        byte Character = Player.FindCharacter(Index, string.Empty);

        // Lê os dados
        string Name = Data.ReadString().Trim();

        // Verifica se está tudo certo
        if (Name.Length < Game.Min_Name_Length || Name.Length > Game.Max_Name_Length)
        {
            Send.Alert(Index, "The character name must contain between " + Game.Min_Name_Length + " and " + Game.Max_Name_Length + " characters.", false);
            return;
        }
        if (Name.Contains(";") || Name.Contains(":"))
        {
            Send.Alert(Index, "Can't contain ';' and ':' in the character name.", false);
            return;
        }
        if (Read.Characters_Name().Contains(";" + Name + ":"))
        {
            Send.Alert(Index, "A character with this name already exists", false);
            return;
        }

        // Define o personagem que será usado
        Lists.TempPlayer[Index].Using = Character;

        // Define os valores iniciais do personagem
        Player.Character(Index).Name = Name;
        Player.Character(Index).Level = 1;
        Player.Character(Index).Class = Data.ReadByte();
        Player.Character(Index).Genre = Data.ReadBoolean();
        Player.Character(Index).Attribute = Lists.Class[Player.Character(Index).Class].Attribute;
        Player.Character(Index).Map = Lists.Class[Player.Character(Index).Class].Spawn_Map;
        Player.Character(Index).Direction = (Game.Directions)Lists.Class[Player.Character(Index).Class].Spawn_Direction;
        Player.Character(Index).X = Lists.Class[Player.Character(Index).Class].Spawn_X;
        Player.Character(Index).Y = Lists.Class[Player.Character(Index).Class].Spawn_Y;
        for (byte i = 0; i < (byte)Game.Vitals.Amount; i++) Player.Character(Index).Vital[i] = Player.Character(Index).MaxVital(i);

        // Salva a conta
        Write.Character(Name);
        Write.Player(Index);

        // Entra no jogo
        Player.Join(Index);
    }

    private static void Character_Use(byte Index, NetIncomingMessage Data)
    {
        // Define o personagem que será usado
        Lists.TempPlayer[Index].Using = Data.ReadByte();

        // Entra no jogo
        Player.Join(Index);
    }

    private static void Character_Create(byte Index)
    {
        // Verifica se o jogador já criou o máximo de personagens possíveis
        if (Player.FindCharacter(Index, string.Empty) == 0)
        {
            Send.Alert(Index, "You can only have " + Lists.Server_Data.Max_Characters + " characters.", false);
            return;
        }

        // Abre a janela de seleção de personagens
        Send.Classes(Index);
        Send.CreateCharacter(Index);
    }

    private static void Character_Delete(byte Index, NetIncomingMessage Data)
    {
        byte Character = Data.ReadByte();
        string Name = Lists.Player[Index].Character[Character].Name;

        // Verifica se o personagem existe
        if (string.IsNullOrEmpty(Name)) return;

        // Deleta o personagem
        Send.Alert(Index, "The character '" + Name + "' has been deleted.", false);
        Write.Characters(Read.Characters_Name().Replace(":;" + Name + ":", ":"));
        Clear.Player_Character(Index, Character);

        // Salva o personagem
        Send.Characters(Index);
        Write.Player(Index);
    }

    private static void Player_Direction(byte Index, NetIncomingMessage Data)
    {
        Game.Directions Direction = (Game.Directions)Data.ReadByte();

        // Previne erros
        if (Direction < Game.Directions.Up || Direction > Game.Directions.Right) return;
        if (Lists.TempPlayer[Index].GettingMap) return;

        // Defini a direção do jogador
        Player.Character(Index).Direction = Direction;
        Send.Player_Direction(Index);
    }

    private static void Player_Move(byte Index, NetIncomingMessage Data)
    {
        byte X = Data.ReadByte(), Y = Data.ReadByte();

        // Move o jogador se necessário
        if (Player.Character(Index).X != X || Player.Character(Index).Y != Y)
            Send.Player_Position(Index);
        else
            Player.Move(Index, Data.ReadByte());
    }

    private static void RequestMap(byte Index, NetIncomingMessage Data)
    {
        // Se necessário enviar as informações do mapa ao jogador
        if (Data.ReadBoolean()) Send.Map(Index, Player.Character(Index).Map);

        // Envia a informação aos outros jogadores
        Send.Map_Players(Index);

        // Entra no mapa
        Lists.TempPlayer[Index].GettingMap = false;
        Send.JoinMap(Index);
    }

    private static void Message(byte Index, NetIncomingMessage Data)
    {
        string Message = Data.ReadString();

        // Evita caracteres inválidos
        for (byte i = 0; i >= Message.Length; i++)
            if ((Message[i] < 32 && Message[i] > 126))
                return;

        // Envia a mensagem para os outros jogadores
        switch ((Game.Messages)Data.ReadByte())
        {
            case Game.Messages.Map: Send.Message_Map(Index, Message); break;
            case Game.Messages.Global: Send.Message_Global(Index, Message); break;
            case Game.Messages.Private: Send.Message_Private(Index, Data.ReadString(), Message); break;
        }
    }

    private static void Player_Attack(byte Index)
    {
        // Ataca
        Player.Attack(Index);
    }

    private static void AddPoint(byte Index, NetIncomingMessage Data)
    {
        byte Attribute_Num = Data.ReadByte();

        // Adiciona um ponto a determinado atributo
        if (Player.Character(Index).Points > 0)
        {
            Player.Character(Index).Attribute[Attribute_Num] += 1;
            Player.Character(Index).Points -= 1;
            Send.Player_Experience(Index);
            Send.Map_Players(Index);
        }
    }

    private static void CollectItem(byte Index)
    {
        short Map_Num = Player.Character(Index).Map;
        byte Map_Item = Map.HasItem(Map_Num, Player.Character(Index).X, Player.Character(Index).Y);
        short Map_Item_Num = Lists.Map[Map_Num].Temp_Item[Map_Item].Index;

        // Somente se necessário
        if (Map_Item == 0) return;

        // Dá o item ao jogador
        if (Player.GiveItem(Index, Map_Item_Num, Lists.Map[Map_Num].Temp_Item[Map_Item].Amount))
        {
            // Retira o item do mapa
            Lists.Map[Map_Num].Temp_Item.RemoveAt(Map_Item);
            Send.Map_Items(Map_Num);
        }
    }

    private static void DropItem(byte Index, NetIncomingMessage Data)
    {
        Player.DropItem(Index, Data.ReadByte());
    }

    private static void Inventory_Change(byte Index, NetIncomingMessage Data)
    {
        byte Slot_Old = Data.ReadByte(), Slot_New = Data.ReadByte();
        byte Hotbar_Old = Player.FindHotbar(Index, (byte)Game.Hotbar.Item, Slot_Old), Hotbar_New = Player.FindHotbar(Index, (byte)Game.Hotbar.Item, Slot_New);
        Lists.Structures.Inventories Old = Player.Character(Index).Inventory[Slot_Old];

        // Somente se necessário
        if (Player.Character(Index).Inventory[Slot_Old].Item_Num == 0) return;
        if (Slot_Old == Slot_New) return;

        // Caso houver um item no novo slot, trocar ele para o velho
        if (Player.Character(Index).Inventory[Slot_New].Item_Num > 0)
        {
            // Inventário
            Player.Character(Index).Inventory[Slot_Old].Item_Num = Player.Character(Index).Inventory[Slot_New].Item_Num;
            Player.Character(Index).Inventory[Slot_Old].Amount = Player.Character(Index).Inventory[Slot_New].Amount;
            Player.Character(Index).Hotbar[Hotbar_New].Slot = Slot_Old;
        }
        else
        {
            Player.Character(Index).Inventory[Slot_Old].Item_Num = 0;
            Player.Character(Index).Inventory[Slot_Old].Amount = 0;
        }

        // Muda o item de slot
        Player.Character(Index).Inventory[Slot_New].Item_Num = Old.Item_Num;
        Player.Character(Index).Inventory[Slot_New].Amount = Old.Amount;
        Player.Character(Index).Hotbar[Hotbar_Old].Slot = Slot_New;
        Send.Player_Inventory(Index);
        Send.Player_Hotbar(Index);
    }

    private static void Inventory_Use(byte Index, NetIncomingMessage Data)
    {
        Player.UseItem(Index, Data.ReadByte());
    }

    private static void Equipment_Remove(byte Index, NetIncomingMessage Data)
    {
        byte Slot = Data.ReadByte();
        short Map_Num = Player.Character(Index).Map;
        Lists.Structures.Map_Items Map_Item = new Lists.Structures.Map_Items();

        // Apenas se necessário
        if (Player.Character(Index).Equipment[Slot] == 0) return;

        // Adiciona o equipamento ao inventário
        if (!Player.GiveItem(Index, Player.Character(Index).Equipment[Slot], 1))
        {
            // Somente se necessário
            if (Lists.Map[Map_Num].Temp_Item.Count == Game.Max_Map_Items) return;

            // Solta o item no chão
            Map_Item.Index = Player.Character(Index).Equipment[Slot];
            Map_Item.Amount = 1;
            Map_Item.X = Player.Character(Index).X;
            Map_Item.Y = Player.Character(Index).Y;
            Lists.Map[Map_Num].Temp_Item.Add(Map_Item);

            // Envia os dados
            Send.Map_Items(Map_Num);
            Send.Player_Inventory(Index);
        }

        // Remove o equipamento
        for (byte i = 0; i < (byte)Game.Attributes.Amount; i++) Player.Character(Index).Attribute[i] -= Lists.Item[Player.Character(Index).Equipment[Slot]].Equip_Attribute[i];
        Player.Character(Index).Equipment[Slot] = 0;

        // Envia os dados
        Send.Player_Equipments(Index);
    }

    private static void Hotbar_Add(byte Index, NetIncomingMessage Data)
    {
        byte Hotbar_Slot = Data.ReadByte();
        byte Type = Data.ReadByte();
        byte Slot = Data.ReadByte();

        // Somente se necessário
        if (Slot != 0 && Player.FindHotbar(Index, Type, Slot) > 0) return;

        // Define os dados
        Player.Character(Index).Hotbar[Hotbar_Slot].Slot = Slot;
        Player.Character(Index).Hotbar[Hotbar_Slot].Type = Type;

        // Envia os dados
        Send.Player_Hotbar(Index);
    }

    private static void Hotbar_Change(byte Index, NetIncomingMessage Data)
    {
        byte Slot_Old = Data.ReadByte(), Slot_New = Data.ReadByte();
        Lists.Structures.Hotbar Old = Player.Character(Index).Hotbar[Slot_Old];

        // Somente se necessário
        if (Player.Character(Index).Hotbar[Slot_Old].Slot == 0) return;
        if (Slot_Old == Slot_New) return;

        // Caso houver um item no novo slot, trocar ele para o velho
        if (Player.Character(Index).Hotbar[Slot_New].Slot > 0)
        {
            Player.Character(Index).Hotbar[Slot_Old].Slot = Player.Character(Index).Hotbar[Slot_New].Slot;
            Player.Character(Index).Hotbar[Slot_Old].Type = Player.Character(Index).Hotbar[Slot_New].Type;
        }
        else
        {
            Player.Character(Index).Hotbar[Slot_Old].Slot = 0;
            Player.Character(Index).Hotbar[Slot_Old].Type = 0;
        }

        // Muda o item de slot
        Player.Character(Index).Hotbar[Slot_New].Slot = Old.Slot;
        Player.Character(Index).Hotbar[Slot_New].Type = Old.Type;
        Send.Player_Hotbar(Index);
    }

    private static void Hotbar_Use(byte Index, NetIncomingMessage Data)
    {
        byte Hotbar_Slot = Data.ReadByte();

        // Usa o item
        if (Player.Character(Index).Hotbar[Hotbar_Slot].Type == (byte)Game.Hotbar.Item)
            Player.UseItem(Index, Player.Character(Index).Hotbar[Hotbar_Slot].Slot);
    }

    private static void Write_Server_Data(byte Index, NetIncomingMessage Data)
    {
        // Altera os dados
        Lists.Server_Data.Game_Name = Data.ReadString();
        Lists.Server_Data.Welcome = Data.ReadString();
        Lists.Server_Data.Port = Data.ReadInt16();
        Lists.Server_Data.Max_Players = Data.ReadByte();
        Lists.Server_Data.Max_Characters = Data.ReadByte();

        // Salva os dados
        Write.Server_Data();
    }

    private static void Write_Classes(byte Index, NetIncomingMessage Data)
    {
        // Quantidade de classes
        Lists.Class = new Lists.Structures.Class[Data.ReadByte()];
        Lists.Server_Data.Num_Classes = (byte)Lists.Class.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Class.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.Class[i].Vital = new short[(byte)Game.Vitals.Amount];
            Lists.Class[i].Attribute = new short[(byte)Game.Attributes.Amount];

            // Lê os dados
            Lists.Class[i].Name = Data.ReadString();
            Lists.Class[i].Texture_Male = Data.ReadInt16();
            Lists.Class[i].Texture_Female = Data.ReadInt16();
            Lists.Class[i].Spawn_Map = Data.ReadInt16();
            Lists.Class[i].Spawn_Direction = Data.ReadByte();
            Lists.Class[i].Spawn_X = Data.ReadByte();
            Lists.Class[i].Spawn_Y = Data.ReadByte();
            for (byte v = 0; v < (byte)Game.Vitals.Amount; v++) Lists.Class[i].Vital[v] = Data.ReadInt16();
            for (byte a = 0; a < (byte)Game.Attributes.Amount; a++) Lists.Class[i].Attribute[a] = Data.ReadInt16();
        }

        // Salva os dados
        Write.Classes();
    }

    private static void Write_Tiles(byte Index, NetIncomingMessage Data)
    {
        // Quantidade de tiles 
        Lists.Tile = new Lists.Structures.Tile[Data.ReadByte()];
        Lists.Server_Data.Num_Tiles = (byte)Lists.Tile.GetUpperBound(0);
        Write.Server_Data();

        for (byte i = 1; i < Lists.Tile.Length; i++)
        {
            // Dados básicos
            Lists.Tile[i].Width = Data.ReadByte();
            Lists.Tile[i].Height = Data.ReadByte();
            Lists.Tile[i].Data = new Lists.Structures.Tile_Data[Lists.Tile[i].Width + 1, Lists.Tile[i].Height + 1];

            for (byte x = 0; x <= Lists.Tile[i].Width; x++)
                for (byte y = 0; y <= Lists.Tile[i].Height; y++)
                {
                    // Atributos
                    Lists.Tile[i].Data[x, y].Attribute = Data.ReadByte();

                    // Bloqueio direcional
                    for (byte d = 0; d < (byte)Game.Directions.Amount; d++)
                    {
                        Lists.Tile[i].Data[x, y].Block = new bool[(byte)Game.Directions.Amount];
                        Lists.Tile[i].Data[x, y].Block[d] = Data.ReadBoolean();
                    }
                }
        }

        // Salva os dados
        Write.Tiles();
    }

    private static void Write_Maps(byte Index, NetIncomingMessage Data)
    {

        // Salva os dados
        Write.Maps();
    }

    private static void Write_NPCs(byte Index, NetIncomingMessage Data)
    {
        // Quantidade de npcs
        Lists.NPC = new Lists.Structures.NPC[Data.ReadInt16()];
        Lists.Server_Data.Num_NPCs = (byte)Lists.NPC.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.NPC.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.NPC[i].Vital = new short[(byte)Game.Vitals.Amount];
            Lists.NPC[i].Attribute = new short[(byte)Game.Attributes.Amount];
            Lists.NPC[i].Drop = new Lists.Structures.NPC_Drop[Game.Max_NPC_Drop];

            // Lê os dados
            Lists.NPC[i].Name = Data.ReadString();
            Lists.NPC[i].Texture = Data.ReadInt16();
            Lists.NPC[i].Behaviour = Data.ReadByte();
            Lists.NPC[i].SpawnTime = Data.ReadByte();
            Lists.NPC[i].Sight = Data.ReadByte();
            Lists.NPC[i].Experience = Data.ReadByte();
            for (byte n = 0; n < (byte)Game.Vitals.Amount; n++) Lists.NPC[i].Vital[n] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Game.Attributes.Amount; n++) Lists.NPC[i].Attribute[n] = Data.ReadInt16();
            for (byte n = 0; n < Game.Max_NPC_Drop; n++)
            {
                Lists.NPC[i].Drop[n].Item_Num = Data.ReadInt16();
                Lists.NPC[i].Drop[n].Amount = Data.ReadInt16();
                Lists.NPC[i].Drop[n].Chance = Data.ReadByte();
            }
        }

        // Salva os dados
        Write.NPCs();
    }

    private static void Write_Items(byte Index, NetIncomingMessage Data)
    {
        // Quantidade de itens
        Lists.Item = new Lists.Structures.Item[Data.ReadInt16()];
        Lists.Server_Data.Num_Items = (byte)Lists.Item.GetUpperBound(0);
        Write.Server_Data();

        for (short i = 1; i < Lists.Item.Length; i++)
        {
            // Redimensiona os valores necessários 
            Lists.Item[i].Potion_Vital = new short[(byte)Game.Vitals.Amount];
            Lists.Item[i].Equip_Attribute = new short[(byte)Game.Attributes.Amount];

            // Lê os dados
            Lists.Item[i].Name = Data.ReadString();
            Lists.Item[i].Description = Data.ReadString();
            Lists.Item[i].Texture = Data.ReadInt16();
            Lists.Item[i].Type = Data.ReadByte();
            Lists.Item[i].Price = Data.ReadInt16();
            Lists.Item[i].Stackable = Data.ReadBoolean();
            Lists.Item[i].Bind = Data.ReadBoolean();
            Lists.Item[i].Req_Level = Data.ReadInt16();
            Lists.Item[i].Req_Class = Data.ReadByte();
            Lists.Item[i].Potion_Experience = Data.ReadInt16();
            for (byte v = 0; v < (byte)Game.Vitals.Amount; v++) Lists.Item[i].Potion_Vital[v] = Data.ReadInt16();
            Lists.Item[i].Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Game.Attributes.Amount; a++) Lists.Item[i].Equip_Attribute[a] = Data.ReadInt16();
            Lists.Item[i].Weapon_Damage = Data.ReadInt16();
        }

        // Salva os dados
        Write.Items();
    }

    private static void Request_Server_Data(byte Index, NetIncomingMessage Data)
    {
        Send.Server_Data(Index, Data.ReadBoolean());
    }

    private static void Request_Classes(byte Index, NetIncomingMessage Data)
    {
        Send.Classes(Index, Data.ReadBoolean());
    }

    private static void Request_Tiles(byte Index, NetIncomingMessage Data)
    {
        Send.Tiles(Index, Data.ReadBoolean());
    }

    private static void Request_Map(byte Index, NetIncomingMessage Data)
    {
        Send.Map(Index, Data.ReadInt16());
    }

    private static void Request_Maps(byte Index, NetIncomingMessage Data)
    {
        Send.Maps(Index, Data.ReadBoolean());
    }

    private static void Request_NPCs(byte Index, NetIncomingMessage Data)
    {
        Send.NPCs(Index, Data.ReadBoolean());
    }

    private static void Request_Items(byte Index, NetIncomingMessage Data)
    {
        Send.Items(Index, Data.ReadBoolean());
    }
}