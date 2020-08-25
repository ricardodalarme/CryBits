using Entities;
using Interface;
using Lidgren.Network;
using Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Logic.Utils;

namespace Network
{
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
            Panels.SelectCharacter = 0;
            Class.List = new Dictionary<Guid, Class>();

            // Abre o painel de seleção de personagens
            Panels.Menu_Close();
            Panels.List["SelectCharacter"].Visible = true;
        }

        private static void Join(NetIncomingMessage Data)
        {
            // Reseta alguns valores
            Player.List = new List<Player>();
            Item.List = new Dictionary<Guid, Item>();
            Shop.List = new Dictionary<Guid, Shop>();
            NPC.List = new Dictionary<Guid, NPC>();
            Entities.Map.List = new Dictionary<Guid, Map>();
            TempMap.List = new Dictionary<Guid, TempMap>();

            // Definir os valores que são enviados do servidor
            Player.Me = new Me_Structure(Data.ReadString());
            Player.List.Add(Player.Me);
        }

        private static void CreateCharacter()
        {
            // Reseta os valores
            TextBoxes.List["CreateCharacter_Name"].Text = string.Empty;
            CheckBoxes.List["GenderMale"].Checked = true;
            CheckBoxes.List["GenderFemale"].Checked = false;
            Panels.CreateCharacter_Class = 0;
            Panels.CreateCharacter_Tex = 0;

            // Abre o painel de criação de personagem
            Panels.Menu_Close();
            Panels.List["CreateCharacter"].Visible = true;
        }

        private static void Classes(NetIncomingMessage Data)
        {
            // Classes a serem removidas
            Dictionary<Guid, Class> ToRemove = new Dictionary<Guid, Class>(Class.List);

            // Quantidade de classes
            short Count = Data.ReadByte();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Class Class;

                // Obtém o dado
                if (Class.List.ContainsKey(ID))
                {
                    Class = Class.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Class = new Class(ID);
                    Class.List.Add(Class.ID, Class);
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
            foreach (Guid Remove in ToRemove.Keys) Shop.List.Remove(Remove);
        }

        private static void Characters(NetIncomingMessage Data)
        {
            // Redimensiona a lista
            Panels.Characters = new Panels.TempCharacter[Data.ReadByte()];

            for (byte i = 0; i < Panels.Characters.Length; i++)
            {
                // Recebe os dados do personagem
                Panels.Characters[i] = new Panels.TempCharacter
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
            Chat.Order = new List<Chat.Structure>();
            Chat.Lines_First = 0;
            Loop.Chat_Timer = Environment.TickCount + Chat.Sleep_Timer;
            TextBoxes.List["Chat"].Text = string.Empty;
            CheckBoxes.List["Options_Sounds"].Checked = Option.Sounds;
            CheckBoxes.List["Options_Musics"].Checked = Option.Musics;
            CheckBoxes.List["Options_Chat"].Checked = Option.Chat;
            CheckBoxes.List["Options_FPS"].Checked = Option.FPS;
            CheckBoxes.List["Options_Latency"].Checked = Option.Latency;
            CheckBoxes.List["Options_Trade"].Checked = Option.Trade;
            CheckBoxes.List["Options_Party"].Checked = Option.Party;
            Loop.Chat_Timer = Loop.Chat_Timer = Environment.TickCount + 10000;
            Panels.Infomation_ID = Guid.Empty;

            // Reseta a interface
            Panels.List["Menu_Character"].Visible = false;
            Panels.List["Menu_Inventory"].Visible = false;
            Panels.List["Menu_Options"].Visible = false;
            Panels.List["Chat"].Visible = false;
            Panels.List["Drop"].Visible = false;
            Panels.List["Party_Invitation"].Visible = false;
            Panels.List["Trade"].Visible = false;
            Buttons.List["Trade_Offer_Confirm"].Visible = true;
            Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
            Panels.List["Trade_Offer_Disable"].Visible = false;
            Panels.List["Shop"].Visible = false;
            Panels.List["Shop_Sell"].Visible = false;

            // Abre o jogo
            Audio.Music.Stop();
            Windows.Current = Windows.Types.Game;
        }

        private static void Player_Data(NetIncomingMessage Data)
        {
            string Name = Data.ReadString();
            Player Player;

            // Adiciona o jogador à lista
            if (Name != Player.Me.Name)
            {
                Player = new Player(Name);
                Player.List.Add(Player);
            }
            else
                Player = Player.Me;

            // Defini os dados do jogador
            Player.Texture_Num = Data.ReadInt16();
            Player.Level = Data.ReadInt16();
            Player.Map = TempMap.List[new Guid(Data.ReadString())];
            Player.X = Data.ReadByte();
            Player.Y = Data.ReadByte();
            Player.Direction = (Directions)Data.ReadByte();
            for (byte n = 0; n < (byte)Vitals.Count; n++)
            {
                Player.Vital[n] = Data.ReadInt16();
                Player.Max_Vital[n] = Data.ReadInt16();
            }
            for (byte n = 0; n < (byte)Attributes.Count; n++) Player.Attribute[n] = Data.ReadInt16();
            for (byte n = 0; n < (byte)Equipments.Count; n++) Player.Equipment[n] = Item.Get(new Guid(Data.ReadString()));
            Mapper.Current = Player.Map;
        }

        private static void Player_Position(NetIncomingMessage Data)
        {
            Player Player = Player.Get(Data.ReadString());

            // Defini os dados do jogador
            Player.X = Data.ReadByte();
            Player.Y = Data.ReadByte();
            Player.Direction = (Directions)Data.ReadByte();

            // Para a movimentação
            Player.X2 = 0;
            Player.Y2 = 0;
            Player.Movement = Movements.Stopped;
        }

        private static void Player_Vitals(NetIncomingMessage Data)
        {
            Player Player = Player.Get(Data.ReadString());

            // Define os dados
            for (byte i = 0; i < (byte)Vitals.Count; i++)
            {
                Player.Vital[i] = Data.ReadInt16();
                Player.Max_Vital[i] = Data.ReadInt16();
            }
        }

        private static void Player_Equipments(NetIncomingMessage Data)
        {
            Player Player = Player.Get(Data.ReadString());

            // Altera os dados dos equipamentos do jogador
            for (byte i = 0; i < (byte)Equipments.Count; i++) Player.Equipment[i] = Item.Get(new Guid(Data.ReadString()));
        }

        private static void Player_Leave(NetIncomingMessage Data)
        {
            // Limpa os dados do jogador
            Player.List.Remove(Player.Get(Data.ReadString()));
        }

        private static void Player_Move(NetIncomingMessage Data)
        {
            Player Player = Player.Get(Data.ReadString());

            // Move o jogador
            Player.X = Data.ReadByte();
            Player.Y = Data.ReadByte();
            Player.Direction = (Directions)Data.ReadByte();
            Player.Movement = (Movements)Data.ReadByte();
            Player.X2 = 0;
            Player.Y2 = 0;

            // Posição exata do jogador
            switch (Player.Direction)
            {
                case Directions.Up: Player.Y2 = Grid; break;
                case Directions.Down: Player.Y2 = Grid * -1; break;
                case Directions.Right: Player.X2 = Grid * -1; break;
                case Directions.Left: Player.X2 = Grid; break;
            }
        }

        private static void Player_Direction(NetIncomingMessage Data)
        {
            // Altera a posição do jogador
            Player.Get(Data.ReadString()).Direction = (Directions)Data.ReadByte();
        }

        private static void Player_Attack(NetIncomingMessage Data)
        {
            Player Player = Player.Get(Data.ReadString());
            string Victim = Data.ReadString();
            byte Victim_Type = Data.ReadByte();

            // Inicia o ataque
            Player.Attacking = true;
            Player.Attack_Timer = Environment.TickCount;

            // Sofrendo dano
            if (Victim != string.Empty)
                if (Victim_Type == (byte)Target.Player)
                {
                    Player Victim_Data = Player.Get(Victim);
                    Victim_Data.Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new TMap_Blood((byte)MyRandom.Next(0, 3), Victim_Data.X, Victim_Data.Y, 255));
                }
                else if (Victim_Type == (byte)Target.NPC)
                {
                    Mapper.Current.NPC[byte.Parse(Victim)].Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new TMap_Blood((byte)MyRandom.Next(0, 3), Mapper.Current.NPC[byte.Parse(Victim)].X, Mapper.Current.NPC[byte.Parse(Victim)].Y, 255));
                }
        }

        private static void Player_Experience(NetIncomingMessage Data)
        {
            // Define os dados
            Player.Me.Experience = Data.ReadInt32();
            Player.Me.ExpNeeded = Data.ReadInt32();
            Player.Me.Points = Data.ReadByte();

            // Manipula a visibilidade dos botões
            Buttons.List["Attributes_Strength"].Visible = Player.Me.Points > 0;
            Buttons.List["Attributes_Resistance"].Visible = Player.Me.Points > 0;
            Buttons.List["Attributes_Intelligence"].Visible = Player.Me.Points > 0;
            Buttons.List["Attributes_Agility"].Visible = Player.Me.Points > 0;
            Buttons.List["Attributes_Vitality"].Visible = Player.Me.Points > 0;
        }

        private static void Player_Inventory(NetIncomingMessage Data)
        {
            // Define os dados
            for (byte i = 1; i <= Max_Inventory; i++)
            {
                Player.Me.Inventory[i].Item = Item.Get(new Guid(Data.ReadString()));
                Player.Me.Inventory[i].Amount = Data.ReadInt16();
            }
        }

        private static void Player_Hotbar(NetIncomingMessage Data)
        {
            // Define os dados
            for (byte i = 0; i < Max_Hotbar; i++)
            {
                Player.Me.Hotbar[i].Type = Data.ReadByte();
                Player.Me.Hotbar[i].Slot = Data.ReadByte();
            }
        }

        private static void Map_Revision(NetIncomingMessage Data)
        {
            bool Needed = false;
            Guid ID = new Guid(Data.ReadString());

            // Limpa todos os outros jogadores
            for (byte i = 0; i < Player.List.Count; i++)
                if (Player.List[i] != Player.Me)
                    Player.List.RemoveAt(i);

            /*
            // Verifica se é necessário baixar os dados do mapa
            if (System.IO.File.Exists(Directories.Maps_Data.FullName + ID + Directories.Format))
            {
                Needed = true;
                break;
                Read.Map(ID);
                if (Map.List[ID].Revision != Data.ReadInt16())
                    Needed = true;
            }
            else
                Needed = true;
            */
            Needed = true;
            // Solicita os dados do mapa
            Send.RequestMap(Needed);

            // Reseta os sangues do mapa
            Mapper.Current.Blood = new List<TMap_Blood>();
        }

        private static void Map(NetIncomingMessage Data)
        {
            Guid ID = new Guid(Data.ReadString());
            Map Map;

            // Obtém o dado
            if (Map.List.ContainsKey(ID)) Map = Map.List[ID];
            else
            {
                Map = new Map(ID);
                Map.List.Add(ID, Map);
                TempMap.List.Add(ID, new TempMap(Map));
            }
            Mapper.Current = TempMap.List[ID];

            // Lê os dados
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
            Data.ReadByte(); // Luz global

            // Ligações
            Map.Link = new short[(byte)Directions.Count];
            for (byte i = 0; i < (byte)Directions.Count; i++)
                Data.ReadString();

            // Azulejos
            byte Num_Layers = Data.ReadByte();

            // Redimensiona os dados

            for (byte x = 0; x < Map_Width; x++)
                for (byte y = 0; y < Map_Height; y++)
                    Map.Tile[x, y].Data = new Map_Tile_Data[(byte)global::Mapper.Layers.Count, Num_Layers];

            // Lê os azulejos
            for (byte i = 0; i < Num_Layers; i++)
            {
                // Dados básicos
                Data.ReadString(); // Name
                byte t = Data.ReadByte(); // Tipo

                // Azulejos
                for (byte x = 0; x < Map_Width; x++)
                    for (byte y = 0; y < Map_Height; y++)
                    {
                        Map.Tile[x, y].Data[t, i].X = Data.ReadByte();
                        Map.Tile[x, y].Data[t, i].Y = Data.ReadByte();
                        Map.Tile[x, y].Data[t, i].Tile = Data.ReadByte();
                        Map.Tile[x, y].Data[t, i].Automatic = Data.ReadBoolean();
                        Map.Tile[x, y].Data[t, i].Mini = new Point[4];
                    }
            }

            // Dados específicos dos azulejos
            for (byte x = 0; x < Map_Width; x++)
                for (byte y = 0; y < Map_Height; y++)
                {
                    Map.Tile[x, y].Attribute = Data.ReadByte();
                    Data.ReadString(); // Dado 1
                    Data.ReadInt16(); // Dado 2
                    Data.ReadInt16(); // Dado 3
                    Data.ReadInt16(); // Dado 4
                    Data.ReadByte(); // Zona

                    // Bloqueio direcional
                    Map.Tile[x, y].Block = new bool[(byte)Directions.Count];
                    for (byte i = 0; i < (byte)Directions.Count; i++)
                        Map.Tile[x, y].Block[i] = Data.ReadBoolean();
                }

            // Luzes
            Map.Light = new Map_Light[Data.ReadByte()];
            if (Map.Light.GetUpperBound(0) > 0)
                for (byte i = 0; i < Map.Light.Length; i++)
                {
                    Map.Light[i].X = Data.ReadByte();
                    Map.Light[i].Y = Data.ReadByte();
                    Map.Light[i].Width = Data.ReadByte();
                    Map.Light[i].Height = Data.ReadByte();
                }

            // NPCs
            Map.NPC = new short[Data.ReadByte()];
            for (byte i = 0; i < Map.NPC.Length; i++)
            {
                Map.NPC[i] = Data.ReadInt16();
                Data.ReadByte(); // Zone
                Data.ReadBoolean(); // Spawn
                Data.ReadByte(); // X
                Data.ReadByte(); // Y
            }

            // Salva o mapa
            Library.Write.Map(Map);

            // Redimensiona as partículas do clima
            Mapper.Weather_Update();
            MapAutoTile.Update();
        }

        private static void JoinMap()
        {
            // Se tiver, reproduz a música de fundo do mapa
            if (Mapper.Current.Data.Music > 0)
                Audio.Music.Play((Audio.Musics)Mapper.Current.Data.Music);
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
            Dictionary<Guid, Item> ToRemove = new Dictionary<Guid, Item>(Item.List);

            // Quantidade de lojas
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Item Item;

                // Obtém o dado
                if (Item.List.ContainsKey(ID))
                {
                    Item = Item.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Item = new Item(ID);
                    Item.List.Add(Item.ID, Item);
                }

                // Lê os dados
                Item.Name = Data.ReadString();
                Item.Description = Data.ReadString();
                Item.Texture = Data.ReadInt16();
                Item.Type = Data.ReadByte();
                Data.ReadBoolean(); // Stackable
                Item.Bind = (BindOn)Data.ReadByte();
                Item.Rarity = Data.ReadByte();
                Item.Req_Level = Data.ReadInt16();
                Item.Req_Class = Class.Get(new Guid(Data.ReadString()));
                Item.Potion_Experience = Data.ReadInt32();
                for (byte v = 0; v < (byte)Vitals.Count; v++) Item.Potion_Vital[v] = Data.ReadInt16();
                Item.Equip_Type = Data.ReadByte();
                for (byte a = 0; a < (byte)Attributes.Count; a++) Item.Equip_Attribute[a] = Data.ReadInt16();
                Item.Weapon_Damage = Data.ReadInt16();
            }

            // Remove as lojas que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) Item.List.Remove(Remove);
        }

        private static void Map_Items(NetIncomingMessage Data)
        {
            // Quantidade
            Mapper.Current.Item = new TMap_Items[Data.ReadByte()];

            // Lê os dados de todos
            for (byte i = 0; i < Mapper.Current.Item.Length; i++)
            {
                // Geral
                Mapper.Current.Item[i] = new TMap_Items();
                Mapper.Current.Item[i].Item = Item.Get(new Guid(Data.ReadString()));
                Mapper.Current.Item[i].X = Data.ReadByte();
                Mapper.Current.Item[i].Y = Data.ReadByte();
            }
        }

        private static void Party(NetIncomingMessage Data)
        {
            // Lê os dados do grupo
            Player.Me.Party = new Player[Data.ReadByte()];
            for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(Data.ReadString());
        }

        private static void Party_Invitation(NetIncomingMessage Data)
        {
            // Nega o pedido caso o jogador não quiser receber convites
            if (!Option.Party)
            {
                Send.Party_Decline();
                return;
            }

            // Abre a janela de convite para o grupo
            Panels.Party_Invitation = Data.ReadString();
            Panels.List["Party_Invitation"].Visible = true;
        }

        private static void Trade(NetIncomingMessage Data)
        {
            bool State = Data.ReadBoolean();

            // Visibilidade do painel
            Panels.List["Trade"].Visible = Data.ReadBoolean();

            if (State)
            {
                // Reseta os botões
                Buttons.List["Trade_Offer_Confirm"].Visible = true;
                Panels.List["Trade_Amount"].Visible = Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
                Panels.List["Trade_Offer_Disable"].Visible = false;

                // Limpa os dados
                Player.Me.Trade_Offer = new Inventory[Max_Inventory + 1];
                Player.Me.Trade_Their_Offer = new Inventory[Max_Inventory + 1];
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
            if (!Option.Trade)
            {
                Send.Trade_Decline();
                return;
            }

            // Abre a janela de convite para o grupo
            Panels.Trade_Invitation = Data.ReadString();
            Panels.List["Trade_Invitation"].Visible = true;
        }

        private static void Trade_State(NetIncomingMessage Data)
        {
            switch ((Trade_Status)Data.ReadByte())
            {
                case Trade_Status.Accepted:
                case Trade_Status.Declined:
                    Buttons.List["Trade_Offer_Confirm"].Visible = true;
                    Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
                    Panels.List["Trade_Offer_Disable"].Visible = false;
                    break;
                case Trade_Status.Confirmed:
                    Buttons.List["Trade_Offer_Confirm"].Visible = false;
                    Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = true;
                    Panels.List["Trade_Offer_Disable"].Visible = false;
                    break;
            }
        }

        private static void Trade_Offer(NetIncomingMessage Data)
        {
            // Recebe os dados da oferta
            if (Data.ReadBoolean())
                for (byte i = 1; i <= Max_Inventory; i++)
                {
                    Player.Me.Trade_Offer[i].Item = Item.Get(new Guid(Data.ReadString()));
                    Player.Me.Trade_Offer[i].Amount = Data.ReadInt16();
                }
            else
                for (byte i = 1; i <= Max_Inventory; i++)
                {
                    Player.Me.Trade_Their_Offer[i].Item = Item.Get(new Guid(Data.ReadString()));
                    Player.Me.Trade_Their_Offer[i].Amount = Data.ReadInt16();
                }
        }

        private static void Shops(NetIncomingMessage Data)
        {
            // Lojas a serem removidas
            Dictionary<Guid, Shop> ToRemove = new Dictionary<Guid, Shop>(Shop.List);

            // Quantidade de lojas
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Shop Shop;

                // Obtém o dado
                if (Shop.List.ContainsKey(ID))
                {
                    Shop = Shop.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Shop = new Shop(ID);
                    Shop.List.Add(Shop.ID, Shop);
                }

                // Redimensiona os valores necessários 
                Shop.Sold = new Shop_Item[Data.ReadByte()];
                Shop.Bought = new Shop_Item[Data.ReadByte()];

                // Lê os dados
                Shop.Name = Data.ReadString();
                Shop.Currency = Item.Get(new Guid(Data.ReadString()));
                for (byte j = 0; j < Shop.Sold.Length; j++)
                    Shop.Sold[j] = new Shop_Item
                    {
                        Item = Item.Get(new Guid(Data.ReadString())),
                        Amount = Data.ReadInt16(),
                        Price = Data.ReadInt16()
                    };
                for (byte j = 0; j < Shop.Bought.Length; j++)
                    Shop.Bought[j] = new Shop_Item
                    {
                        Item = Item.Get(new Guid(Data.ReadString())),
                        Amount = Data.ReadInt16(),
                        Price = Data.ReadInt16()
                    };
            }

            // Remove as lojas que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) Shop.List.Remove(Remove);
        }

        private static void Shop_Open(NetIncomingMessage Data)
        {
            // Abre a loja
            Panels.Shop_Open = Shop.Get(new Guid(Data.ReadString()));
            Panels.List["Shop"].Visible = Panels.Shop_Open != null;
        }

        private static void NPCs(NetIncomingMessage Data)
        {
            // Lojas a serem removidas
            Dictionary<Guid, NPC> ToRemove = new Dictionary<Guid, NPC>(NPC.List);

            // Quantidade de lojas
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                NPC NPC;

                // Obtém o dado
                if (NPC.List.ContainsKey(ID))
                {
                    NPC = NPC.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    NPC = new NPC(ID);
                    NPC.List.Add(NPC.ID, NPC);
                }

                // Geral
                NPC.Name = Data.ReadString();
                NPC.SayMsg = Data.ReadString();
                NPC.Texture = Data.ReadInt16();
                NPC.Type = Data.ReadByte();

                // Vitais
                NPC.Vital = new short[(byte)Vitals.Count];
                for (byte n = 0; n < (byte)Vitals.Count; n++)
                    NPC.Vital[n] = Data.ReadInt16();
            }

            // Remove as lojas que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) NPC.List.Remove(Remove);
        }

        private static void Map_NPCs(NetIncomingMessage Data)
        {
            // Lê os dados
            Mapper.Current.NPC = new TempNPC[Data.ReadInt16()];
            for (byte i = 0; i < Mapper.Current.NPC.Length; i++)
            {
                Mapper.Current.NPC[i] = new TempNPC();
                Mapper.Current.NPC[i].X2 = 0;
                Mapper.Current.NPC[i].Y2 = 0;
                Mapper.Current.NPC[i].Data = NPC.Get(new Guid(Data.ReadString()));
                Mapper.Current.NPC[i].X = Data.ReadByte();
                Mapper.Current.NPC[i].Y = Data.ReadByte();
                Mapper.Current.NPC[i].Direction = (Directions)Data.ReadByte();

                // Vitais
                for (byte n = 0; n < (byte)Vitals.Count; n++)
                    Mapper.Current.NPC[i].Vital[n] = Data.ReadInt16();
            }
        }

        private static void Map_NPC(NetIncomingMessage Data)
        {
            // Lê os dados
            byte i = Data.ReadByte();
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].Data = NPC.Get(new Guid(Data.ReadString()));
            Mapper.Current.NPC[i].X = Data.ReadByte();
            Mapper.Current.NPC[i].Y = Data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)Data.ReadByte();
            Mapper.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
            for (byte n = 0; n < (byte)Vitals.Count; n++) Mapper.Current.NPC[i].Vital[n] = Data.ReadInt16();
        }

        private static void Map_NPC_Movement(NetIncomingMessage Data)
        {
            // Lê os dados
            byte i = Data.ReadByte();
            byte x = Mapper.Current.NPC[i].X, y = Mapper.Current.NPC[i].Y;
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].X = Data.ReadByte();
            Mapper.Current.NPC[i].Y = Data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)Data.ReadByte();
            Mapper.Current.NPC[i].Movement = (Movements)Data.ReadByte();

            // Posição exata do jogador
            if (x != Mapper.Current.NPC[i].X || y != Mapper.Current.NPC[i].Y)
                switch (Mapper.Current.NPC[i].Direction)
                {
                    case Directions.Up: Mapper.Current.NPC[i].Y2 = Grid; break;
                    case Directions.Down: Mapper.Current.NPC[i].Y2 = Grid * -1; break;
                    case Directions.Right: Mapper.Current.NPC[i].X2 = Grid * -1; break;
                    case Directions.Left: Mapper.Current.NPC[i].X2 = Grid; break;
                }
        }

        private static void Map_NPC_Attack(NetIncomingMessage Data)
        {
            byte Index = Data.ReadByte();
            string Victim = Data.ReadString();
            byte Victim_Type = Data.ReadByte();

            // Inicia o ataque
            Mapper.Current.NPC[Index].Attacking = true;
            Mapper.Current.NPC[Index].Attack_Timer = Environment.TickCount;

            // Sofrendo dano
            if (Victim != string.Empty)
                if (Victim_Type == (byte)Target.Player)
                {
                    Player Victim_Data = Player.Get(Victim);
                    Victim_Data.Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new TMap_Blood((byte)MyRandom.Next(0, 3), Victim_Data.X, Victim_Data.Y, 255));
                }
                else if (Victim_Type == (byte)Target.NPC)
                {
                    Mapper.Current.NPC[byte.Parse(Victim)].Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new TMap_Blood((byte)MyRandom.Next(0, 3), Mapper.Current.NPC[byte.Parse(Victim)].X, Mapper.Current.NPC[byte.Parse(Victim)].Y, 255));
                }
        }

        private static void Map_NPC_Direction(NetIncomingMessage Data)
        {
            // Define a direção de determinado NPC
            byte i = Data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)Data.ReadByte();
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
        }

        private static void Map_NPC_Vitals(NetIncomingMessage Data)
        {
            byte Index = Data.ReadByte();

            // Define os vitais de determinado NPC
            for (byte n = 0; n < (byte)Vitals.Count; n++)
                Mapper.Current.NPC[Index].Vital[n] = Data.ReadInt16();
        }

        private static void Map_NPC_Died(NetIncomingMessage Data)
        {
            byte i = Data.ReadByte();

            // Limpa os dados do NPC
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].Data = null;
            Mapper.Current.NPC[i].X = 0;
            Mapper.Current.NPC[i].Y = 0;
            Mapper.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
        }
    }
}