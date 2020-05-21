using Lidgren.Network;
using System;
using System.Drawing;

class Send
{
    // Pacotes do servidor para o cliente
    public enum Client_Packets
    {
        Alert,
        Connect,
        CreateCharacter,
        Join,
        Classes,
        Characters,
        JoinGame,
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
        Party,
        Party_Invitation,
        Trade,
        Trade_Invitation,
        Trade_State,
        Trade_Offer,
        Shops,
        Shop_Open
    }

    // Pacotes do servidor para o editor
    public enum Editor_Packets
    {
        Alert,
        Connect,
        Server_Data,
        Classes,
        Maps,
        Map,
        NPCs,
        Items,
        Shops
    }

    private static void ToPlayer(Account.Structure Account, NetOutgoingMessage Data)
    {
        // Recria o pacote e o envia
        NetOutgoingMessage Data_Send = Socket.Device.CreateMessage(Data.LengthBytes);
        Data_Send.Write(Data);
        Socket.Device.SendMessage(Data_Send, Account.Connection, NetDeliveryMethod.ReliableOrdered);
    }

    private static void ToPlayer(Player Player, NetOutgoingMessage Data)
    {
        ToPlayer(Player.Account, Data);
    }

    private static void ToAll(NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                ToPlayer(Lists.Account[i].Character, Data);
    }

    private static void ToAllBut(Player Player, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Player != Lists.Account[i].Character)
                    ToPlayer(Lists.Account[i].Character, Data);
    }

    private static void ToMap(short Map_Num, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Map_Num == Map_Num)
                    ToPlayer(Lists.Account[i].Character, Data);
    }

    private static void ToMapBut(short Map_Num, Player Player, NetOutgoingMessage Data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Map_Num == Map_Num)
                    if (Player != Lists.Account[i].Character)
                        ToPlayer(Lists.Account[i].Character, Data);
    }

    public static void Alert(Account.Structure Account, string Message, bool Disconnect = true)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Alert);
        else Data.Write((byte)Client_Packets.Alert);
        Data.Write(Message);
        ToPlayer(Account, Data);

        // Desconecta o jogador
        if (Disconnect) Account.Connection.Disconnect(string.Empty);
    }

    public static void Connect(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Connect);
        else Data.Write((byte)Client_Packets.Connect);
        ToPlayer(Account, Data);
    }

    public static void CreateCharacter(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.CreateCharacter);
        ToPlayer(Account, Data);
    }

    public static void Join(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Join);
        Data.Write(Player.Name);
        ToPlayer(Player, Data);
    }

    public static void Characters(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Characters);
        Data.Write((byte)Account.Characters.Count);

        for (byte i = 0; i < Account.Characters.Count; i++)
        {
            Data.Write(Account.Characters[i].Name);
            Data.Write(Account.Characters[i].Texture_Num);
            Data.Write(Account.Characters[i].Level);
        }

        ToPlayer(Account, Data);
    }

    public static void Classes(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Classes);
        else Data.Write((byte)Client_Packets.Classes);
        Data.Write((byte)Lists.Class.Count);

        foreach (Objects.Class Class in Lists.Class.Values)
        {
            // Escreve os dados
            Data.Write(Class.ID.ToString());
            Data.Write(Class.Name);
            Data.Write(Class.Description);
            Data.Write((byte)Class.Tex_Male.Length);
            for (byte t = 0; t < Class.Tex_Male.Length; t++) Data.Write(Class.Tex_Male[t]);
            Data.Write((byte)Class.Tex_Female.Length);
            for (byte t = 0; t < Class.Tex_Female.Length; t++) Data.Write(Class.Tex_Female[t]);

            // Apenas dados do editor
            if (Account.InEditor)
            {
                Data.Write(Class.Spawn_Map);
                Data.Write(Class.Spawn_Direction);
                Data.Write(Class.Spawn_X);
                Data.Write(Class.Spawn_Y);
                for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(Class.Vital[n]);
                for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Data.Write(Class.Attribute[n]);
                Data.Write((byte)Class.Item.Length);
                for (byte n = 0; n < (byte)Class.Item.Length; n++)
                {
                    Data.Write(Lists.GetID(Class.Item[n].Item1));
                    Data.Write(Class.Item[n].Item2);
                }
            }
        }

        // Envia os dados
        ToPlayer(Account, Data);
    }

    public static void JoinGame(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.JoinGame);
        ToPlayer(Player, Data);
    }

    public static NetOutgoingMessage Player_Data_Cache(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Escreve os dados
        Data.Write((byte)Client_Packets.Player_Data);
        Data.Write(Player.Name);
        Data.Write(Player.Texture_Num);
        Data.Write(Player.Level);
        Data.Write(Player.Map_Num);
        Data.Write(Player.X);
        Data.Write(Player.Y);
        Data.Write((byte)Player.Direction);
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
        {
            Data.Write(Player.Vital[n]);
            Data.Write(Player.MaxVital(n));
        }
        for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Data.Write(Player.Attribute[n]);
        for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Data.Write(Lists.GetID(Player.Equipment[n]));

        return Data;
    }

    public static void Player_Position(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Position);
        Data.Write(Player.Name);
        Data.Write(Player.X);
        Data.Write(Player.Y);
        Data.Write((byte)Player.Direction);
        ToMap(Player.Map_Num, Data);
    }

    public static void Player_Vitals(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Vitals);
        Data.Write(Player.Name);
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++)
        {
            Data.Write(Player.Vital[i]);
            Data.Write(Player.MaxVital(i));
        }

        ToMap(Player.Map_Num, Data);
    }

    public static void Player_Leave(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Leave);
        Data.Write(Player.Name);
        ToAllBut(Player, Data);
    }

    public static void Player_Move(Player Player, byte Movement)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Move);
        Data.Write(Player.Name);
        Data.Write(Player.X);
        Data.Write(Player.Y);
        Data.Write((byte)Player.Direction);
        Data.Write(Movement);
        ToMapBut(Player.Map_Num, Player, Data);
    }

    public static void Player_Direction(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Direction);
        Data.Write(Player.Name);
        Data.Write((byte)Player.Direction);
        ToMapBut(Player.Map_Num, Player, Data);
    }

    public static void Player_Experience(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Experience);
        Data.Write(Player.Experience);
        Data.Write(Player.ExpNeeded);
        Data.Write(Player.Points);
        ToPlayer(Player, Data);
    }

    public static void Player_Equipments(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Equipments);
        Data.Write(Player.Name);
        for (byte i = 0; i < (byte)Game.Equipments.Count; i++) Data.Write(Lists.GetID(Player.Equipment[i]));
        ToMap(Player.Map_Num, Data);
    }

    public static void Map_Players(Player Player)
    {
        // Envia os dados dos outros jogadores 
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Player != Lists.Account[i].Character)
                    if (Lists.Account[i].Character.Map_Num == Player.Map_Num)
                        ToPlayer(Player, Player_Data_Cache(Lists.Account[i].Character));

        // Envia os dados do jogador
        ToMap(Player.Map_Num, Player_Data_Cache(Player));
    }

    public static void JoinMap(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.JoinMap);
        ToPlayer(Player, Data);
    }

    public static void Player_LeaveMap(Player Player, short Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Leave);
        Data.Write(Player.Name);
        ToMapBut(Map, Player, Data);
    }

    public static void Map_Revision(Player Player, short Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_Revision);
        Data.Write(Map);
        Data.Write(Lists.Map[Map].Revision);
        ToPlayer(Player, Data);
    }

    public static void Maps(Account.Structure Account, bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Editor_Packets.Maps);
        Data.Write((short)Lists.Map.Length);
        ToPlayer(Account, Data);
        for (short i = 1; i < Lists.Map.Length; i++) Map(Account, i);
    }

    public static void Map(Account.Structure Account, short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Map);
        else Data.Write((byte)Client_Packets.Map);
        Data.Write(Map_Num);
        Data.Write(Lists.Map[Map_Num].Revision);
        Data.Write(Lists.Map[Map_Num].Name);
        Data.Write(Lists.Map[Map_Num].Width);
        Data.Write(Lists.Map[Map_Num].Height);
        Data.Write(Lists.Map[Map_Num].Moral);
        Data.Write(Lists.Map[Map_Num].Panorama);
        Data.Write(Lists.Map[Map_Num].Music);
        Data.Write(Lists.Map[Map_Num].Color);
        Data.Write(Lists.Map[Map_Num].Weather.Type);
        Data.Write(Lists.Map[Map_Num].Weather.Intensity);
        Data.Write(Lists.Map[Map_Num].Fog.Texture);
        Data.Write(Lists.Map[Map_Num].Fog.Speed_X);
        Data.Write(Lists.Map[Map_Num].Fog.Speed_Y);
        Data.Write(Lists.Map[Map_Num].Fog.Alpha);
        Data.Write(Lists.Map[Map_Num].Light_Global);
        Data.Write(Lists.Map[Map_Num].Lighting);

        // Ligações
        for (short i = 0; i < (short)Game.Directions.Count; i++)
            Data.Write(Lists.Map[Map_Num].Link[i]);

        // Camadas
        Data.Write((byte)(Lists.Map[Map_Num].Layer.Count - 1));
        for (byte i = 0; i < Lists.Map[Map_Num].Layer.Count; i++)
        {
            Data.Write(Lists.Map[Map_Num].Layer[i].Name);
            Data.Write(Lists.Map[Map_Num].Layer[i].Type);

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Map_Num].Width; x++)
                for (byte y = 0; y <= Lists.Map[Map_Num].Height; y++)
                {
                    Data.Write(Lists.Map[Map_Num].Layer[i].Tile[x, y].X);
                    Data.Write(Lists.Map[Map_Num].Layer[i].Tile[x, y].Y);
                    Data.Write(Lists.Map[Map_Num].Layer[i].Tile[x, y].Tile);
                    Data.Write(Lists.Map[Map_Num].Layer[i].Tile[x, y].Auto);
                }
        }

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map[Map_Num].Width; x++)
            for (byte y = 0; y <= Lists.Map[Map_Num].Height; y++)
            {
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Attribute);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Data_1);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Data_2);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Data_3);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Data_4);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Data_5);
                Data.Write(Lists.Map[Map_Num].Tile[x, y].Zone);

                // Bloqueio direcional
                for (byte i = 0; i < (byte)Game.Directions.Count; i++)
                    Data.Write(Lists.Map[Map_Num].Tile[x, y].Block[i]);
            }

        // Luzes
        Data.Write((byte)Lists.Map[Map_Num].Light.Length);
        for (byte i = 0; i < Lists.Map[Map_Num].Light.Length; i++)
        {
            Data.Write(Lists.Map[Map_Num].Light[i].X);
            Data.Write(Lists.Map[Map_Num].Light[i].Y);
            Data.Write(Lists.Map[Map_Num].Light[i].Width);
            Data.Write(Lists.Map[Map_Num].Light[i].Height);
        }

        // NPCs
        Data.Write((byte)Lists.Map[Map_Num].NPC.Length);
        for (byte i = 0; i < Lists.Map[Map_Num].NPC.Length; i++)
        {
            Data.Write(Lists.GetID(Lists.Map[Map_Num].NPC[i].NPC));
            Data.Write(Lists.Map[Map_Num].NPC[i].Zone);
            Data.Write(Lists.Map[Map_Num].NPC[i].Spawn);
            Data.Write(Lists.Map[Map_Num].NPC[i].X);
            Data.Write(Lists.Map[Map_Num].NPC[i].Y);
        }
        ToPlayer(Account, Data);
    }

    public static void Latency(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Latency);
        ToPlayer(Account, Data);
    }

    public static void Message(Player Player, string Text, Color Color)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Message);
        Data.Write(Text);
        Data.Write(Color.ToArgb());
        ToPlayer(Player, Data);
    }

    public static void Message_Map(Player Player, string Text)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();
        string Message = "[Map] " + Player.Name + ": " + Text;

        // Envia os dados
        Data.Write((byte)Client_Packets.Message);
        Data.Write(Message);
        Data.Write(Color.White.ToArgb());
        ToMap(Player.Map_Num, Data);
    }

    public static void Message_Global(Player Player, string Text)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();
        string Message = "[Global] " + Player.Name + ": " + Text;

        // Envia os dados
        Data.Write((byte)Client_Packets.Message);
        Data.Write(Message);
        Data.Write(Color.Yellow.ToArgb());
        ToAll(Data);
    }

    public static void Message_Private(Player Player, string Addressee_Name, string Texto)
    {
        Player Addressee = Account.FindPlayer(Addressee_Name);

        // Verifica se o jogador está conectado
        if (Addressee == null)
        {
            Message(Player, Addressee_Name + " is currently offline.", Color.Blue);
            return;
        }

        // Envia as mensagens
        Message(Player, "[To] " + Addressee_Name + ": " + Texto, Color.Pink);
        Message(Addressee, "[From] " + Player.Name + ": " + Texto, Color.Pink);
    }

    public static void Player_Attack(Player Player, string Victim = "", Game.Target Victim_Type = 0)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Attack);
        Data.Write(Player.Name);
        Data.Write(Victim);
        Data.Write((byte)Victim_Type);
        ToMap(Player.Map_Num, Data);
    }

    public static void Items(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Items);
        else Data.Write((byte)Client_Packets.Items);
        Data.Write((short)Lists.Item.Count);
        foreach (Objects.Item Item in Lists.Item.Values)
        {
            Data.Write(Item.ID.ToString());
            Data.Write(Item.Name);
            Data.Write(Item.Description);
            Data.Write(Item.Texture);
            Data.Write(Item.Type);
            Data.Write(Item.Stackable);
            Data.Write(Item.Bind);
            Data.Write(Item.Rarity);
            Data.Write(Item.Req_Level);
            Data.Write(Lists.GetID(Item.Req_Class));
            Data.Write(Item.Potion_Experience);
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Data.Write(Item.Potion_Vital[v]);
            Data.Write(Item.Equip_Type);
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Data.Write(Item.Equip_Attribute[a]);
            Data.Write(Item.Weapon_Damage);
        }

        // Envia os dados
        ToPlayer(Account, Data);
    }

    public static void Map_Items(Player Player, short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_Items);
        Data.Write((short)(Lists.Temp_Map[Map_Num].Item.Count - 1));

        for (byte i = 1; i < Lists.Temp_Map[Map_Num].Item.Count; i++)
        {
            // Geral
            Data.Write(Lists.GetID(Lists.Temp_Map[Map_Num].Item[i].Item));
            Data.Write(Lists.Temp_Map[Map_Num].Item[i].X);
            Data.Write(Lists.Temp_Map[Map_Num].Item[i].Y);
        }

        // Envia os dados
        ToPlayer(Player, Data);
    }

    public static void Map_Items(short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_Items);
        Data.Write((short)(Lists.Temp_Map[Map_Num].Item.Count - 1));
        for (byte i = 1; i < Lists.Temp_Map[Map_Num].Item.Count; i++)
        {
            Data.Write(Lists.GetID(Lists.Temp_Map[Map_Num].Item[i].Item));
            Data.Write(Lists.Temp_Map[Map_Num].Item[i].X);
            Data.Write(Lists.Temp_Map[Map_Num].Item[i].Y);
        }
        ToMap(Map_Num, Data);
    }

    public static void Player_Inventory(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Inventory);
        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            Data.Write(Lists.GetID(Player.Inventory[i].Item));
            Data.Write(Player.Inventory[i].Amount);
        }
        ToPlayer(Player, Data);
    }

    public static void Player_Hotbar(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Player_Hotbar);
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            Data.Write(Player.Hotbar[i].Type);
            Data.Write(Player.Hotbar[i].Slot);
        }
        ToPlayer(Player, Data);
    }

    public static void NPCs(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.NPCs);
        else Data.Write((byte)Client_Packets.NPCs);
        Data.Write((short)Lists.NPC.Count);
        foreach (Objects.NPC NPC in Lists.NPC.Values)
        {
            // Geral
            Data.Write(NPC.ID.ToString());
            Data.Write(NPC.Name);
            Data.Write(NPC.SayMsg);
            Data.Write(NPC.Texture);
            Data.Write(NPC.Behaviour);
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(NPC.Vital[n]);

            // Dados apenas do editor
            if (Account.InEditor)
            {
                Data.Write(NPC.SpawnTime);
                Data.Write(NPC.Sight);
                Data.Write(NPC.Experience);
                for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Data.Write(NPC.Attribute[n]);
                Data.Write((byte)NPC.Drop.Length);
                for (byte n = 0; n < NPC.Drop.Length; n++)
                {
                    Data.Write(Lists.GetID(NPC.Drop[n].Item));
                    Data.Write(NPC.Drop[n].Amount);
                    Data.Write(NPC.Drop[n].Chance);
                }
                Data.Write(NPC.AttackNPC);
                Data.Write((byte)NPC.Allie.Length);
                for (byte n = 0; n < NPC.Allie.Length; n++) Data.Write(Lists.GetID(NPC.Allie[n]));
                Data.Write((byte)NPC.Movement);
                Data.Write(NPC.Flee_Helth);
                Data.Write(Lists.GetID(NPC.Shop));
            }
        }
        ToPlayer(Account, Data);
    }

    public static void Map_NPCs(Player Player, short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPCs);
        Data.Write((short)Lists.Temp_Map[Map_Num].NPC.Length);
        for (byte i = 1; i < Lists.Temp_Map[Map_Num].NPC.Length; i++)
        {
            Data.Write(Lists.GetID(Lists.Temp_Map[Map_Num].NPC[i].Data));
            Data.Write(Lists.Temp_Map[Map_Num].NPC[i].X);
            Data.Write(Lists.Temp_Map[Map_Num].NPC[i].Y);
            Data.Write((byte)Lists.Temp_Map[Map_Num].NPC[i].Direction);
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(Lists.Temp_Map[Map_Num].NPC[i].Vital[n]);
        }
        ToPlayer(Player, Data);
    }

    public static void Map_NPC(NPC.Structure NPC)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC);
        Data.Write(NPC.Index);
        Data.Write(Lists.GetID(NPC.Data));
        Data.Write(NPC.X);
        Data.Write(NPC.Y);
        Data.Write((byte)NPC.Direction);
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(NPC.Vital[n]);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Map_NPC_Movement(NPC.Structure NPC, byte Movement)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC_Movement);
        Data.Write(NPC.Index);
        Data.Write(NPC.X);
        Data.Write(NPC.Y);
        Data.Write((byte)NPC.Direction);
        Data.Write(Movement);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Map_NPC_Direction(NPC.Structure NPC)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC_Direction);
        Data.Write(NPC.Index);
        Data.Write((byte)NPC.Direction);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Map_NPC_Vitals(NPC.Structure NPC)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC_Vitals);
        Data.Write(NPC.Index);
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(NPC.Vital[n]);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Map_NPC_Attack(NPC.Structure NPC, string Victim = "", Game.Target Victim_Type = 0)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC_Attack);
        Data.Write(NPC.Index);
        Data.Write(Victim);
        Data.Write((byte)Victim_Type);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Map_NPC_Died(NPC.Structure NPC)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Map_NPC_Died);
        Data.Write(NPC.Index);
        ToMap(NPC.Map_Num, Data);
    }

    public static void Server_Data(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Editor_Packets.Server_Data);
        Data.Write(Lists.Server_Data.Game_Name);
        Data.Write(Lists.Server_Data.Welcome);
        Data.Write(Lists.Server_Data.Port);
        Data.Write(Lists.Server_Data.Max_Players);
        Data.Write(Lists.Server_Data.Max_Characters);
        Data.Write(Lists.Server_Data.Max_Party_Members);
        Data.Write(Lists.Server_Data.Max_Map_Items);
        Data.Write(Lists.Server_Data.Num_Points);
        ToPlayer(Account, Data);
    }

    public static void Party(Player Player)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Party);
        Data.Write((byte)Player.Party.Count);
        for (byte i = 0; i < Player.Party.Count; i++) Data.Write(Player.Party[i].Name);
        ToPlayer(Player, Data);
    }

    public static void Party_Invitation(Player Player, string Player_Invitation)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Party_Invitation);
        Data.Write(Player_Invitation);
        ToPlayer(Player, Data);
    }

    public static void Trade(Player Player, bool State)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Trade);
        Data.Write(State);
        ToPlayer(Player, Data);
    }

    public static void Trade_Invitation(Player Player, string Player_Invitation)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Trade_Invitation);
        Data.Write(Player_Invitation);
        ToPlayer(Player, Data);
    }

    public static void Trade_State(Player Player, Game.Trade_Status State)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Trade_State);
        Data.Write((byte)State);
        ToPlayer(Player, Data);
    }

    public static void Trade_Offer(Player Player, bool Own = true)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();
        Player To = Own ? Player : Player.Trade;

        // Envia os dados
        Data.Write((byte)Client_Packets.Trade_Offer);
        Data.Write(Own);
        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            Data.Write(Lists.GetID(To.Inventory[To.Trade_Offer[i].Slot_Num].Item));
            Data.Write(To.Trade_Offer[i].Amount);
        }
        ToPlayer(Player, Data);
    }

    public static void Shops(Account.Structure Account)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        if (Account.InEditor) Data.Write((byte)Editor_Packets.Shops);
        else Data.Write((byte)Client_Packets.Shops);
        Data.Write((short)Lists.Shop.Count);
        foreach (Objects.Shop Shop in Lists.Shop.Values)
        {
            // Geral
            Data.Write(Shop.ID.ToString());
            Data.Write((byte)Shop.Sold.Length);
            Data.Write((byte)Shop.Bought.Length);
            Data.Write(Shop.Name);
            Data.Write(Lists.GetID(Shop.Currency));
            for (byte j = 0; j < Shop.Sold.Length; j++)
            {
                Data.Write(Lists.GetID(Shop.Sold[j].Item));
                Data.Write(Shop.Sold[j].Amount);
                Data.Write(Shop.Sold[j].Price);
            }
            for (byte j = 0; j < Shop.Bought.Length; j++)
            {
                Data.Write(Lists.GetID(Shop.Bought[j].Item));
                Data.Write(Shop.Bought[j].Amount);
                Data.Write(Shop.Bought[j].Price);
            }
        }
        ToPlayer(Account, Data);
    }

    public static void Shop_Open(Player Player, Objects.Shop Shop)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Client_Packets.Shop_Open);
        Data.Write(Lists.GetID(Shop));
        ToPlayer(Player, Data);
    }
}