using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

class Receive
{
    // Pacotes do servidor
    private enum Packets
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

    public static void Handle(NetIncomingMessage Data)
    {
        // Manuseia os dados recebidos
        switch ((Packets)Data.ReadByte())
        {
            case Packets.Alert: Alert(Data); break;
            case Packets.Connect: Connect(); break;
            case Packets.Join: Join(Data); break;
            case Packets.CreateCharacter: CreateCharacter(); break;
            case Packets.JoinGame: JoinGame(); break;
            case Packets.Classes: Classes(Data); break;
            case Packets.Characters: Characters(Data); break;
            case Packets.Player_Data: Player_Data(Data); break;
            case Packets.Player_Position: Player_Position(Data); break;
            case Packets.Player_Vitals: Player_Vitals(Data); break;
            case Packets.Player_Move: Player_Move(Data); break;
            case Packets.Player_Leave: Player_Leave(Data); break;
            case Packets.Player_Direction: Player_Direction(Data); break;
            case Packets.Player_Attack: Player_Attack(Data); break;
            case Packets.Player_Experience: Player_Experience(Data); break;
            case Packets.Player_Inventory: Player_Inventory(Data); break;
            case Packets.Player_Equipments: Player_Equipments(Data); break;
            case Packets.Player_Hotbar: Player_Hotbar(Data); break;
            case Packets.Map_Revision: Map_Revision(Data); break;
            case Packets.Map: Map(Data); break;
            case Packets.JoinMap: JoinMap(); break;
            case Packets.Latency: Latency(); break;
            case Packets.Message: Message(Data); break;
            case Packets.NPCs: NPCs(Data); break;
            case Packets.Map_NPCs: Map_NPCs(Data); break;
            case Packets.Map_NPC: Map_NPC(Data); break;
            case Packets.Map_NPC_Movement: Map_NPC_Movement(Data); break;
            case Packets.Map_NPC_Direction: Map_NPC_Direction(Data); break;
            case Packets.Map_NPC_Vitals: Map_NPC_Vitals(Data); break;
            case Packets.Map_NPC_Attack: Map_NPC_Attack(Data); break;
            case Packets.Map_NPC_Died: Map_NPC_Died(Data); break;
            case Packets.Items: Items(Data); break;
            case Packets.Map_Items: Map_Items(Data); break;
            case Packets.Party: Party(Data); break;
            case Packets.Party_Invitation: Party_Invitation(Data); break;
            case Packets.Trade: Trade(Data); break;
            case Packets.Trade_Invitation: Trade_Invitation(Data); break;
            case Packets.Trade_State: Trade_State(Data); break;
            case Packets.Trade_Offer: Trade_Offer(Data); break;
            case Packets.Shops: Shops(Data); break;
            case Packets.Shop_Open: Shop_Open(Data); break;
        }
    }

    private static void Alert(NetIncomingMessage Data)
    {
        // Mostra a mensagem
        MessageBox.Show(Data.ReadString());
    }

    private static void Connect()
    {
        // Reseta os valores
        Utilities.SelectCharacter = 0;
        Lists.Class = new Dictionary<Guid, Lists.Structures.Class>();

        // Abre o painel de seleção de personagens
        Panels.Menu_Close();
        Panels.Get("SelectCharacter").Visible = true;
    }

    private static void Join(NetIncomingMessage Data)
    {
        // Definir os valores que são enviados do servidor
        Player.Me = new Player.Me_Structure(Data.ReadString());
        Lists.Player.Add(Player.Me);

        // Reseta alguns valores
        Lists.Class = new Dictionary<Guid, Lists.Structures.Class>();
        Lists.Item = new Dictionary<Guid, Lists.Structures.Item>();
        Lists.Shop = new Dictionary<Guid, Lists.Structures.Shop>();
    }

    private static void CreateCharacter()
    {
        // Reseta os valores
        TextBoxes.Get("CreateCharacter_Name").Text = string.Empty;
        CheckBoxes.Get("GenderMale").Checked = true;
        CheckBoxes.Get("GenderFemale").Checked = false;
        Utilities.CreateCharacter_Class = 0;
        Utilities.CreateCharacter_Tex = 0;

        // Abre o painel de criação de personagem
        Panels.Menu_Close();
        Panels.Get("CreateCharacter").Visible = true;
    }

    private static void Classes(NetIncomingMessage Data)
    {
        // Classes a serem removidas
        Dictionary<Guid, Lists.Structures.Class> ToRemove = new Dictionary<Guid, Lists.Structures.Class>(Lists.Class);

        // Quantidade de classes
        short Count = Data.ReadByte();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Class Class;

            // Obtém o dado
            if (Lists.Class.ContainsKey(ID))
            {
                Class = Lists.Class[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Class = new Lists.Structures.Class(ID);
                Lists.Class.Add(Class.ID, Class);
            }

            // Recebe os dados do personagem
            Class.Name = Data.ReadString();
            Class.Description = Data.ReadString();
            Class.Tex_Male = new short[Data.ReadByte()];
            for (byte n = 0; n < Class.Tex_Male.Length; n++) Class.Tex_Male[n] = Data.ReadInt16();
            Class.Tex_Female = new short[Data.ReadByte()];
            for (byte n = 0; n < Class.Tex_Female.Length; n++) Class.Tex_Female[n] = Data.ReadInt16();
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Shop.Remove(Remove);
    }

    private static void Characters(NetIncomingMessage Data)
    {
        // Redimensiona a lista
        Lists.Characters = new Lists.Structures.Character[Data.ReadByte()];

        for (byte i = 0; i < Lists.Characters.Length; i++)
        {
            // Recebe os dados do personagem
            Lists.Characters[i] = new Lists.Structures.Character
            {
                Name = Data.ReadString(),
                Texture_Num = Data.ReadInt16(),
                Level = Data.ReadInt16()
            };
        }
    }

    private static void JoinGame()
    {
        // Reseta os valores
        Chat.Order = new System.Collections.Generic.List<Chat.Structure>();
        Chat.Lines_First = 0;
        Loop.Chat_Timer = Environment.TickCount + Chat.Sleep_Timer;
        TextBoxes.Get("Chat").Text = string.Empty;
        CheckBoxes.Get("Options_Sounds").Checked = Lists.Options.Sounds;
        CheckBoxes.Get("Options_Musics").Checked = Lists.Options.Musics;
        CheckBoxes.Get("Options_Chat").Checked = Lists.Options.Chat;
        CheckBoxes.Get("Options_FPS").Checked = Lists.Options.FPS;
        CheckBoxes.Get("Options_Latency").Checked = Lists.Options.Latency;
        CheckBoxes.Get("Options_Trade").Checked = Lists.Options.Trade;
        CheckBoxes.Get("Options_Party").Checked = Lists.Options.Party;
        Loop.Chat_Timer = Loop.Chat_Timer = Environment.TickCount + 10000;
        Utilities.Infomation_ID = Guid.Empty.ToString();

        // Reseta a interface
        Panels.Get("Menu_Character").Visible = false;
        Panels.Get("Menu_Inventory").Visible = false;
        Panels.Get("Menu_Options").Visible = false;
        Panels.Get("Chat").Visible = false;
        Panels.Get("Drop").Visible = false;
        Panels.Get("Party_Invitation").Visible = false;
        Panels.Get("Trade").Visible = false;
        Buttons.Get("Trade_Offer_Confirm").Visible = true;
        Buttons.Get("Trade_Offer_Accept").Visible = Buttons.Get("Trade_Offer_Decline").Visible = false;
        Panels.Get("Trade_Offer_Disable").Visible = false;
        Panels.Get("Shop").Visible = false;
        Panels.Get("Shop_Sell").Visible = false;

        // Abre o jogo
        Audio.Music.Stop();
        Window.Current = Window.Types.Game;
    }

    private static void Player_Data(NetIncomingMessage Data)
    {
        string Name = Data.ReadString();
        Player.Structure Player;

        // Adiciona o jogador à lista
        if (Name != global::Player.Me.Name)
        {
            Player = new Player.Structure(Name);
            Lists.Player.Add(Player);
        }
        else
            Player = global::Player.Me;

        // Defini os dados do jogador
        Player.Texture_Num = Data.ReadInt16();
        Player.Level = Data.ReadInt16();
        Player.Map_Num = Data.ReadInt16();
        Player.X = Data.ReadByte();
        Player.Y = Data.ReadByte();
        Player.Direction = (Game.Directions)Data.ReadByte();
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
        {
            Player.Vital[n] = Data.ReadInt16();
            Player.Max_Vital[n] = Data.ReadInt16();
        }
        for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Player.Attribute[n] = Data.ReadInt16();
        for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Player.Equipment[n] = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
    }

    private static void Player_Position(NetIncomingMessage Data)
    {
        Player.Structure Player = global::Player.Get(Data.ReadString());

        // Defini os dados do jogador
        Player.X = Data.ReadByte();
        Player.Y = Data.ReadByte();
        Player.Direction = (Game.Directions)Data.ReadByte();

        // Para a movimentação
        Player.X2 = 0;
        Player.Y2 = 0;
        Player.Movement = Game.Movements.Stopped;
    }

    private static void Player_Vitals(NetIncomingMessage Data)
    {
        Player.Structure Player = global::Player.Get(Data.ReadString());

        // Define os dados
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++)
        {
            Player.Vital[i] = Data.ReadInt16();
            Player.Max_Vital[i] = Data.ReadInt16();
        }
    }

    private static void Player_Equipments(NetIncomingMessage Data)
    {
        Player.Structure Player = global::Player.Get(Data.ReadString());

        // Altera os dados dos equipamentos do jogador
        for (byte i = 0; i < (byte)Game.Equipments.Count; i++) Player.Equipment[i] = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
    }

    private static void Player_Leave(NetIncomingMessage Data)
    {
        // Limpa os dados do jogador
        Lists.Player.Remove(Player.Get(Data.ReadString()));
    }

    private static void Player_Move(NetIncomingMessage Data)
    {
        Player.Structure Player = global::Player.Get(Data.ReadString());

        // Move o jogador
        Player.X = Data.ReadByte();
        Player.Y = Data.ReadByte();
        Player.Direction = (Game.Directions)Data.ReadByte();
        Player.Movement = (Game.Movements)Data.ReadByte();
        Player.X2 = 0;
        Player.Y2 = 0;

        // Posição exata do jogador
        switch (Player.Direction)
        {
            case Game.Directions.Up: Player.Y2 = Game.Grid; break;
            case Game.Directions.Down: Player.Y2 = Game.Grid * -1; break;
            case Game.Directions.Right: Player.X2 = Game.Grid * -1; break;
            case Game.Directions.Left: Player.X2 = Game.Grid; break;
        }
    }

    private static void Player_Direction(NetIncomingMessage Data)
    {
        // Altera a posição do jogador
        Player.Get(Data.ReadString()).Direction = (Game.Directions)Data.ReadByte();
    }

    private static void Player_Attack(NetIncomingMessage Data)
    {
        Player.Structure Player = global::Player.Get(Data.ReadString());
        string Victim = Data.ReadString();
        byte Victim_Type = Data.ReadByte();

        // Inicia o ataque
        Player.Attacking = true;
        Player.Attack_Timer = Environment.TickCount;

        // Sofrendo dano
        if (Victim != string.Empty)
            if (Victim_Type == (byte)Game.Target.Player)
            {
                Player.Structure Victim_Data = global::Player.Get(Victim);
                Victim_Data.Hurt = Environment.TickCount;
                Lists.Temp_Map.Blood.Add(new Lists.Structures.Map_Blood((byte)Game.Random.Next(0, 3), Victim_Data.X, Victim_Data.Y, 255));
            }
            else if (Victim_Type == (byte)Game.Target.NPC)
            {
                Lists.Temp_Map.NPC[byte.Parse(Victim)].Hurt = Environment.TickCount;
                Lists.Temp_Map.Blood.Add(new Lists.Structures.Map_Blood((byte)Game.Random.Next(0, 3), Lists.Temp_Map.NPC[byte.Parse(Victim)].X, Lists.Temp_Map.NPC[byte.Parse(Victim)].Y, 255));
            }
    }

    private static void Player_Experience(NetIncomingMessage Data)
    {
        // Define os dados
        Player.Me.Experience = Data.ReadInt32();
        Player.Me.ExpNeeded = Data.ReadInt32();
        Player.Me.Points = Data.ReadByte();

        // Manipula a visibilidade dos botões
        Buttons.Get("Attributes_Strength").Visible = Player.Me.Points > 0;
        Buttons.Get("Attributes_Resistance").Visible = Player.Me.Points > 0;
        Buttons.Get("Attributes_Intelligence").Visible = Player.Me.Points > 0;
        Buttons.Get("Attributes_Agility").Visible = Player.Me.Points > 0;
        Buttons.Get("Attributes_Vitality").Visible = Player.Me.Points > 0;
    }

    private static void Player_Inventory(NetIncomingMessage Data)
    {
        // Define os dados
        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            Player.Me.Inventory[i].Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            Player.Me.Inventory[i].Amount = Data.ReadInt16();
        }
    }

    private static void Player_Hotbar(NetIncomingMessage Data)
    {
        // Define os dados
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            Player.Me.Hotbar[i].Type = Data.ReadByte();
            Player.Me.Hotbar[i].Slot = Data.ReadByte();
        }
    }

    private static void Map_Revision(NetIncomingMessage Data)
    {
        bool Needed = false;
        short Map_Num = Data.ReadInt16();

        // Limpa todos os outros jogadores
        for (byte i = 0; i < Lists.Player.Count; i++)
            if (Lists.Player[i] != Player.Me)
                Lists.Player.RemoveAt(i);

        // Verifica se é necessário baixar os dados do mapa
        if (System.IO.File.Exists(Directories.Maps_Data.FullName + Map_Num + Directories.Format))
        {
            Read.Map(Map_Num);
            if (Lists.Map.Revision != Data.ReadInt16())
                Needed = true;
        }
        else
            Needed = true;

        // Solicita os dados do mapa
        Send.RequestMap(Needed);

        // Reseta os sangues do mapa
        Lists.Temp_Map.Blood = new System.Collections.Generic.List<Lists.Structures.Map_Blood>();
    }

    private static void Map(NetIncomingMessage Data)
    {
        // Define os dados
        short Map_Num = Data.ReadInt16();

        // Lê os dados
        Lists.Map.Revision = Data.ReadInt16();
        Lists.Map.Name = Data.ReadString();
        Lists.Map.Width = Data.ReadByte();
        Lists.Map.Height = Data.ReadByte();
        Lists.Map.Moral = Data.ReadByte();
        Lists.Map.Panorama = Data.ReadByte();
        Lists.Map.Music = Data.ReadByte();
        Lists.Map.Color = Data.ReadInt32();
        Lists.Map.Weather.Type = Data.ReadByte();
        Lists.Map.Weather.Intensity = Data.ReadByte();
        Lists.Map.Fog.Texture = Data.ReadByte();
        Lists.Map.Fog.Speed_X = Data.ReadSByte();
        Lists.Map.Fog.Speed_Y = Data.ReadSByte();
        Lists.Map.Fog.Alpha = Data.ReadByte();
        Data.ReadByte(); // Luz global
        Data.ReadByte(); // Iluminação

        // Ligações
        Lists.Map.Link = new short[(byte)Game.Directions.Count];
        for (short i = 0; i < (short)Game.Directions.Count; i++)
            Lists.Map.Link[i] = Data.ReadInt16();

        // Azulejos
        byte Num_Layers = Data.ReadByte();

        // Redimensiona os dados
        Lists.Map.Tile = new Lists.Structures.Map_Tile[Lists.Map.Width + 1, Lists.Map.Height + 1];
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
                Lists.Map.Tile[x, y].Data = new Lists.Structures.Map_Tile_Data[(byte)global::Map.Layers.Amount, Num_Layers + 1];

        // Lê os azulejos
        for (byte i = 0; i <= Num_Layers; i++)
        {
            // Dados básicos
            Data.ReadString(); // Name
            byte t = Data.ReadByte(); // Tipo

            // Azulejos
            for (byte x = 0; x <= Lists.Map.Width; x++)
                for (byte y = 0; y <= Lists.Map.Height; y++)
                {
                    Lists.Map.Tile[x, y].Data[t, i].X = Data.ReadByte();
                    Lists.Map.Tile[x, y].Data[t, i].Y = Data.ReadByte();
                    Lists.Map.Tile[x, y].Data[t, i].Tile = Data.ReadByte();
                    Lists.Map.Tile[x, y].Data[t, i].Automatic = Data.ReadBoolean();
                    Lists.Map.Tile[x, y].Data[t, i].Mini = new Point[4];
                }
        }

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
            {
                Lists.Map.Tile[x, y].Attribute = Data.ReadByte();
                Data.ReadInt16(); // Dado 1
                Data.ReadInt16(); // Dado 2
                Data.ReadInt16(); // Dado 3
                Data.ReadInt16(); // Dado 4
                Data.ReadByte(); // Zona

                // Bloqueio direcional
                Lists.Map.Tile[x, y].Block = new bool[(byte)Game.Directions.Count];
                for (byte i = 0; i < (byte)Game.Directions.Count; i++)
                    Lists.Map.Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        Lists.Map.Light = new Lists.Structures.Map_Light[Data.ReadByte()];
        if (Lists.Map.Light.GetUpperBound(0) > 0)
            for (byte i = 0; i < Lists.Map.Light.Length; i++)
            {
                Lists.Map.Light[i].X = Data.ReadByte();
                Lists.Map.Light[i].Y = Data.ReadByte();
                Lists.Map.Light[i].Width = Data.ReadByte();
                Lists.Map.Light[i].Height = Data.ReadByte();
            }

        // NPCs
        Lists.Map.NPC = new short[Data.ReadByte() + 1];
        for (byte i = 1; i < Lists.Map.NPC.Length; i++)
        {
            Lists.Map.NPC[i] = Data.ReadInt16();
            Data.ReadByte(); // Zone
            Data.ReadBoolean(); // Spawn
            Data.ReadByte(); // X
            Data.ReadByte(); // Y
        }

        // Salva o mapa
        Write.Map(Map_Num);

        // Redimensiona as partículas do clima
        global::Map.Weather_Update();
        global::Map.Autotile.Update();
    }

    private static void JoinMap()
    {
        // Se tiver, reproduz a música de fundo do mapa
        if (Lists.Map.Music > 0)
            Audio.Music.Play((Audio.Musics)Lists.Map.Music);
        else
            Audio.Music.Stop();
    }

    private static void Latency()
    {
        // Define a latência
        Socket.Latency = Environment.TickCount - Socket.Latency_Send;
    }

    private static void Message(NetIncomingMessage Data)
    {
        // Adiciona a mensagem
        string Text = Data.ReadString();
        Color Color = Color.FromArgb(Data.ReadInt32());
        Chat.AddText(Text, new SFML.Graphics.Color(Color.R, Color.G, Color.B));
    }

    private static void Items(NetIncomingMessage Data)
    {
        // Itens a serem removidas
        Dictionary<Guid, Lists.Structures.Item> ToRemove = new Dictionary<Guid, Lists.Structures.Item>(Lists.Item);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Item Item;

            // Obtém o dado
            if (Lists.Item.ContainsKey(ID))
            {
                Item = Lists.Item[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Item = new Lists.Structures.Item(ID);
                Lists.Item.Add(Item.ID, Item);
            }

            // Lê os dados
            Item.Name = Data.ReadString();
            Item.Description = Data.ReadString();
            Item.Texture = Data.ReadInt16();
            Item.Type = Data.ReadByte();
            Data.ReadBoolean(); // Stackable
            Item.Bind = (Game.BindOn)Data.ReadByte();
            Item.Rarity = Data.ReadByte();
            Item.Req_Level = Data.ReadInt16();
            Item.Req_Class = (Lists.Structures.Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
            Item.Potion_Experience = Data.ReadInt32();
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++) Item.Potion_Vital[v] = Data.ReadInt16();
            Item.Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Game.Attributes.Count; a++) Item.Equip_Attribute[a] = Data.ReadInt16();
            Item.Weapon_Damage = Data.ReadInt16();
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Item.Remove(Remove);
    }

    private static void Map_Items(NetIncomingMessage Data)
    {
        // Quantidade
        Lists.Temp_Map.Item = new Lists.Structures.Map_Items[Data.ReadInt16() + 1];

        // Lê os dados de todos
        for (byte i = 1; i < Lists.Temp_Map.Item.Length; i++)
        {
            // Geral
            Lists.Temp_Map.Item[i].Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())); 
            Lists.Temp_Map.Item[i].X = Data.ReadByte();
            Lists.Temp_Map.Item[i].Y = Data.ReadByte();
        }
    }

    private static void Party(NetIncomingMessage Data)
    {
        // Lê os dados do grupo
        Player.Me.Party = new Player.Structure[Data.ReadByte()];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(Data.ReadString());
    }

    private static void Party_Invitation(NetIncomingMessage Data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Lists.Options.Party)
        {
            Send.Party_Decline();
            return;
        }

        // Abre a janela de convite para o grupo
        Utilities.Party_Invitation = Data.ReadString();
        Panels.Get("Party_Invitation").Visible = true;
    }

    private static void Trade(NetIncomingMessage Data)
    {
        bool State = Data.ReadBoolean();

        // Visibilidade do painel
        Panels.Get("Trade").Visible = Data.ReadBoolean();

        if (State)
        {
            // Reseta os botões
            Buttons.Get("Trade_Offer_Confirm").Visible = true;
            Panels.Get("Trade_Amount").Visible = Buttons.Get("Trade_Offer_Accept").Visible = Buttons.Get("Trade_Offer_Decline").Visible = false;
            Panels.Get("Trade_Offer_Disable").Visible = false;

            // Limpa os dados
            Player.Me.Trade_Offer = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
            Player.Me.Trade_Their_Offer = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
        }
        else
        {
            // Limpa os dados
            Player.Me.Trade_Offer = null;
            Player.Me.Trade_Their_Offer = null;
        }
    }

    private static void Trade_Invitation(NetIncomingMessage Data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Lists.Options.Trade)
        {
            Send.Trade_Decline();
            return;
        }

        // Abre a janela de convite para o grupo
        Utilities.Trade_Invitation = Data.ReadString();
        Panels.Get("Trade_Invitation").Visible = true;
    }

    private static void Trade_State(NetIncomingMessage Data)
    {
        switch ((Game.Trade_Status)Data.ReadByte())
        {
            case Game.Trade_Status.Accepted:
            case Game.Trade_Status.Declined:
                Buttons.Get("Trade_Offer_Confirm").Visible = true;
                Buttons.Get("Trade_Offer_Accept").Visible = Buttons.Get("Trade_Offer_Decline").Visible = false;
                Panels.Get("Trade_Offer_Disable").Visible = false;
                break;
            case Game.Trade_Status.Confirmed:
                Buttons.Get("Trade_Offer_Confirm").Visible = false;
                Buttons.Get("Trade_Offer_Accept").Visible = Buttons.Get("Trade_Offer_Decline").Visible = true;
                Panels.Get("Trade_Offer_Disable").Visible = false;
                break;
        }
    }

    private static void Trade_Offer(NetIncomingMessage Data)
    {
        // Recebe os dados da oferta
        if (Data.ReadBoolean())
            for (byte i = 1; i <= Game.Max_Inventory; i++)
            {
                Player.Me.Trade_Offer[i].Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
                Player.Me.Trade_Offer[i].Amount = Data.ReadInt16();
            }
        else
            for (byte i = 1; i <= Game.Max_Inventory; i++)
            {
                Player.Me.Trade_Their_Offer[i].Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
                Player.Me.Trade_Their_Offer[i].Amount = Data.ReadInt16();
            }
    }

    private static void Shops(NetIncomingMessage Data)
    {
        // Lojas a serem removidas
        Dictionary<Guid, Lists.Structures.Shop> ToRemove = new Dictionary<Guid, Lists.Structures.Shop>(Lists.Shop);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Shop Shop;

            // Obtém o dado
            if (Lists.Shop.ContainsKey(ID))
            {
                Shop = Lists.Shop[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Shop = new Lists.Structures.Shop(ID);
                Lists.Shop.Add(Shop.ID, Shop);
            }

            // Redimensiona os valores necessários 
            Shop.Sold = new Lists.Structures.Shop_Item[Data.ReadByte()];
            Shop.Bought = new Lists.Structures.Shop_Item[Data.ReadByte()];

            // Lê os dados
            Shop.Name = Data.ReadString();
            Shop.Currency = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            for (byte j = 0; j < Shop.Sold.Length; j++)
                Shop.Sold[j] = new Lists.Structures.Shop_Item
                {
                    Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())),
                    Amount = Data.ReadInt16(),
                    Price = Data.ReadInt16()
                };
            for (byte j = 0; j < Shop.Bought.Length; j++)
                Shop.Bought[j] = new Lists.Structures.Shop_Item
                {
                    Item = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())),
                    Amount = Data.ReadInt16(),
                    Price = Data.ReadInt16()
                };
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Shop.Remove(Remove);
    }

    private static void Shop_Open(NetIncomingMessage Data)
    {
        // Abre a loja
        Utilities.Shop_Open = (Lists.Structures.Shop)Lists.GetData(Lists.Shop, new Guid(Data.ReadString()));
        Panels.Get("Shop").Visible = Utilities.Shop_Open != null;
    }

    private static void NPCs(NetIncomingMessage Data)
    {
        // Quantidade
        Lists.NPC = new Lists.Structures.NPC[Data.ReadInt16()];

        // Lê os dados de todos
        for (byte i = 1; i < Lists.NPC.Length; i++)
        {
            // Geral
            Lists.NPC[i].Name = Data.ReadString();
            Lists.NPC[i].SayMsg = Data.ReadString();
            Lists.NPC[i].Texture = Data.ReadInt16();
            Lists.NPC[i].Type = Data.ReadByte();

            // Vitais
            Lists.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
                Lists.NPC[i].Vital[n] = Data.ReadInt16();
        }
    }

    private static void Map_NPCs(NetIncomingMessage Data)
    {
        // Lê os dados
        Lists.Temp_Map.NPC = new NPC[Data.ReadInt16()];
        for (byte i = 1; i < Lists.Temp_Map.NPC.Length; i++)
        {
            Lists.Temp_Map.NPC[i] = new NPC();
            Lists.Temp_Map.NPC[i].X2 = 0;
            Lists.Temp_Map.NPC[i].Y2 = 0;
            Lists.Temp_Map.NPC[i].Index = Data.ReadInt16();
            Lists.Temp_Map.NPC[i].X = Data.ReadByte();
            Lists.Temp_Map.NPC[i].Y = Data.ReadByte();
            Lists.Temp_Map.NPC[i].Direction = (Game.Directions)Data.ReadByte();

            // Vitais
            Lists.Temp_Map.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
                Lists.Temp_Map.NPC[i].Vital[n] = Data.ReadInt16();
        }
    }

    private static void Map_NPC(NetIncomingMessage Data)
    {
        // Lê os dados
        byte i = Data.ReadByte();
        Lists.Temp_Map.NPC[i].X2 = 0;
        Lists.Temp_Map.NPC[i].Y2 = 0;
        Lists.Temp_Map.NPC[i].Index = Data.ReadInt16();
        Lists.Temp_Map.NPC[i].X = Data.ReadByte();
        Lists.Temp_Map.NPC[i].Y = Data.ReadByte();
        Lists.Temp_Map.NPC[i].Direction = (Game.Directions)Data.ReadByte();
        Lists.Temp_Map.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Lists.Temp_Map.NPC[i].Vital[n] = Data.ReadInt16();
    }

    private static void Map_NPC_Movement(NetIncomingMessage Data)
    {
        // Lê os dados
        byte i = Data.ReadByte();
        byte x = Lists.Temp_Map.NPC[i].X, y = Lists.Temp_Map.NPC[i].Y;
        Lists.Temp_Map.NPC[i].X2 = 0;
        Lists.Temp_Map.NPC[i].Y2 = 0;
        Lists.Temp_Map.NPC[i].X = Data.ReadByte();
        Lists.Temp_Map.NPC[i].Y = Data.ReadByte();
        Lists.Temp_Map.NPC[i].Direction = (Game.Directions)Data.ReadByte();
        Lists.Temp_Map.NPC[i].Movement = (Game.Movements)Data.ReadByte();

        // Posição exata do jogador
        if (x != Lists.Temp_Map.NPC[i].X || y != Lists.Temp_Map.NPC[i].Y)
            switch (Lists.Temp_Map.NPC[i].Direction)
            {
                case Game.Directions.Up: Lists.Temp_Map.NPC[i].Y2 = Game.Grid; break;
                case Game.Directions.Down: Lists.Temp_Map.NPC[i].Y2 = Game.Grid * -1; break;
                case Game.Directions.Right: Lists.Temp_Map.NPC[i].X2 = Game.Grid * -1; break;
                case Game.Directions.Left: Lists.Temp_Map.NPC[i].X2 = Game.Grid; break;
            }
    }

    private static void Map_NPC_Attack(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();
        string Victim = Data.ReadString();
        byte Victim_Type = Data.ReadByte();

        // Inicia o ataque
        Lists.Temp_Map.NPC[Index].Attacking = true;
        Lists.Temp_Map.NPC[Index].Attack_Timer = Environment.TickCount;

        // Sofrendo dano
        if (Victim != string.Empty)
            if (Victim_Type == (byte)Game.Target.Player)
            {
                Player.Structure Victim_Data = Player.Get(Victim);
                Victim_Data.Hurt = Environment.TickCount;
                Lists.Temp_Map.Blood.Add(new Lists.Structures.Map_Blood((byte)Game.Random.Next(0, 3), Victim_Data.X, Victim_Data.Y, 255));
            }
            else if (Victim_Type == (byte)Game.Target.NPC)
            {
                Lists.Temp_Map.NPC[byte.Parse(Victim)].Hurt = Environment.TickCount;
                Lists.Temp_Map.Blood.Add(new Lists.Structures.Map_Blood((byte)Game.Random.Next(0, 3), Lists.Temp_Map.NPC[byte.Parse(Victim)].X, Lists.Temp_Map.NPC[byte.Parse(Victim)].Y, 255));
            }
    }

    private static void Map_NPC_Direction(NetIncomingMessage Data)
    {
        // Define a direção de determinado NPC
        byte i = Data.ReadByte();
        Lists.Temp_Map.NPC[i].Direction = (Game.Directions)Data.ReadByte();
        Lists.Temp_Map.NPC[i].X2 = 0;
        Lists.Temp_Map.NPC[i].Y2 = 0;
    }

    private static void Map_NPC_Vitals(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Define os vitais de determinado NPC
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
            Lists.Temp_Map.NPC[Index].Vital[n] = Data.ReadInt16();
    }

    private static void Map_NPC_Died(NetIncomingMessage Data)
    {
        byte i = Data.ReadByte();

        // Limpa os dados do NPC
        Lists.Temp_Map.NPC[i].X2 = 0;
        Lists.Temp_Map.NPC[i].Y2 = 0;
        Lists.Temp_Map.NPC[i].Index = 0;
        Lists.Temp_Map.NPC[i].X = 0;
        Lists.Temp_Map.NPC[i].Y = 0;
        Lists.Temp_Map.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
    }
}