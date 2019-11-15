using Lidgren.Network;
using System.Drawing;

partial class Send
{
    // Pacotes do servidor
    public enum Packets
    {
        Alert,
        Connect,
        CreateCharacter,
        Join,
        Classes,
        Characters,
        JoinGame,
        HigherIndex,
        Player_Data,
        Player_Position,
        Player_Vitals,
        Player_Leave,
        Player_Attack,
        Player_Move,
        Player_Direction,
        Player_Experience,
        Player_Inventory,
        Player_Equipments,
        Player_Hotbar,
        JoinMap,
        Map_Revision,
        Map,
        Latency,
        Message,
        NPCs,
        Map_NPCs,
        Map_NPC,
        Map_NPC_Movement,
        Map_NPC_Direction,
        Map_NPC_Vitals,
        Map_NPC_Attack,
        Map_NPC_Died,
        Items,
        Map_Items,
    }

    public static void ToPlayer(byte Index, NetOutgoingMessage Data)
    {
        // Previne sobrecarga
        if (!Socket.IsConnected(Index)) return;

        // Recria o pacote e o envia
        NetOutgoingMessage Data_Send = Socket.Device.CreateMessage(Data.LengthBytes);
        Data_Send.Write(Data);
        Socket.Device.SendMessage(Data_Send, Socket.Connection[Index], NetDeliveryMethod.ReliableOrdered);
    }

    public static void ToAll(NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                ToPlayer(i, Data);
    }

    public static void ToAllBut(byte Index, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Index != i)
                    ToPlayer(i, Data);
    }

    public static void ToMap(short Map, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Player.Character(i).Map == Map)
                    ToPlayer(i, Data);
    }

    public static void ToMapBut(short Map, byte Index, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Player.Character(i).Map == Map)
                    if (Index != i)
                        ToPlayer(i, Data);
    }

    public static void Alert(byte Index, string Message, bool Disconnect = true)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Alert);
        Data.Write(Message);
        ToPlayer(Index, Data);

        // Desconecta o jogador
        if (Disconnect)
            Socket.Connection[Index].Disconnect(string.Empty);
    }

    public static void Message(string Message)
    {
        // Envia o alerta para todos
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Socket.Connection[i] != null)
                if (Socket.Connection[i].Status == NetConnectionStatus.Connected)
                    Alert(i, Message);
    }

    public static void Connect(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Connect);
        Data.Write(Index);
        ToPlayer(Index, Data);
    }

    public static void CreateCharacter(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.CreateCharacter);
        ToPlayer(Index, Data);
    }

    public static void Join(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Join);
        Data.Write(Index);
        Data.Write(Game.HigherIndex);
        Data.Write(Lists.Server_Data.Max_Players);
        ToPlayer(Index, Data);
    }

    public static void Characters(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Characters);
        Data.Write(Lists.Server_Data.Max_Characters);

        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
        {
            Data.Write(Lists.Player[Index].Character[i].Name);
            Data.Write(Lists.Player[Index].Character[i].Class);
            Data.Write(Lists.Player[Index].Character[i].Genre);
            Data.Write(Lists.Player[Index].Character[i].Level);
        }

        ToPlayer(Index, Data);
    }

    public static void Classes(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Classes);
        Data.Write(Lists.Server_Data.Num_Classes);

        for (byte i = 1; i <= Lists.Class.GetUpperBound(0); i++)
        {
            Data.Write(Lists.Class[i].Name);
            Data.Write(Lists.Class[i].Texture_Male);
            Data.Write(Lists.Class[i].Texture_Female);
        }

        // Envia os dados
        ToPlayer(Index, Data);
    }

    public static void JoinGame(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.JoinGame);
        ToPlayer(Index, Data);
    }

    public static NetOutgoingMessage Player_Data_Cache(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Escreve os dados
        Data.Write((byte)Packets.Player_Data);
        Data.Write(Index);
        Data.Write(Player.Character(Index).Name);
        Data.Write(Player.Character(Index).Class);
        Data.Write(Player.Character(Index).Genre);
        Data.Write(Player.Character(Index).Level);
        Data.Write(Player.Character(Index).Map);
        Data.Write(Player.Character(Index).X);
        Data.Write(Player.Character(Index).Y);
        Data.Write((byte)Player.Character(Index).Direction);
        for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++)
        {
            Data.Write(Player.Character(Index).Vital[n]);
            Data.Write(Player.Character(Index).MaxVital(n));
        }
        for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) Data.Write(Player.Character(Index).Attribute[n]);
        for (byte n = 0; n <= (byte)Game.Equipments.Amount - 1; n++) Data.Write(Player.Character(Index).Equipment[n]);

        return Data;
    }

    public static void Player_Position(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Position);
        Data.Write(Index);
        Data.Write(Player.Character(Index).X);
        Data.Write(Player.Character(Index).Y);
        Data.Write((byte)Player.Character(Index).Direction);
        ToMap(Player.Character(Index).Map, Data);
    }

    public static void Player_Vitals(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Vitals);
        Data.Write(Index);
        for (byte i = 0; i <= (byte)Game.Vitals.Amount - 1; i++)
        {
            Data.Write(Player.Character(Index).Vital[i]);
            Data.Write(Player.Character(Index).MaxVital(i));
        }

        ToMap(Player.Character(Index).Map, Data);
    }

    public static void Player_Leave(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Leave);
        Data.Write(Index);
        ToAllBut(Index, Data);
    }

    public static void HigherIndex()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.HigherIndex);
        Data.Write(Game.HigherIndex);
        ToAll(Data);
    }

    public static void Player_Move(byte Index, byte Movement)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Move);
        Data.Write(Index);
        Data.Write(Player.Character(Index).X);
        Data.Write(Player.Character(Index).Y);
        Data.Write((byte)Player.Character(Index).Direction);
        Data.Write(Movement);
        ToMapBut(Player.Character(Index).Map, Index, Data);
    }

    public static void Player_Direction(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Direction);
        Data.Write(Index);
        Data.Write((byte)Player.Character(Index).Direction);
        ToMapBut(Player.Character(Index).Map, Index, Data);
    }

    public static void Player_Experience(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Experience);
        Data.Write(Player.Character(Index).Experience);
        Data.Write(Player.Character(Index).ExpNeeded);
        Data.Write(Player.Character(Index).Points);
        ToPlayer(Index, Data);
    }

    public static void Player_Equipments(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Equipments);
        Data.Write(Index);
        for (byte i = 0; i <= (byte)Game.Equipments.Amount - 1; i++) Data.Write(Player.Character(Index).Equipment[i]);
        ToMap(Player.Character(Index).Map, Data);
    }

    public static void Map_Players(byte Index)
    {
        // Envia os dados dos outros jogadores 
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Index != i)
                    if (Player.Character(i).Map == Player.Character(Index).Map)
                        ToPlayer(Index, Player_Data_Cache(i));

        // Envia os dados do jogador
        ToMap(Player.Character(Index).Map, Player_Data_Cache(Index));
    }

    public static void JoinMap(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.JoinMap);
        ToPlayer(Index, Data);
    }

    public static void Player_LeaveMap(byte Index, short Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Leave);
        Data.Write(Index);
        ToMapBut(Map, Index, Data);
    }

    public static void Map_Revision(byte Index, short Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Map_Revision);
        Data.Write(Map);
        Data.Write(Lists.Map[Map].Revision);
        ToPlayer(Index, Data);
    }

    public static void Map(byte Index, short Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Map);
        Data.Write(Map);
        Data.Write(Lists.Map[Map].Revision);
        Data.Write(Lists.Map[Map].Name);
        Data.Write(Lists.Map[Map].Width);
        Data.Write(Lists.Map[Map].Height);
        Data.Write(Lists.Map[Map].Moral);
        Data.Write(Lists.Map[Map].Panorama);
        Data.Write(Lists.Map[Map].Music);
        Data.Write(Lists.Map[Map].Color);
        Data.Write(Lists.Map[Map].Weather.Type);
        Data.Write(Lists.Map[Map].Weather.Intensity);
        Data.Write(Lists.Map[Map].Fog.Texture);
        Data.Write(Lists.Map[Map].Fog.Speed_X);
        Data.Write(Lists.Map[Map].Fog.Speed_Y);
        Data.Write(Lists.Map[Map].Fog.Alpha);

        // Ligações
        for (short i = 0; i <= (short)Game.Directions.Amount - 1; i++)
            Data.Write(Lists.Map[Map].Link[i]);

        // Azulejos
        Data.Write((byte)Lists.Map[Map].Tile[0, 0].Data.GetUpperBound(1));
        for (byte x = 0; x <= Lists.Map[Map].Width; x++)
            for (byte y = 0; y <= Lists.Map[Map].Height; y++)
                for (byte c = 0; c <= (byte)global::Map.Layers.Amount - 1; c++)
                    for (byte q = 0; q <= Lists.Map[Map].Tile[x, y].Data.GetUpperBound(1); q++)
                    {
                        Data.Write(Lists.Map[Map].Tile[x, y].Data[c, q].X);
                        Data.Write(Lists.Map[Map].Tile[x, y].Data[c, q].Y);
                        Data.Write(Lists.Map[Map].Tile[x, y].Data[c, q].Tile);
                        Data.Write(Lists.Map[Map].Tile[x, y].Data[c, q].Automatic);
                    }

        // Data específicos dos azulejos
        for (byte x = 0; x <= Lists.Map[Map].Width; x++)
            for (byte y = 0; y <= Lists.Map[Map].Height; y++)
            {
                Data.Write(Lists.Map[Map].Tile[x, y].Attribute);
                for (byte i = 0; i <= (byte)Game.Directions.Amount - 1; i++)
                    Data.Write(Lists.Map[Map].Tile[x, y].Block[i]);
            }

        // Luzes
        Data.Write(Lists.Map[Map].Light.GetUpperBound(0));
        if (Lists.Map[Map].Light.GetUpperBound(0) > 0)
            for (byte i = 0; i <= Lists.Map[Map].Light.GetUpperBound(0); i++)
            {
                Data.Write(Lists.Map[Map].Light[i].X);
                Data.Write(Lists.Map[Map].Light[i].Y);
                Data.Write(Lists.Map[Map].Light[i].Width);
                Data.Write(Lists.Map[Map].Light[i].Height);
            }

        // NPCs
        Data.Write((short)Lists.Map[Map].NPC.GetUpperBound(0));
        if (Lists.Map[Map].NPC.GetUpperBound(0) > 0)
            for (byte i = 1; i <= Lists.Map[Map].NPC.GetUpperBound(0); i++)
                Data.Write(Lists.Map[Map].NPC[i].Index);

        ToPlayer(Index, Data);
    }

    public static void Latency(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Latency);
        ToPlayer(Index, Data);
    }

    public static void Message(byte Index, string Mensagem, Color Color)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Message);
        Data.Write(Mensagem);
        Data.Write(Color.ToArgb());
        ToPlayer(Index, Data);
    }

    public static void Message_Map(byte Index, string Text)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();
        string Message = "[Map] " + Player.Character(Index).Name + ": " + Text;

        // Envia os dados
        Data.Write((byte)Packets.Message);
        Data.Write(Message);
        Data.Write(Color.White.ToArgb());
        ToMap(Player.Character(Index).Map, Data);
    }

    public static void Message_Global(byte Index, string Text)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();
        string Message = "[Global] " + Player.Character(Index).Name + ": " + Text;

        // Envia os dados
        Data.Write((byte)Packets.Message);
        Data.Write(Message);
        Data.Write(Color.Yellow.ToArgb());
        ToAll(Data);
    }

    public static void Mensagem_Global(string Message)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Message);
        Data.Write(Message);
        Data.Write(Color.Yellow.ToArgb());
        ToAll(Data);
    }

    public static void Message_Private(byte Index, string Addressee_Name, string Texto)
    {
        byte Addressee = Player.Find(Addressee_Name);

        // Verifica se o jogador está conectado
        if (Addressee == 0)
        {
            Message(Index, Addressee_Name + " is currently offline.", Color.Blue);
            return;
        }

        // Envia as mensagens
        Message(Index, "[To] " + Addressee_Name + ": " + Texto, Color.Pink);
        Message(Addressee, "[From] " + Player.Character(Index).Name + ": " + Texto, Color.Pink);
    }

    public static void Player_Attack(byte Index, byte Victim, byte Victim_Type)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Attack);
        Data.Write(Index);
        Data.Write(Victim);
        Data.Write(Victim_Type);
        ToMap(Player.Character(Index).Map, Data);
    }

    public static void Items(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Items);
        Data.Write((byte)Lists.Item.GetUpperBound(0));

        for (byte i = 1; i <= Lists.Item.GetUpperBound(0); i++)
        {
            // Geral
            Data.Write(Lists.Item[i].Name);
            Data.Write(Lists.Item[i].Description);
            Data.Write(Lists.Item[i].Texture);
            Data.Write(Lists.Item[i].Type);
            Data.Write(Lists.Item[i].Req_Level);
            Data.Write(Lists.Item[i].Req_Class);
            Data.Write(Lists.Item[i].Potion_Experience);
            for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++) Data.Write(Lists.Item[i].Potion_Vital[n]);
            Data.Write(Lists.Item[i].Equip_Type);
            for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) Data.Write(Lists.Item[i].Equip_Attribute[n]);
            Data.Write(Lists.Item[i].Weapon_Damage);
        }

        // Envia os dados
        ToPlayer(Index, Data);
    }

    public static void Map_Items(byte Index, short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Map_Items);
        Data.Write((short)(Lists.Map[Map_Num].Temp_Item.Count - 1));

        for (byte i = 1; i <= Lists.Map[Map_Num].Temp_Item.Count - 1; i++)
        {
            // Geral
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].Index);
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].X);
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].Y);
        }

        // Envia os dados
        ToPlayer(Index, Data);
    }

    public static void Map_Items(short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Map_Items);
        Data.Write((short)(Lists.Map[Map_Num].Temp_Item.Count - 1));
        for (byte i = 1; i <= Lists.Map[Map_Num].Temp_Item.Count - 1; i++)
        {
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].Index);
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].X);
            Data.Write(Lists.Map[Map_Num].Temp_Item[i].Y);
        }
        ToMap(Map_Num, Data);
    }

    public static void Player_Inventory(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Inventory);
        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            Data.Write(Player.Character(Index).Inventory[i].Item_Num);
            Data.Write(Player.Character(Index).Inventory[i].Amount);
        }
        ToPlayer(Index, Data);
    }

    public static void Player_Hotbar(byte Index)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Player_Hotbar);
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            Data.Write(Player.Character(Index).Hotbar[i].Type);
            Data.Write(Player.Character(Index).Hotbar[i].Slot);
        }
        ToPlayer(Index, Data);
    }
}